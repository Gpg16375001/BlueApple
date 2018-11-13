LibraryCriNc = {

$CriNc: {
	context: null,	// WebAudioのContext
	voices: [{}],	// ボイス
	buses: [],		// バス
	buffers: {},	// ロードしたAudioBufferが全部入っている
	banks: {},		// ロードしたACBのAudioBufferが入っている
	bankCount: 0,
	browser: null,
	os: null,
	isMobile: false,

	bqfTypeTable: [
		"lowpass",
		"highpass",
		"notch",
		"lowshelf",
		"highshelf",
		"peaking",
	],
	updateBpf: function(bpf, cofLow, cofHigh) {
		// バンドパスパラメータからバイクワッドパラメータに変換
		var maxFreq = CriNc.context.sampleRate / 2;
		var cof1 = Math.min(Math.max(cofLow, 20), maxFreq);
		var cof2 = Math.min(Math.max(cofHigh, 20), maxFreq);
		if (cof1 > cof2 - 10) cof1 = cof2 - 10;
		var bw = Math.log(cof2 / cof1) / Math.log(2);
		var f0 = cof1 * Math.pow(2, bw / 2);
		bpf.frequency.value = f0;
		bpf.Q.value = f0 / (cof2 - cof1);
	},
	defineClasses: function() {
		// ボイス
		/** @constructor */
		CriNc.Voice = function(context, inCh, outCh) {
			this.decoder = null;			// 外部から渡されるデコーダ
			this.playing = false;			// 再生中フラグ
			this.releasing = false;			// 停止中フラグ
			this.paused = false;			// 一時停止されている
			this.rate = 0;					// 再生速度レート
			this.predelay = 0;				// プリディレイ
			this.startOffset = 0;			// シーク時間
			this.samples = 0;				// 再生済サンプル数
			this.stopTime = 0;				// 停止する時刻
			this.lastTime = 0;				// 最後にupdateされた時刻
			this.envTime = 0;				// Sustainまでのエンベロープ時間
			this.envActive = false;			// エンベロープ有効フラグ
			this.envAtk = 0;				// アタック時間
			this.envHld = 0;				// ホールド時間				
			this.envDcy = 0;				// ディケイ時間
			this.envRel = 0;				// リリース時間
			this.envSus = 1;				// サスティンレベル
			this.bpfCofLo = 0;				// バンドパス低域カット周波数
			this.bpfCofHi = 24000;			// バンドパス高域カット周波数

			this.source = null;				// AudioBufferSourceNode
			this.waveData = null;			// WaveData

			// エンベロープのレベル変化、ポーズ時のフェード処理に使うGainNode
			this.level = context.createGain();
			// バイクワッドフィルタ
			this.biq = context.createBiquadFilter();
			// バンドパスフィルタ
			this.bpf = context.createBiquadFilter();
			// マトリクスレベル処理
			this.matrix = new CriNc.Matrix(context, inCh, outCh);
			// バスセンドのルート処理
			this.router = new CriNc.Router(context, 8);

			// バイクワッドフィルタの有効フラグ
			this.biqEnabled = false;
			// バンドパスフィルタの有効フラグ
			this.bpfEnabled = false;
			// バンドパス固定しておく
			this.bpf.type = "bandpass";
			// ルーターに接続しておく
			this.matrix.merge.connect(this.router.levels[0]);
			// 初期化
			this.cleanup();
		};
		// ボイスの関数
		CriNc.Voice.prototype = {
			// オフセット時間をセット
			setStartOffset: function(startOffset) {
				this.startOffset = startOffset * 0.001;	// ミリ秒から秒に
			},
			// ソースノードを作成
			setData: function(waveData) {
				this.waveData = waveData;
			},
			// ノード関連の初期化
			cleanup: function() {
				if (this.source) {
					this.source.disconnect();
					this.source.onended = null;
					this.source = null;
				}

				this.waveData = null;
				this.level.gain.cancelScheduledValues(0);
				this.matrix.reset();
				this.playing = false;
				this.releasing = false;
			},
			setSamplingRate: function(rate) {
				this.rate = rate;
				this.updatePlaybackRate();
			},
			updatePlaybackRate: function() {
				if (!this.waveData) return;
				if (this.paused) {
					// 0だとFirefoxで止まらないのでゼロに近い値をセット
					this.source.playbackRate.value = 0.00000001;
				} else {
					this.source.playbackRate.value = +this.rate / this.waveData.originalSampleRate;
				}
			},
			start: function() {
				if (this.playing) {
					return;
				}
				var waveData = this.waveData;
				var startOffset = this.startOffset;

				var source = CriNc.context.createBufferSource();
				
				// バッファをセット
				source.buffer = waveData.buffer;
				
				if (waveData.loopEnd > 0) {
					// ループ情報をセット
					source.loop = true;
					source.loopStart = waveData.loopStart;
					source.loopEnd = waveData.loopEnd;
					// シーク位置計算
					if (startOffset < waveData.loopEnd) {
						// ループ前
					} else {
						// ループ後
						startOffset = waveData.loopStart + 
							(startOffset - waveData.loopStart) % 
							(waveData.loopEnd - waveData.loopStart);
					}
				}

				// 再生終了コールバック
				source.onended = function() {
					this.cleanup();
				}.bind(this);

				this.source = source;
				source.connect(this.level);
				
				this.updateFilters();
				this.updatePlaybackRate();

				var time = CriNc.context.currentTime + this.predelay;
				var startTime = time;
				var gain = this.level.gain;

				this.samples = 0;
				this.releasing = false;
				this.playing = true;
				this.envTime = startTime;
				this.lastTime = startTime;

				// エンベロープ制御
				if (this.envActive) {
					// アタック
					if (this.envAtk > 0) {
						gain.setValueAtTime(0, time);
						time += this.envAtk;
						gain.linearRampToValueAtTime(1, time);
					} else {
						gain.setValueAtTime(1, time);
					}
					// ホールド
					if (this.envHld > 0) {
						time += this.envHld;
					}
					// ディケイ
					if (this.envDcy > 0) {
						gain.setValueAtTime(1, time);
						time += this.envDcy;
						gain.linearRampToValueAtTime(this.envSus, time);
					}
				} else {
					gain.setValueAtTime(1, startTime);
				}

				// 開始リクエスト
				if (source.loop) {
					source.start(startTime, this.waveData.offset + startOffset);
				} else {
					source.start(startTime, this.waveData.offset + startOffset, this.waveData.duration);
				}
			},
			stop: function() {
				if (!this.playing) {
					return;
				}
				if (this.paused || this.releasing) {
					// ポーズ中、リリース中なら即時停止
					this.stopNode(false);
					this.paused = false;
				} else {
					// リリースを行う
					this.stopNode(true);
				}
			},
			// リリース有りの停止
			stopNode: function(shouldRelease) {
				// 停止時間を計算
				var time = CriNc.context.currentTime;
				this.level.gain.cancelScheduledValues(0);
				this.level.gain.setValueAtTime(this.getEnvLevel(), time);
				this.envTime = time;
				if (shouldRelease && this.envActive) {
					time += this.envRel;
				} else {
					time += 0.02;
				}
				this.level.gain.linearRampToValueAtTime(0, time);
				// 停止リクエスト
				this.source.stop(time);
				this.stopTime = time;
				this.releasing = true;
			},
			update: function() {
				// 時刻の更新
				var currentTime = CriNc.context.currentTime;
				if (this.playing && !this.paused) {
					var elapsedTime = currentTime - this.lastTime;
					if (elapsedTime >= 0) {
						this.samples += elapsedTime * this.waveData.originalSampleRate * 
							+this.rate / this.waveData.originalSampleRate;
					}
				}

				this.lastTime = currentTime;
				// 停止時間を過ぎていたら停止
				if (this.releasing && currentTime >= this.stopTime) {
					this.cleanup();
				}
			},
			getEnvLevel: function() {
				// エンベロープタイムを計算
				var time = CriNc.context.currentTime - this.envTime;
				if (this.paused) {
					return 0;
				}
				if (this.releasing) {
					// リリース中のレベルを計算
					if (time < this.envRel) return this.envSus * (1.0 - time / this.envRel);
					return 0;
				} else {
					// アタック,ホールド,ディケイ中のレベルを計算
					if (time < this.envAtk) return time / this.envAtk;
					time -= this.envAtk;
					if (time < this.envHld) return 1.0;
					time -= this.envHld;
					if (time < this.envDcy) return 1.0 - (1.0 - this.envSus) * time / this.envDcy;
					return this.envSus;
				}	
			},
			pause: function(paused) {
				// 再生中でない時、ポーズ状態が変更されない時は抜ける
				if (!this.playing || paused == this.paused) {
					return;
				}
				// リリース中のポーズは停止
				if (this.releasing) {
					this.stopNode(false);
					return;
				}
				this.paused = paused;

				// 短い時間でレベルをフェードイン/アウトさせてプチノイズを回避
				var levelNode = this.level.gain;
				var beginLevel, endLevel;
				if (paused) {
					// レベルを下げる
					beginLevel = this.envSus;
					endLevel = 0;
				} else {
					// レベルを戻す
					beginLevel = 0;
					endLevel = this.envSus;
				}
				var time = CriNc.context.currentTime;
				levelNode.cancelScheduledValues(0);
				levelNode.setValueAtTime(beginLevel, time);
				levelNode.linearRampToValueAtTime(endLevel, time + 0.02);
				
				// 再生レートをゼロにしてポーズを実現するので更新をかける
				this.updatePlaybackRate();
			},
			getTime: function(count, tunit) {
				// ポインタに現在時刻を返す
				if (this.waveData) {
					setValue(count, this.samples, "i64");
					setValue(tunit, this.waveData.originalSampleRate, "i32");
				}
			},
			setOutputMatrix: function(nch, nspk, matrix) {
				var shouldCancelParam = (CriNc.browser == "edge");
				var gains = this.matrix.gains;
				nch  = Math.min(nch, gains.length / 2)|0;
				nspk = Math.min(nspk, 2)|0;
				var time = CriNc.context.currentTime + 0.01;
				for (var i = 0; i < nch; i++) {
					// メモリからポインタを取得
					var ptr = HEAPU32[(matrix>>2) + i];
					for (var j = 0; j < nspk; j++) {
						// メモリからレベルを取得
						var level = HEAPF32[(ptr>>2) + j];
						// 設定対象のゲインノードを取得
						var node = gains[i * 2 + j].gain;
						// 前回設定したレベルを取得
						if (node.lastValue === undefined) {
							// 初回はそのレベルを設定する
							node.lastValue = level;
							node.value = level;
						}
						if (shouldCancelParam) {
							// Edgeで処理が重くなるのを防ぐため、毎回AudioParamをリセットして再設定する
							node.cancelScheduledValues(0);
						}
						// 開始レベルをセット
						node.setValueAtTime(node.lastValue, time);
						// 終了レベルをセット
						node.linearRampToValueAtTime(level, time + 0.01);
						// 最後に設定した値として保存
						node.lastValue = level;
					}
				}
			},
			// プリディレイの設定
			setPreDelay: function(time) {
				this.predelay = time * 0.001;
			},
			// エンベロープのスイッチ
			setEnvActive: function(active) {
				this.envActive = active;
				// リリース中のエンベロープOFFは停止
				if (!active && this.releasing) {
					this.stopNode(false);
				}
			},
			// エンベロープパラメータの設定
			setEnvParam: function(paramId, value) {
				switch (paramId) {
				case 0: this.envAtk = value * 0.001; break;
				case 1: this.envHld = value * 0.001; break;
				case 2: this.envDcy = value * 0.001; break;
				case 3: this.envRel = value * 0.001; break;
				case 4: this.envSus = value; break;
				}
			},
			// バイクアッドフィルタのスイッチ
			setBiqActive: function(active) {
				this.biqEnabled = !!active;
				this.updateFilters();
			},
			// バイクアッドフィルタのタイプ設定
			setBiqType: function(value) {
				this.biq.type = CriNc.bqfTypeTable[value];
			},
			// バイクアッドフィルタの周波数設定
			setBiqFreq: function(value) {
				var maxFreq = CriNc.context.sampleRate / 2;
				var cof = Math.min(Math.max(value, 20), maxFreq);
				this.biq.frequency.value = cof;
			},
			// バイクアッドフィルタのQ値設定
			setBiqQ: function(value) {
				this.biq.Q.value = value;
			},
			// バイクアッドフィルタのゲイン設定
			setBiqGain: function(value) {
				this.biq.gain.value = value;
			},
			// バイクアッドフィルタの更新
			updateBiq: function() {
				// 特に何もやらない
			},
			// バンドパスフィルタのスイッチ
			setBpfActive: function(active) {
				this.bpfEnabled = !!active;
				this.updateFilters();
			},
			// バンドパスフィルタの低域カットオフ周波数の設定
			setBpfCofLo: function(value) {
				this.bpfCofLo = value;
			},
			// バンドパスフィルタの高域カットオフ周波数の設定
			setBpfCofHi: function(value) {
				this.bpfCofHi = value;
			},
			// バンドパスフィルタの更新
			updateBpf: function() {
				CriNc.updateBpf(this.bpf, this.bpfCofLo, this.bpfCofHi);
			},
			// DSP関連(バイクワッド、バンドパス、エンベロープ、プリディレイ)のリセット
			resetDspParams: function() {
				// バイクワッドフィルタを初期化
				this.biqEnabled = false;
				this.biq.type = "highpass";
				this.biq.frequency.value = 0;
				this.biq.Q.value = 1.0;
				this.biq.gain.value = 1.0;

				// バンドパスフィルタを初期化
				this.bpfEnabled = false;
				this.bpfCofLo = 0;
				this.bpfCofHi = 24000;
				this.updateFilters();

				// エンベロープを初期化
				this.envActive = false;
				this.envAtk = 0;
				this.envHld = 0;
				this.envDcy = 0;
				this.envRel = 0;
				this.envSus = 1.0;

				// プリディレイを初期化
				this.predelay = 0;
			},
			// フィルタ関連の更新
			updateFilters: function() {
				this.level.disconnect();
				this.biq.disconnect();
				this.bpf.disconnect();

				var node = this.level;
				if (this.biqEnabled) {
					node.connect(this.biq);
					node = this.biq;
				}
				if (this.bpfEnabled) {
					node.connect(this.bpf);
					node = this.bpf;
				}
				node.connect(this.matrix.split);
			},
			setRouting: function(busId, level) {
				this.router.set(busId, level, this.matrix.merge);
			},
			resetRouting: function() {
				this.router.reset();
				this.matrix.merge.disconnect();
				this.router.set(0, 1.0, this.matrix.merge);
			},
		},
		// バス
		/** @constructor */
		CriNc.Bus = function(context) {
			this.fx = new Array(8);
			for (var i = 0; i < 8; i++) {
				this.fx[i] = null;
			}
			this.input = context.createGain();
			this.level = context.createGain();
			this.router = new CriNc.Router(context, 8);
			this.update();
		};
		// バスの関数
		CriNc.Bus.prototype = {
			// DSPを空きスロットに追加
			attachFx: function(fx) {
				var id = this.fx.indexOf(null);
				if (id >= 0) {
					this.fx[id] = fx;
				}
			},
			// 全てのDSPを破棄
			detachAllFx: function() {
				for (var i = 0; i < this.fx.length; i++) {
					if (this.fx[i]) {
						this.fx[i] = null;
					}
				}
				this.update();
			},
			// DSPFxをfxidで探す
			findFx: function(fxId) {
				for (var i = 0; i < this.fx.length; i++) {
					var fx = this.fx[i];
					if (fx && fx.id == fxId) {
						return fx;
					}
				}
			},
			// ノードの接続関係を更新する
			update: function() {
				var node = this.input;
				node.disconnect();
				for (var i = 0; i < this.fx.length; i++) {
					var fx = this.fx[i];
					if (fx) {
						node.connect(fx.input);
						node = fx.output;
					}
				}
				// 最終的にレベルノードに接続
				node.connect(this.level);
			},
			setRouting: function(busId, level) {
				this.router.set(busId, level, this.level);
			},
			resetRouting: function() {
				this.router.reset();
				this.level.disconnect();
			},
		},
		// パンニング用マトリクスゲイン
		/** @constructor */
		CriNc.Matrix = function(context, inCh, outCh) {
			this.split = context.createChannelSplitter(inCh);
			this.merge = context.createChannelMerger(outCh);
			this.gains = new Array(inCh * outCh);
			for (var i = 0; i < inCh; i++) {
				for (var j = 0; j < outCh; j++) {
					var gain = context.createGain();
					this.split.connect(gain, i, 0);
					gain.connect(this.merge, 0, j);
					this.gains[i * outCh + j] = gain;
				}
			}
		};
		CriNc.Matrix.prototype = {
			reset: function() {
				for (var i = 0; i < this.gains.length; i++) {
					var node = this.gains[i].gain;
					node.cancelScheduledValues(0);
					node.lastValue = undefined;
					node.value = 0;
				}
			}
		};
		// バスルーティング用ゲイン
		/** @constructor */
		CriNc.Router = function(context, count) {
			this.levels = new Array(count);
			this.targets = new Array(count);
			// GainNodeは1つはあらかじめ作っておく
			this.levels[0] = context.createGain();
			this.reset();
		};
		CriNc.Router.prototype = {
			reset: function() {
				for (var i = 0; i < this.targets.length; i++) {
					if (this.levels[i]) {
						this.levels[i].disconnect();
					}
					this.targets[i] = -1;
				}
				for (var i = 1; i < this.levels.length; i++) {
					this.levels[i] = null;
				}
			},
			set: function(target, level, source) {
				// センド先が登録されているか探す
				var index = this.targets.indexOf(target);
				if (index == -1) {
					// 見つからなければ接続する
					if (level <= 0) {
						return;
					}
					index = this.targets.indexOf(-1);
					if (index < 0) {
						return;
					}
					// GainNodeが無ければ作成する
					if (this.levels[index] == null) {
						this.levels[index] = CriNc.context.createGain();
					}
					this.targets[index] = target;
					if (target < CriNc.buses.length) {
						// バスへ接続
						this.levels[index].connect(CriNc.buses[target].input);
					}
					// バスセンドのノードにソースを接続
					source.connect(this.levels[index]);
				}

				// レベル調整
				var node = this.levels[index];
				node.gain.cancelScheduledValues(0);
				node.gain.setValueAtTime(level, CriNc.context.currentTime);
			},
		};

		CriNc.genIR = function(dspId, numChannels, numSamples, params) {
			var context = CriNc.context;
			var outMem = _malloc(8 + (numSamples + 256) * 4 * numChannels);
			for (var i = 0; i < numChannels; i++) {
				HEAP32[(outMem>>2) + i] = (outMem + 8) + i * numSamples * 4;
			}
			
			// パラメータ用メモリを確保する
			var paramsMem = _malloc(params.length * 4);

			// パラメータのセット
			for (var i = 0; i < params.length; i++) {
				HEAPF32[(paramsMem>>2) + i] = params[i];
			}

			// DSPを実行してIRを取得する
			Runtime.dynCall("iiiiiiii", CriNc.dspFunc, 
				[dspId, outMem, numChannels, numSamples, context.sampleRate, paramsMem, params.length]);
			
			// パラメータ用メモリを解放する
			_free(paramsMem);
			
			//var data = new Array(numChannels);
			var buffer = context.createBuffer(numChannels, numSamples, context.sampleRate);
			for (var i = 0; i < numChannels; i++) {
				var offset = HEAP32[(outMem>>2) + i] >> 2;
				var data = HEAPF32.subarray(offset, offset + numSamples);
				buffer.getChannelData(i).set(data);
			}
			_free(outMem);
			return buffer;
		};

		// リバーブDSP
		/** @constructor */
		CriNc.FxReverb = function(context) {
			this.id = 0;

			this.verb = context.createConvolver();
			this.input = this.verb;
			this.output = this.verb;

			this.params = [
				3000,	// reverbTime
				10,		// roomSize
				100,	// predelayTime
				0,		// cofLow
				8000,	// cofHigh
			];
		};
		CriNc.FxReverb.prototype = {
			setParam: function(paramId, value) {
				this.params[paramId] = value;
			},
			update: function() {
				var params = this.params;
				var numSamples = (params[0] * CriNc.context.sampleRate / 1000)|0;
				this.verb.buffer = CriNc.genIR(0, 2, numSamples, params);
			},
			destroy: function() {
				this.verb.disconnect();
			},
		};
		// I3DL2リバーブDSP
		/** @constructor */
		CriNc.FxI3DL2Reverb = function(context) {
			this.id = 2;

			this.verb = context.createConvolver();
			this.input = this.verb;
			this.output = this.verb;

			this.params = [
				-1000.0,	// room 
				-100.0,		// roomHF 
				1.49,		// decayTime 
				0.83,		// decayHFRatio 
				-2602.0,	// reflections 
				0.007,		// reflectionsDelay 
				200.0,		// reverb 
				0.011,		// reverbDelay 
				100.00,		// diffusion 
				100.00,		// density 
				5000.0,		// hfReference 
			];
		};
		CriNc.FxI3DL2Reverb.prototype = {
			setParam: function(paramId, value) {
				this.params[paramId] = value;
			},
			update: function() {
				var params = this.params;
				var numSamples = ((params[5] + params[7] + params[2]) * CriNc.context.sampleRate / 1000)|0;
				this.verb.buffer = CriNc.genIR(1, 2, numSamples, params);
			},
			destroy: function() {
				this.verb.disconnect();
			},
		};

		// エコーDSP
		/** @constructor */
		CriNc.FxEcho = function(context) {
			this.id = 1;

			this.delay = context.createDelay(1);
			this.feedback = context.createGain();
			this.delay.connect(this.feedback);
			this.feedback.connect(this.delay);
			this.input = this.delay;
			this.output = this.delay;

			this.params = {
				delayTime: 300,
				feedback: 0.5,
			};
		};
		CriNc.FxEcho.prototype = {
			setParam: function(paramId, value) {
				switch (paramId) {
				case 0:this.params.delayTime = value; break;
				case 1:this.params.feedback = value; break;
				}
			},
			update: function() {
				this.delay.delayTime.value = Math.min(Math.max(this.params.delayTime * 0.001, 0.1), 1);
				this.feedback.gain.value = this.params.feedback;
			},
			destroy: function() {
				this.delay.disconnect();
				this.feedback.disconnect();
			},
		};

		// ディレイDSP
		/** @constructor */
		CriNc.FxDelay = function(context) {
			this.id = 3;

			this.delay = context.createDelay(1);
			this.input = this.delay;
			this.output = this.delay;

			this.params = {
				delayTime: 300,
			};
		};
		CriNc.FxDelay.prototype = {
			setParam: function(paramId, value) {
				switch (paramId) {
				case 0:this.params.delayTime = value; break;
				}
			},
			update: function() {
				this.delay.delayTime.value = Math.min(Math.max(this.params.delayTime * 0.001, 0.1), 1);
			},
			destroy: function() {
				this.delay.disconnect();
			},
		};

		// バンドパスフィルタDSP
		/** @constructor */
		CriNc.FxBandpass = function(context) {
			this.id = 4;

			this.filter = context.createBiquadFilter();
			this.input = this.filter;
			this.output = this.filter;

			this.params = {
				cofLow: 0,
				cofHigh: 24000,
			};
		};
		CriNc.FxBandpass.prototype = {
			setParam: function(paramId, value) {
				switch (paramId) {
				case 0:this.params.cofLow = value; break;
				case 1:this.params.cofHigh = value; break;
				}
			},
			update: function() {
				CriNc.updateBpf(this.filter, this.params.cofLow, this.params.cofHigh);
			},
			destroy: function() {
				this.filter.disconnect();
			},
		};

		// バイクワッドフィルタDSP
		/** @constructor */
		CriNc.FxBiquad = function(context) {
			this.id = 5;

			this.filter = context.createBiquadFilter();
			this.input = this.filter;
			this.output = this.filter;
			
			this.params = {
				type: 0,
				freq: 8000,
				q: 1.0,
				gain: 1.0,
			};
		};
		CriNc.FxBiquad.prototype = {
			setParam: function(paramId, value) {
				switch (paramId) {
				case 0:this.params.type = value; break;
				case 1:this.params.freq = value; break;
				case 2:this.params.q = value; break;
				case 3:this.params.gain = value; break;
				}
			},
			update: function() {
				this.filter.type = CriNc.bqfTypeTable[this.params.type];
				this.filter.frequency.value = this.params.freq;
				this.filter.Q.value = this.params.q;
				this.filter.gain.value = this.params.gain;
			},
			destroy: function() {
				this.filter.disconnect();
			},
		};

		// 3バンドEQ DSP
		/** @constructor */
		CriNc.Fx3BandEq = function(context) {
			this.id = 6;

			this.eq = [];
			for (var i = 0; i < 3; i++) {
				this.eq.push(context.createBiquadFilter());
			}
			for (var i = 0; i < this.eq.length - 1; i++) {
				this.eq[i].connect(this.eq[i + 1]);
			}
			this.input = this.eq[0];
			this.output = this.eq[this.eq.length - 1];
			
			this.params = {
				band: 0,
				type: 0,
				freq: 0.5,
				q: 1.0,
				gain: 1.0,
			};
		};
		CriNc.Fx3BandEq.prototype = {
			setParam: function(paramId, value) {
				switch (paramId) {
				case 0:this.params.band = value; break;
				case 1:this.params.type = value; break;
				case 2:this.params.freq = value; break;
				case 3:this.params.q = value; break;
				case 4:this.params.gain = value; break;
				}
			},
			update: function() {
				var eq = this.eq[this.params.band];
				eq.type = CriNc.bqfTypeTable[this.params.type];
				eq.frequency.value = this.params.freq;
				eq.Q.value = this.params.q;
				eq.gain.value = this.params.gain;
			},
			destroy: function() {
				for (var i = 0; i < this.eq.length; i++) {
					this.eq[i].disconnect();
				}
			},
		};

		/** @constructor */
		CriNc.WaveData = function() {
			this.id = 0;
			this.buffer = null;
			this.refcnt = 1;
			this.error = false;
			this.offset = 0;
			this.duration = 0;
			this.loopStart = 0;
			this.loopEnd = 0;
		};
	},
	wakeupMobileAudio: function(e) {
		if (!CriNc.hasAudioAwoken) {
			CriNc.context.createBufferSource().start(0);
			CriNc.hasAudioAwoken = true;
		}
		window.removeEventListener("touchstart", CriNc.wakeupMobileAudio);
	},
},
WAJS_Initialize: function(dspFunc) {
	CriNc.defineClasses();
	CriNc.dspFunc = dspFunc;
	
	var ua = navigator.userAgent.toLowerCase();
	// ブラウザ判定 ---------------------------------
	if (ua.match(/msie|trident/)) {
		CriNc.browser = 'msie';
	} else if (ua.match(/edge/)) {
		CriNc.browser = 'edge';
	} else if (ua.match(/vivaldi/)) {
		CriNc.browser = 'vivaldi';
	} else if (ua.match(/opera/)) {
		CriNc.browser = 'opera';
	} else if (ua.match(/chrome/)) {
		CriNc.browser = 'chrome';
	} else if (ua.match(/firefox/)) {
		CriNc.browser = 'firefox';
	} else if (ua.match(/safari/)) {
		CriNc.browser = 'safari';
	} else {
		CriNc.browser = 'n/a';
	}

	// OS判定 ---------------------------------
	if (ua.match(/iphone|ipad/)) {
		CriNc.os = "iOS";
		CriNc.isMobile = true;
	} else if (ua.match(/android/)) {
		CriNc.os = "android";
		CriNc.isMobile = true;
	} else if (ua.match(/win/)) {
		CriNc.os = "windows";
	} else if (ua.match(/mac/)) {
		CriNc.os = "macOS";
	} else if (ua.match(/linux/)) {
		CriNc.os = "linux";
	} else {
		CriNc.os = "n/a";
	}

	//console.log(CriNc.os + ", " + CriNc.browser);
	
	// AudioContextを作成
	var AudioContext = window.AudioContext || window.webkitAudioContext;
	if (!AudioContext) {
		console.warn("Web Audio API is not supported.");
		return;
	}
	CriNc.context = CriNc.context || new AudioContext();
	
	// Safe initialization for Mobile Safari
	// Safariのバグでサンプリング周波数が48000の場合、ノイズまみれになる現象を回避
	if (CriNc.os == "iOS") {
		var dummy = CriNc.context.createBufferSource()
		dummy.buffer = CriNc.context.createBuffer(1, 1, 44100);
		dummy.connect(CriNc.context.destination)
		dummy.start(0);
		dummy.disconnect();
		// Contextを再作成する
		CriNc.context = new AudioContext();
	}

	// バスを8個作成する
	for (var i = 0; i < 8; i++) {
		CriNc.buses.push(new CriNc.Bus(CriNc.context));
	}
	// マスターバスの設定
	var masterBus = CriNc.buses[0];
	masterBus.level.connect(CriNc.context.destination);

	// モバイル環境ではデフォルトでは音が出ないのでタッチイベントを拾って1音再生する
	if (CriNc.isMobile) {
		window.addEventListener("touchstart", CriNc.wakeupMobileAudio);
	}
},
WAJS_Finalize: function() {
	if (!CriNc.context) return;
	// 終了処理
	CriNc.voices = [{}];
	CriNc.buffers = {};
	CriNc.buses = [];

	// Edgeがclose()に対応していないため、AudioContextを破棄しないで持っておく
	//CriNc.context.close();
	//CriNc.context = null;
},
WAJS_SuspendContext: function() {
	if (!CriNc.context || !CriNc.context.suspend) return false;
	CriNc.context.suspend();
	return true;
},
WAJS_ResumeContext: function() {
	if (!CriNc.context || !CriNc.context.resume)  return false;
	CriNc.context.resume();
	return true;
},
WAJS_AttachDspFx: function(busId, fxId) {
	if (!CriNc.context) return;
	//console.log("WAJS_AttachDspFx", busId, fxId);
	var fx = null;
	switch (fxId) {
	case  0: fx = new CriNc.FxReverb(CriNc.context); break;
	case  1: fx = new CriNc.FxEcho(CriNc.context); break;
	case  2: fx = new CriNc.FxI3DL2Reverb(CriNc.context); break;
	case  3: fx = new CriNc.FxDelay(CriNc.context); break;
	case  4: fx = new CriNc.FxBandpass(CriNc.context); break;
	case  5: fx = new CriNc.FxBiquad(CriNc.context); break;
	case  6: fx = new CriNc.Fx3BandEq(CriNc.context); break;
	}
	CriNc.buses[busId].attachFx(fx);
},
WAJS_ResetDspBus: function(busId) {
	if (!CriNc.context) return;
	var bus = CriNc.buses[busId];
	bus.detachAllFx();
	bus.resetRouting();
	if (busId == 0) {
		// マスターバスの設定
		var masterBus = CriNc.buses[0];
		masterBus.level.connect(CriNc.context.destination);
	}
},
WAJS_UpdateDspBus: function(busId) {
	if (!CriNc.context) return;
	var bus = CriNc.buses[busId];
	bus.update();
},
WAJS_SetDspBusVolume: function(busId, volume) {
	if (!CriNc.context) return;
	var bus = CriNc.buses[busId];
	bus.level.gain.value = volume;
},
WAJS_SetDspBusSendLevel: function(busId, target, level) {
	if (!CriNc.context) return;
	var bus = CriNc.buses[busId];
	bus.setRouting(target, level);
},
WAJS_SetDspFxParam: function(busId, fxId, paramId, value) {
	if (!CriNc.context) return;
	var bus = CriNc.buses[busId];
	var fx = bus.findFx(fxId);
	if (fx) {
		fx.setParam(paramId, value);
	}
},
WAJS_UpdateDspFx: function(busId, fxId) {
	if (!CriNc.context) return;
	var bus = CriNc.buses[busId];
	var fx = bus.findFx(fxId);
	if (fx) {
		fx.update();
	}
},
WAJS_SetIntervalEvent: function(callback, time) {
	setInterval(function() {
		Runtime.dynCall("v", callback, []);
	}, time);
},
WAJS_CreateBank: function(numContents) {
	var bankId = ++CriNc.bankCount;
	CriNc.banks[bankId] = [];
	return bankId;
},
WAJS_DestroyBank: function(bandId) {
	var buffersInBank = CriNc.banks[bandId];
	for (var i = 0; i < buffersInBank.length; i++) {
		delete CriNc.buffers[buffersInBank[i].id];
	}
	delete CriNc.banks[bandId];
},
WAJS_LoadData: function(bandId, ptr, size, 
	originalSampleRate, originalSamples, encoderDelay, 
	loopStart, loopEnd, cbfunc, cbobj) {

	if (!CriNc.context) {
		return 0;
	}
	//console.log("WAJS_LoadData: ", ptr);

	// 一旦ヒープメモリからArrayBufferコピーする必要がある
	var data = HEAPU8.buffer.slice(ptr, ptr + size);

	// WaveDataを作成
	var waveData = new CriNc.WaveData();
	waveData.originalSampleRate = originalSampleRate;
	waveData.originalSamples = originalSamples;

	// ポインタアドレスをID兼キーとする
	var bufferId = ptr;
	waveData.id = bufferId;
	CriNc.buffers[bufferId] = waveData;

	if (bandId > 0) {
		CriNc.banks[bandId].push(waveData);
	}

	// デコードする
	CriNc.context.decodeAudioData(data, function(buffer) {
		waveData.buffer = buffer;
		var numSamples;
		if (originalSamples > 0) {
			if (CriNc.browser == "safari") {
				// 既にディレイ分を全てカットされている
				encoderDelay = 0;
			}

			if (CriNc.browser == "firefox" && CriNc.os == "macOS") {
				if (encoderDelay == 5186) {
					// HE-AACは既にディレイ分を962だけカットされている
					encoderDelay = 962;
				}
			}

			// オフセット計算
			waveData.offset = +encoderDelay / originalSampleRate;

			// ループ設定
			if (loopStart < loopEnd) {
				waveData.loopStart = +(encoderDelay + loopStart) / originalSampleRate;
				waveData.loopEnd = +(encoderDelay + loopEnd) / originalSampleRate;
			}
			// 音声の長さの計算
			waveData.duration = +originalSamples / originalSampleRate;
			numSamples = originalSamples * buffer.sampleRate / originalSampleRate;
		} else {
			waveData.offset = 0;
			waveData.duration = buffer.duration;
			numSamples = buffer.length;
		}
		//console.log(buffer);
		if (cbfunc) {
			Runtime.dynCall("vii", cbfunc, [cbobj, 1]);
		}
	}, function() {
		waveData.error = true;
		if (cbfunc) {
			Runtime.dynCall("vii", cbfunc, [cbobj, 0]);
		}
	});
	return bufferId;
},
WAJS_ReleaseData: function(bufferId) {
	//console.log("WAJS_ReleaseData: ", bufferId);
	var waveData = CriNc.buffers[bufferId];
	if (waveData) {
		if (--waveData.refcnt <= 0) {
			delete CriNc.buffers[bufferId];
		}
	}
},
WAJS_GetBufferId: function(ptr, retain) {
	var waveData = CriNc.buffers[ptr];
	if (waveData) {
		if (retain) {
			waveData.refcnt++;
		}
		var bufferId = ptr;
		return bufferId;
	}
	return 0;
},
WAJS_GetBufferStatus: function(bufferId) {
	var waveData = CriNc.buffers[bufferId];
	if (waveData) {
		if (waveData.buffer) {
			return 1;		// Complete
		} else if (waveData.error) {
			return 2;		// Error
		} else {
			return 0;		// Loading
		}
	}
	return -1;
},
WAJS_GetBufferFormat: function(bufferId, srate, nch, nsmpl) {
	var waveData = CriNc.buffers[bufferId];
	if (waveData && waveData.buffer) {
		HEAP32[srate >> 2] = waveData.originalSampleRate;
		HEAP32[nch >> 2] = waveData.buffer.numberOfChannels;
		HEAP32[nsmpl >> 2] = waveData.originalSamples;
		return true;
	}
	return false;
},
WAJS_CreateVoice: function(maxChannels) {
	var voice = null;
	if (CriNc.context) {
		voice = new CriNc.Voice(CriNc.context, maxChannels, 2);
	}
	// 空きスロットを探す
	var id = CriNc.voices.indexOf(null);
	if (id >= 0) {
		// 開いていたらそこにセットする
		CriNc.voices[id] = voice;
	} else {
		// 開いてなければ配列に追加する
		id = CriNc.voices.length;
		CriNc.voices.push(voice);
	}
	return id;
},
WAJS_DestroyVoice: function(id) {
	CriNc.voices[id] = null;
},
WAJS_SetDecoder: function(id, decoder) {
	if (!CriNc.context) return;
	CriNc.voices[id].decoder = decoder;
},
WAJS_GetDecoder: function(id, decoder) {
	if (!CriNc.context) return;
	return CriNc.voices[id].decoder;
},
WAJS_SetStartTime: function(id, startTime) {
	if (!CriNc.context) return;
	CriNc.voices[id].setStartOffset(startTime);
},
WAJS_SetData: function(id, bufferId) {
	if (!CriNc.context) return;
	CriNc.voices[id].setData(CriNc.buffers[bufferId]);
},
WAJS_SetSamplingRate: function(id, rate) {
	if (!CriNc.context) return;
	CriNc.voices[id].setSamplingRate(rate);
},
WAJS_Start: function(id) {
	if (!CriNc.context) return;
	CriNc.voices[id].start();
},
WAJS_Stop: function(id) {
	if (!CriNc.context) return;
	CriNc.voices[id].stop();
},
WAJS_IsPlaying: function(id) {
	if (!CriNc.context) return;
	return (CriNc.voices[id].playing) ? 1 : 0;
},
WAJS_Update: function(id) {
	if (!CriNc.context) return;
	CriNc.voices[id].update();
},
WAJS_GetEnvelopeLevel: function(id) {
	if (!CriNc.context) return;
	CriNc.voices[id].pause(paused);
},
WAJS_Pause: function(id, paused) {
	if (!CriNc.context) return;
	CriNc.voices[id].pause(paused);
},
WAJS_GetTime: function(id, count, tunit) {
	if (!CriNc.context) return;
	CriNc.voices[id].getTime(count, tunit);
},
WAJS_SetOutputMatrix: function(id, nch, nspk, matrix) {
	if (!CriNc.context) return;
	CriNc.voices[id].setOutputMatrix(nch, nspk, matrix);
},
WAJS_SetPreDelay: function(id, time) {
	if (!CriNc.context) return;
	CriNc.voices[id].setPreDelay(time);
},
WAJS_SetEnvelopeActive: function(id, active) {
	if (!CriNc.context) return;
	CriNc.voices[id].setEnvActive(active);
},
WAJS_SetEnvelopeParam: function(id, paramId, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setEnvParam(paramId, value);
},
WAJS_SetBiquadActive: function(id, active) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBiqActive(active);
},
WAJS_SetBiquadType: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBiqType(value);
},
WAJS_SetBiquadFreq: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBiqFreq(value);
},
WAJS_SetBiquadQ: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBiqQ(value);
},
WAJS_SetBiquadGain: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBiqGain(value);
},
WAJS_UpdateBiquad: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].updateBiq(value);
},
WAJS_SetBandpassActive: function(id, active) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBpfActive(active);
},
WAJS_SetBandpassCofLo: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBpfCofLo(value);
},
WAJS_SetBandpassCofHi: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].setBpfCofHi(value);
},
WAJS_UpdateBandpass: function(id, value) {
	if (!CriNc.context) return;
	CriNc.voices[id].updateBpf(value);
},
WAJS_ResetDspParameters: function(id) {
	if (!CriNc.context) return;
	CriNc.voices[id].resetDspParams();
},
WAJS_SetRouting: function(id, busId, level) {
	if (!CriNc.context) return;
	CriNc.voices[id].setRouting(busId, level);
},
WAJS_ResetRouting: function(id) {
	if (!CriNc.context) return;
	CriNc.voices[id].resetRouting();
},
};

autoAddDeps(LibraryCriNc, '$CriNc');
mergeInto(LibraryManager.library, LibraryCriNc);
