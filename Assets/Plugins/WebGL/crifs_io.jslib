/****************************************************************************
 *
 * CRI Middleware SDK
 *
 * Copyright (c) 2016 CRI Middleware Co., Ltd.
 *
 * Library  : CRI File System
 * Module   : I/O interface of JavaScript library
 * File     : crifs_io.jslib
 *
 ****************************************************************************/
var LibraryCriFsIo = {
	/*==========================================================================
	/* 内部関数
	/*========================================================================*/
	$CriFsIo: {
		/*----------------------------------------------------------------------
		/* 基本関数
		/*--------------------------------------------------------------------*/
		/* ファイルの検索 */
		findFile: function (filePath, callback) {
			/* XMLHttpRequestオブジェクトの作成 */
			var request = new XMLHttpRequest();

			/* ファイル情報取得後の処理を登録 */
			request.onreadystatechange = function () {
				/* ステータスのチェック */
				/* 備考）取得完了までは何もしない。 */
				if (request.readyState == 4) {
					var fileSize;

					/* エラーチェック */
					if (request.status == 200) {
						/* 成功時はファイルサイズを保存 */
						fileSize = request.getResponseHeader("Content-Length");
					} else {
						/* 失敗時はエラーコードを表示 */
						console.log("E2016022601JS: Failed to find " + filePath + ". (status = " + request.status + ")");
						fileSize = -1;
					}

					/* リード完了を通知 */
					callback(fileSize);
				}
			};

			/* エラー発生時の処理を登録 */
			request.onerror = function () {
				/* 失敗時はエラーコードを表示 */
				console.log("E2016022602JS: Reading error occurred while finding " + filePath + " .");

				/* リード完了を通知 */
				callback(-1);
			}

			/* タイムアウト処理の設定 */
			/* 備考）60秒間応答がない場合は処理をタイムアウト。 */
			request.timeout = 60000;

			/* エラー発生時の処理を登録 */
			request.ontimeout = function () {
				/* 失敗時はエラーコードを表示 */
				console.log("E2016022604JS: Request for " + filePath + " timed out.");

				/* リード完了を通知 */
				callback(-1);
			}

			/* ファイルの取得リクエストの作成 */
			request.open("HEAD", filePath, true);

			/* リクエストの送信 */
			try {
				request.send(null);
			}
			catch (e) {
				/* 失敗時はエラーコードを表示 */
				console.log("E2016022603JS: Failed to send request to find " + filePath + ".");

				/* リード完了を通知 */
				callback(-1);
			}
		},

		/* ファイルのロード */
		loadFile: function (filePath, callback) {
			/* XMLHttpRequestオブジェクトの作成 */
			var request = new XMLHttpRequest();

			/* ロード完了後の処理を登録 */
			request.onload = function () {
				var byteArray;

				/* エラーチェック */
				if (request.status == 200) {
					/* 成功時はリード結果を保存 */
					byteArray = new Uint8Array(request.response);
				} else {
					/* 失敗時はエラーコードを表示 */
					console.log("E2016022301JS: Failed to load " + filePath + ". (status = " + request.status + ")");
					byteArray = null;
				}

				/* リード完了を通知 */
				callback(byteArray);
			};

			/* エラー発生時の処理を登録 */
			request.onerror = function () {
				/* 失敗時はエラーコードを表示 */
				console.log("E2016022302JS: Reading error occurred while loading " + filePath + " .");

				/* リード完了を通知 */
				callback(null);
			}

			/* ファイルの取得リクエストの作成 */
			request.open("GET", filePath, true);
			request.responseType = 'arraybuffer';

			/* リクエストの送信 */
			try {
				request.send(null);
			}
			catch (e) {
				/* 失敗時はエラーコードを表示 */
				console.log("E2016022303JS: Failed to send request to load " + filePath + ".");

				/* リード完了を通知 */
				callback(null);
			}
		},

		/*----------------------------------------------------------------------
		/* CriFsIoインターフェース
		/*--------------------------------------------------------------------*/
		/* ロード済みファイルリスト */
		fileList: {},

		/* ファイルのロード */
		load: function (filePath) {
			/* リクエス発行済みかどうかチェック */
			if ((filePath in CriFsIo.fileList) == false) {
				/* リクエスト未発行時は新規リクエストを作成 */
				/* 備考）ファイル名をキーにしてファイル情報を登録。 */
				var initialInfo = {};
				initialInfo.referenceCount = 0;
				initialInfo.fileSize = -1;
				initialInfo.byteArray = null;
				initialInfo.isErrorOccurred = false;
				CriFsIo.fileList[filePath] = initialInfo;
			}

			/* ファイル情報の取得 */
			var fileInfo = CriFsIo.fileList[filePath];

			/* エラーチェック */
			if (fileInfo.isErrorOccurred != false) {
				/* 備考）エラーが発生したファイルのロードはリトライしない */
				return;
			}

			/* 参照カウントの更新 */
			fileInfo.referenceCount++;

			/* 初回ロードかどうかチェック */
			if (fileInfo.referenceCount != 1) {
				/* 多重ロードは行わない */
				return;
			}

			/* ファイル検索の実行 */
			CriFsIo.findFile(filePath,
				/* 検索完了時はファイルサイズを更新 */
				function (fileSize) {
					/* エラーチェック */
					if (fileSize < 0) {
						fileInfo.isErrorOccurred = true;
						return;
					}

					/* ファイルサイズの保存 */
					fileInfo.fileSize = fileSize;

					/* 続けてファイルをロード */
					CriFsIo.loadFile(filePath,
						/* 検索完了時はファイルサイズを更新 */
						function (byteArray) {
							/* エラーチェック */
							if (byteArray == null) {
								fileInfo.isErrorOccurred = true;
								return;
							}

							/* バイト列の保存 */
							fileInfo.byteArray = byteArray;
						}
					);
				}
			);
		},

		/* ロード完了チェック */
		isLoaded: function (filePath) {
			/* ファイル情報が取得済みかどうかチェック */
			if ((filePath in CriFsIo.fileList) == false) {
				return false;
			}

			/* ファイルがロード済みかどうかチェック */
			/* 備考）バイト列がnullかつエラー未発生時はまだロード中。 */
			var fileInfo = CriFsIo.fileList[filePath];
			if ((fileInfo.byteArray == null) && (fileInfo.isErrorOccurred == false)) {
				return false;
			}

			return true;
		},

		/* ファイルサイズの取得 */
		getFileSize: function (filePath) {
			/* ファイル情報が取得済みかどうかチェック */
			if ((filePath in CriFsIo.fileList) == false) {
				return -1;
			}

			/* ファイル情報の取得 */
			var fileInfo = CriFsIo.fileList[filePath];

			/* ファイルサイズを返す */
			return fileInfo.fileSize;
		},

		/* ファイルのリード */
		fetch: function (filePath, offset, length, pointer) {
			/* ファイル情報が取得済みかどうかチェック */
			if ((filePath in CriFsIo.fileList) == false) {
				return null;
			}

			/* byteArrayの取得 */
			var fileInfo = CriFsIo.fileList[filePath];
			if (fileInfo.byteArray == null) {
				return null;
			}

			/* コピー範囲の切り出し */
			var subArray = fileInfo.byteArray.subarray(offset, offset + length);

			return subArray;
		},

		/* ファイルのアンロード */
		unload: function (filePath) {
			/* ファイル情報が取得済みかどうかチェック */
			if ((filePath in CriFsIo.fileList) == false) {
				return;
			}

			/* ファイル情報の取得 */
			var fileInfo = CriFsIo.fileList[filePath];

			/* 参照カウントを更新 */
			fileInfo.referenceCount--;
			if (fileInfo.referenceCount != 0) {
				return;
			}

			/* データのアンロード */
			fileInfo.byteArray = null;

			/* 備考）キーはクリアしない。 */
			/* 　　　→再利用時のため、ファイルサイズを残しておく。 */
		},

		/* エラーの有無を確認 */
		isErrorOccurred: function (filePath) {
			/* ファイル情報が取得済みかどうかチェック */
			if ((filePath in CriFsIo.fileList) == false) {
				return true;
			}

			/* ファイル情報の取得 */
			var fileInfo = CriFsIo.fileList[filePath];

			/* エラーの有無を返す */
			return fileInfo.isErrorOccurred;
		}
	},

	/*==========================================================================
	/* 外部関数
	/*========================================================================*/
	/* 検索 */
	criFsIoJs_Load: function (path) {
		CriFsIo.load(Pointer_stringify(path));
	},

	/* ファイル検索完了チェック */
	criFsIoJs_IsLoaded: function (path) {
		return CriFsIo.isLoaded(Pointer_stringify(path));
	},

	/* プリロード済みファイルサイズの取得 */
	criFsIoJs_GetFileSize: function (path) {
		return CriFsIo.getFileSize(Pointer_stringify(path));
	},

	/* アンロード */
	criFsIoJs_Unload: function (path) {
		CriFsIo.unload(Pointer_stringify(path));
	},

	/* データの読み込み */
	criFsIoJs_Read: function (path, offset, length, pointer) {
		/* コピー元の取得 */
		var srcArray = CriFsIo.fetch(Pointer_stringify(path), offset, length);
		if (srcArray == null) {
			return -1;
		}

		/* コピー先の取得 */
		var dstArray = new Uint8Array(Module.HEAPU8.buffer, pointer, length);

		/* データのコピー */
		for (var i = 0; i < length; i++) {
			dstArray[i] = srcArray[i];
		}

		return length;
	},

	/* エラーの有無を確認 */
	criFsIoJs_IsErrorOccurred: function (path) {
		return CriFsIo.isErrorOccurred(Pointer_stringify(path));
	}
}

/* インターフェースの登録 */
autoAddDeps(LibraryCriFsIo, '$CriFsIo');
mergeInto(LibraryManager.library, LibraryCriFsIo);

/* --- end of file --- */
