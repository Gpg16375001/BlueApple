LibraryCriMana = {

$CriMana: {
	defineClasses: function() {
	"use strict";

	var cri = cri || {};
	var WORKER_VERSION = 6;
	var idcount = 1;
	var players = {};
	var worker = new Worker("StreamingAssets/sofdec2.worker.js?version=" + WORKER_VERSION);
	worker.onmessage = function(e) {
		var res = e.data;

		var player = players[res.id];
		if (player == null) {
			return;
		}

		if (res.type == "movieframe") {
			if (player.movieInfo) {
				player.frames.push(res);
				player.status = res.status;
			}
		} else if (res.type == "movieinfo") {
			player.movieInfo = res.movieInfo;
			player.status = res.status;
		} else if (res.type == "status") {
			player.status = res.status;
			if (player.status == 0) {
				player.frames.length = 0;
				player.numEntries = 0;
				player.prepared = false;
			}
		} else if (res.type == "ondecreaseentry") {
			if (player.numEntries > 0) {
				player.numEntries -= res.count;
			}
		}
	};
	
	/**
	 * @name cri.ManaPlayer
	 * @class
	 * 動画再生を管理するプレーヤです。<br>
	 * {@link cri.ManaPlayer.create} で作成します。
	 */
	//[ts] export class ManaPlayer {
	(function(){
		cri.ManaPlayer = function() {
			this.id = idcount++;
			this.prepared = false;
			this.frames = [];
			this.movieInfo = null;
			this.status = 0;
			this.time = 0;
			this.numEntries = 0;

			players[this.id] = this;
			worker.postMessage({
				id:this.id, type:"create"
			});
		};
		cri.ManaPlayer.prototype = {
			/**
			 * プレーヤの破棄<br>
			 * 参考：criManaPlayer_Destroy
			 */
			//[ts] public destroy(): void;
			destroy: function() {
				worker.postMessage({
					id:this.id, type:"destroy"
				});
				delete players[this.id];
			},
			/**
			 * ファイルのセット<br>
			 * 参考：criManaPlayer_SetFile
			 * @param {string} path ファイル(URL)
			 */
			//[ts] public setFile(path: string): void;
			setFile: function(path) {
				if (typeof path != "string") {
					console.error("Invalid argument in setFile()");
					return;
				}
				worker.postMessage({
					id:this.id, type:"setfile", path:path
				});
			},
			/**
			 * ファイル範囲のセット<br>
			 * 参考：criManaPlayer_SetFileRange
			 * @param {string} path ファイル(URL)
			 * @param {number} offset オフセット(バイト)
			 * @param {number} size サイズ(バイト)
			 */
			//[ts] public setFileRange(path: string, offset: number, size: number): void;
			setFileRange: function(path, offset, size) {
				if (typeof path != "string") {
					console.error("Invalid argument in setFileRange()");
					return;
				}
				worker.postMessage({
					id:this.id, type:"setfilerange", path:path, offset:offset, size:size
				});
			},
			/**
			 * ArrayBufferのセット<br>
			 * @param {string} buffer ArrayBuffer
			 */
			//[ts] public setData(buffer: ArrayBuffer): void;
			setData: function(buffer) {
				worker.postMessage({
					id:this.id, type:"setdata", buffer:buffer
				}, [buffer]);
			},
			/**
			 * 再生の準備<br>
			 * 参考：criManaPlayer_Prepare
			 * @return {cri.AtomExPlayback} 再生インスタンス
			 */
			//[ts] public prepare(): cri.AtomExPlayback;
			prepare: function() {
				if (this.status != 0 && this.status != 6) {
					return;
				}
				if (this.prepared) {
					return;
				}
				this.status = 1;
				this.prepared = true;
				worker.postMessage({
					id:this.id, type:"prepare",
				});
			},
			/**
			 * 再生の開始<br>
			 * 参考：criManaPlayer_Start
			 * @return {cri.AtomExPlayback} 再生インスタンス
			 */
			//[ts] public start(): cri.AtomExPlayback;
			start: function() {
				if (!this.prepared) {
					if (this.status != 0 && this.status != 6) {
						return;
					}
					this.status = 1;
				}
				this.prepared = false;
				worker.postMessage({
					id:this.id, type:"start",
				});
			},
			/**
			 * 再生の停止<br>
			 * 参考：criManaPlayer_Stop
			 */
			//[ts] public stop(): void;
			stop: function() {
				this.movieInfo = null;
				this.frames.length = 0;
				worker.postMessage({
					id:this.id, type:"stop",
				});
			},
			/**
			 * ポーズ／ポーズ解除<br>
			 * 参考：criManaPlayer_Pause
			 * @param {boolean} sw スイッチ（false：ポーズ解除、true：ポーズ）
			 */
			//[ts] public pause(sw: boolean): void;
			pause: function(sw) {
				worker.postMessage({
					id:this.id, type:"pause", sw:sw
				});
				this.isPaused = sw;
			},
			/**
			 * 再生音の機能別のポーズ解除<br>
			 * 参考：criManaPlayer_Resume
			 * @param {cri.AtomExResumeMode} mode ポーズ解除対象
			 */
			//[ts] public resume(mode: cri.AtomExResumeMode): void;
			resume: function(mode) {
				this.pause(false);
			},
			/**
			 * ポーズ状態の取得
			 * 参考：criManaPlayer_IsPaused
			 * @return {boolean} ポーズ中かどうか（false：ポーズされていない、true：ポーズ中）
			 */
			//[ts] public isPaused(): boolean;
			isPaused: function() {
				return this.isPaused;
			},
			/**
			 * 再生時刻の取得<br>
			 * 参考：criManaPlayer_GetTime
			 * @return {number} 再生時刻（ミリ秒単位）
			 */
			//[ts] public getTime(): number;
			getTime: function() {
				return this.time;
			},
			/**
			 * ステータスの取得<br>
			 * 参考：criManaPlayer_GetStatus
			 * @return {cri.ManaPlayer.Status} ステータス
			 */
			//[ts] public getStatus(): cri.ManaPlayer.Status;
			getStatus: function() {
				return this.status;
			},

			setSeekPosition: function(seekpos) {
				worker.postMessage({
					id:this.id, type:"seek", seekpos:seekpos
				});
			},

			loop: function(sw) {
				worker.postMessage({
					id:this.id, type:"loop", sw:((sw) ? true : false)
				});
			},
			setSpeed: function(speed) {
				worker.postMessage({
					id:this.id, type:"setspeed", speed:speed
				});
			},
			entryFile: function(path, repeat) {
				this.numEntries++;
				worker.postMessage({
					id:this.id, type:"entryfile", 
					path:path, repeat:repeat, offset:0, size:0
				});
				return true;
			},
			entryFileRange: function(path, offset, size, repeat) {
				this.numEntries++;
				worker.postMessage({
					id:this.id, type:"entryfile", 
					path:path, repeat:repeat, offset:offset, size:size
				});
				return true;
			},
			entryData: function(buffer, repeat) {
				this.numEntries++;
				worker.postMessage({
					id:this.id, type:"entrydata", 
					buffer:buffer, repeat:repeat
				}, [buffer]);
				return true;
			},
			getNumEntry: function() {
				return this.numEntries;
			},
			clearEntry: function() {
				this.numEntries = 0;
				worker.postMessage({
					id:this.id, type:"clearentry",
				});
			},

			getMovieInfo: function() {
				return this.movieInfo;
			},
			referFrame: function() {
				if (this.frames.length == 0) {
					return null;
				}
				return this.frames[0];
			},
			discardFrame: function() {
				if (this.frames.length == 0) {
					return;
				}
				var frame = this.frames.pop();
				worker.postMessage({
					id:this.id, type:"returnbuffer", buffer:frame.buffer
				}, [frame.buffer]);
			},
			updateWebGLTexture: function() {
			},
		};
		cri.ManaPlayer.findInstance = function(id) {
			return players[id];
		};
		cri.ManaPlayer.getTotalNumEntries = function() {
			var total = 0;
			Object.keys(players).forEach(function(key) {
				total += players[key].numEntries;
			});
			return total;
		};
	})();
	//[ts] }

		// ManaPlayerクラス
		CriMana.Player = cri.ManaPlayer;

		// Mana初期化
		CriMana.initialize = function(decoders) {
			worker.postMessage({
				type: "initialize", 
				version: WORKER_VERSION,
				decoders: decoders,
			});
		};
		
		// Mana終了処理
		CriMana.finalize = function() {
			worker.postMessage({
				type: "finalize", 
			});
		};

		CriMana.onvisibilitychange = function() {
			if (document.hidden) {
				Object.keys(players).forEach(function(key) {
					var player = players[key];
					while (player.frames > 0) {
						player.discardFrame();
					}
				});
			}
			worker.postMessage({
				type: "onvisibilitychange", 
				hidden: document.hidden,
			});
		};
	},
	isLocalPath: function(path) {
		return path.indexOf("http://") < 0 && path.indexOf("https://") < 0;
	},
	loadFromFS: function(path, offset, size) {
		try {
			var stat = FS.stat(path);
			var stm = FS.open(path, "r");
			if (offset == 0 && size == 0) {
				size = stat.size;
			}
			var buffer = new ArrayBuffer(size);
			FS.read(stm, new Uint8Array(buffer), 0, size, offset);
			FS.close(stm);
			return buffer;
		} catch (e) {
			console.error(path + "' is Not found.");
		}
		return null;
	}
},
JSMP_Initialize: function(decoders) {
	if (CriMana.Player == null) {
		CriMana.defineClasses();
	}
	CriMana.initialize(decoders);
	document.addEventListener("visibilitychange", CriMana.onvisibilitychange);
},
JSMP_Finalize: function() {
	document.removeEventListener("visibilitychange", CriMana.onvisibilitychange);
	CriMana.finalize();
},
JSMP_GetTotalNumberOfEntry: function() {
	return CriMana.Player.getTotalNumEntries();
},
JSMP_CreatePlayer: function() {
	var player = new CriMana.Player();
	return player.id;
},
JSMP_DestroyPlayer: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.destroy();
},
JSMP_SetFile: function(id, pathptr) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	var path = Pointer_stringify(pathptr);
	if (CriMana.isLocalPath(path)) {
		var buffer = CriMana.loadFromFS(path, 0, 0);
		if (buffer) player.setData(buffer);
	} else {
		player.setFile(path);
	}
},
JSMP_SetFileRange: function(id, pathptr, offset, size) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	var path = Pointer_stringify(pathptr);
	if (CriMana.isLocalPath(path)) {
		var buffer = CriMana.loadFromFS(path, offset, size);
		if (buffer) player.setData(buffer);
	} else {
		player.setFileRange(path, offset, size);
	}
},
JSMP_Start: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.start();
},
JSMP_Prepare: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.prepare();
},
JSMP_Stop: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.clearEntry();
	player.stop();
	player.waitprep = undefined;
},
JSMP_GetMovieInfo: function(id, movieInfoPtr) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return false;
	
	var movieInfo = player.movieInfo;
	if (!movieInfo) return false;

	setValue(movieInfoPtr + 4 *  0, movieInfo.is_playable,         'i32');
	setValue(movieInfoPtr + 4 *  1, movieInfo.has_alpha,           'i32');
	setValue(movieInfoPtr + 4 *  2, movieInfo.width,               'i32');
	setValue(movieInfoPtr + 4 *  3, movieInfo.height,              'i32');
	setValue(movieInfoPtr + 4 *  4, movieInfo.disp_width,          'i32');
	setValue(movieInfoPtr + 4 *  5, movieInfo.disp_height,         'i32');
	setValue(movieInfoPtr + 4 *  6, movieInfo.framerate_n,         'i32');
	setValue(movieInfoPtr + 4 *  7, movieInfo.framerate_d,         'i32');
	setValue(movieInfoPtr + 4 *  8, movieInfo.total_frames,        'i32');
	setValue(movieInfoPtr + 4 *  9, movieInfo.codec_type,          'i32');
	setValue(movieInfoPtr + 4 * 10, movieInfo.alpha_codec_type,    'i32');
	//setValue(movieInfoPtr + 4 * 11, movieInfo.audio_sampling_rate,   'i32'),
	//setValue(movieInfoPtr + 4 * 12, movieInfo.audio_num_channels,    'i32'),
	//setValue(movieInfoPtr + 4 * 13, movieInfo.audio_total_samples,   'i32'),
	//setValue(movieInfoPtr + 4 * 14, movieInfo.num_subtitle_channels, 'i32'),
	//setValue(movieInfoPtr + 4 * 15, movieInfo.max_subtitle_size,     'i32')
	return true;
},
JSMP_GetFrameInfo: function(id, frameInfoPtr) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return false;

	var frame = player.referFrame();
	if (!frame) return false;

	// フレーム情報構造体にデータをセットする
	var frameInfo = frame.frameInfo;
	setValue(frameInfoPtr + 4 *  0, frameInfo.frame_no               , 'i32');
	setValue(frameInfoPtr + 4 *  1, frameInfo.frame_no_per_file      , 'i32');
	setValue(frameInfoPtr + 4 *  2, frameInfo.width                  , 'i32');
	setValue(frameInfoPtr + 4 *  3, frameInfo.height                 , 'i32');
	setValue(frameInfoPtr + 4 *  4, frameInfo.disp_width             , 'i32');
	setValue(frameInfoPtr + 4 *  5, frameInfo.disp_height            , 'i32');
	setValue(frameInfoPtr + 4 *  6, frameInfo.framerate_n            , 'i32');
	setValue(frameInfoPtr + 4 *  7, frameInfo.framerate_d            , 'i32');
	setValue(frameInfoPtr + 4 *  8, frameInfo.time                   , 'i64');
	setValue(frameInfoPtr + 4 * 10, frameInfo.tunit                  , 'i64');
	setValue(frameInfoPtr + 4 * 12, frameInfo.cnt_concatenated_movie , 'i32');
	setValue(frameInfoPtr + 4 * 13, frameInfo.alpha_type             , 'i32');
	setValue(frameInfoPtr + 4 * 14, frameInfo.cnt_skipped_frames     , 'i32');

	return true;
},
JSMP_UpdateTextures: function(id, numTextures, textures) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return false;

	var frame = player.referFrame();
	if (!frame) return false;

	var frameInfo = frame.frameInfo;
	var buffer = frame.buffer;
	var width = player.movieInfo.width;
	var height = player.movieInfo.height;

	var offset = 0;
	var ya_size = frameInfo.y_pitch * height;
	var uv_size = frameInfo.u_pitch * height / 2;
	
	// YUV+Aのプレーンバッファを切り出す
	var y_plane = new Uint8Array(buffer, offset, ya_size); offset += ya_size;
	var u_plane = new Uint8Array(buffer, offset, uv_size); offset += uv_size;
	var v_plane = new Uint8Array(buffer, offset, uv_size); offset += uv_size;
	
	var y_tex = HEAP32[(textures>>2) + 0];
	var u_tex = HEAP32[(textures>>2) + 1];
	var v_tex = HEAP32[(textures>>2) + 2];
	var a_tex = (player.movieInfo.has_alpha && numTextures >= 4) ? 
		HEAP32[(textures>>2) + 3] : 0;
	
	// "texStorage2D" の有無でWebGL1.0かWebGL2.0か判別
	if (GLctx.texStorage2D && !GL.textures[y_tex].isAlpha8) {
		// WebGL2.0環境ではテクスチャが何故かRGBA8で作られるため、A8テクスチャを再作成する
		
		width = (width+255)& -256;
		height = (height+15)& -16;
		
		var createTexture = function(width, height, div) {
			var tex = GLctx.createTexture();
			GLctx.bindTexture(GLctx.TEXTURE_2D, tex);
			GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_S, GLctx.CLAMP_TO_EDGE);
			GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_WRAP_T, GLctx.CLAMP_TO_EDGE);
			GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MIN_FILTER, GLctx.LINEAR);
			GLctx.texParameteri(GLctx.TEXTURE_2D, GLctx.TEXTURE_MAG_FILTER, GLctx.LINEAR);
			GLctx.texImage2D(GLctx.TEXTURE_2D, 0, GLctx.ALPHA, width / div, height / div, 0, GLctx.ALPHA, GLctx.UNSIGNED_BYTE, null);
			return tex;
		};

		GLctx.deleteTexture(GL.textures[y_tex]);
		GL.textures[y_tex] = createTexture(width, height, 1);
		GLctx.deleteTexture(GL.textures[u_tex]);
		GL.textures[u_tex] = createTexture(width, height, 2);
		GLctx.deleteTexture(GL.textures[v_tex]);
		GL.textures[v_tex] = createTexture(width, height, 2);
		if (a_tex) {
			GLctx.deleteTexture(GL.textures[a_tex]);
			GL.textures[a_tex] = createTexture(width, height, 1);
		}
		GL.textures[y_tex].isAlpha8 = true;
	}

	var savedTexture = GLctx.getParameter(GLctx.TEXTURE_BINDING_2D);

	GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[y_tex]);
	GLctx.texSubImage2D(GLctx.TEXTURE_2D, 0, 0, 0, 
		frameInfo.y_pitch, height, GLctx.ALPHA, GLctx.UNSIGNED_BYTE, y_plane);

	GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[u_tex]);
	GLctx.texSubImage2D(GLctx.TEXTURE_2D, 0, 0, 0,
		frameInfo.u_pitch, height / 2, GLctx.ALPHA, GLctx.UNSIGNED_BYTE, u_plane);

	GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[v_tex]);
	GLctx.texSubImage2D(GLctx.TEXTURE_2D, 0, 0, 0,
		frameInfo.v_pitch, height / 2, GLctx.ALPHA, GLctx.UNSIGNED_BYTE, v_plane);

	if (a_tex) {
		var a_plane = new Uint8Array(buffer, offset, ya_size); offset += ya_size;

		GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[a_tex]);
		GLctx.texSubImage2D(GLctx.TEXTURE_2D, 0, 0, 0, 
			frameInfo.a_pitch, height, GLctx.ALPHA, GLctx.UNSIGNED_BYTE, a_plane);
	}

	GLctx.bindTexture(GLctx.TEXTURE_2D, savedTexture);

	// バッファをWorker側に返却する
	player.discardFrame();
	
	return true;
},
JSMP_Pause: function(id, sw) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.pause(sw != 0);
},
JSMP_IsPaused: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	return player.isPaused;
},
JSMP_GetTime: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	return player.time;
},
JSMP_Update: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	if (player.waitprep === undefined && player.movieInfo) {
		// 再生開始時、最低1フレームはWaitPrep状態を返さないといけない
		player.waitprep = true;
		return 2;		// WaitPrep状態を返す
	} else if (player.waitprep) {
		player.waitprep = false;
	}
	return player.getStatus();
},
JSMP_GetStatus: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	if (player.waitprep) {
		return 2;		// WaitPrep状態を返す
	}
	return player.getStatus();
},
JSMP_SetSeekPosition: function(id, seekpos) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.setSeekPosition(seekpos);
},
JSMP_Loop: function(id, sw) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.loop(sw);
},
JSMP_SetSpeed: function(id, speed) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.setSpeed(speed);
},
JSMP_EntryFile: function(id, pathptr, offset, size, repeat) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	var path = Pointer_stringify(pathptr);
	if (CriMana.isLocalPath(path)) {
		var buffer = CriMana.loadFromFS(path, offset, size);
		if (buffer) player.entryData(buffer);
	} else if (offset == 0 && size == 0) {
		player.entryFile(path, repeat);
	} else {
		player.entryFileRange(path, offset, size, repeat);
	}
	return true;
},
JSMP_GetNumberOfEntry: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return 0;
	return player.getNumEntry();
},
JSMP_ClearEntry: function(id) {
	var player = CriMana.Player.findInstance(id);
	if (!player) return;
	player.clearEntry();
},
};

autoAddDeps(LibraryCriMana, '$CriMana');
mergeInto(LibraryManager.library, LibraryCriMana);
