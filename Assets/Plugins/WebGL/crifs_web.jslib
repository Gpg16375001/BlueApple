LibraryCriFs = {

$CriFs: {
	reqs: {},		// リクエスト中のXHRを保持する。キーはCriFsLoaderHn
	ins: {},		// 実行中のインストーラを保持する。キーはハンドル
	insCount: 0,	// インストーラの作成回数(ハンドルの数値に使う)
	idb: null,		// IDBFSに使う
	
	defineClasses: function() {
		CriFs.XHR = function() {
		};
		CriFs.XHR.prototype.start = function(url, offset, size, onload, onerror) {
			var req = new XMLHttpRequest();
			var rangeRequested = false;

			this.req = req;
			this.data = null;

			// offset,sizeが0の時はHEADリクエスト
			req.open((offset == 0 && size == 0) ? 'HEAD' : 'GET', url, true);	
			req.responseType = "arraybuffer";
			
			// offset,sizeが有効なときは、Rangeリクエストを追加
			if (offset > 0 || (size > 0 && size < 0xFFFFFFFF)) {
				req.setRequestHeader("Range", "bytes=" + offset + "-" + (size + offset - 1));
				// Rangeアクセスしたときキャッシュが有効だとChromeで失敗することがあるので無効化
				req.setRequestHeader('Pragma', 'no-cache');
				req.setRequestHeader('Cache-Control', 'no-cache');
				req.setRequestHeader('If-Modified-Since', 'Thu, 01 Jun 1970 00:00:00 GMT');
				rangeRequested = true;
			}

			// ロード完了
			req.onload = function() {
				if (req.status == 200 || req.status == 206) {
					if (req.status == 200 && rangeRequested) {
						// Rangeリクエストしたのに200で返すサーバー対策
						this.data = new Uint8Array(req.response, offset, size);
					} else {
						this.data = new Uint8Array(req.response)
					}
					onload(this.data.length);
				} else {
					onerror();
				}
			}.bind(this);
			// 正常なレスポンスが得られなかった
			req.onerror = function(e) {
				onerror();
			};
			// タイムアウト時もエラーとする
			req.ontimeout = onerror;
			// リクエスト開始
			req.send();
		};
		CriFs.XHR.prototype.stop = function() {
			if (this.req) this.req.abort();
		};
		CriFs.XHR.prototype.read = function(buffer, offset, size) {
			// データのコピー
			if (this.data) HEAP8.set(this.data.subarray(offset, offset + size), buffer);
		};

		CriFs.MEM = function() {
		};
		CriFs.MEM.prototype.start = function(path, offset, size, onload, onerror) {
			this.offset = offset;
			this.size = size;

			try {
				var stat = memfs.stat(path);
				this.stm = memfs.open(path, "r");
				var csize = stat.size - offset;
				if (size > 0 && size < 0xFFFFFFFF) {
					if (csize > size) csize = size;
				}
				onload(csize);
			} catch (e) {
				console.error("MEMFS: '" + path + "' is Not found.");
				onerror();
			}
		};
		CriFs.MEM.prototype.stop = function() {
			if (this.stm) memfs.close(this.stm);
		};
		CriFs.MEM.prototype.read = function(buffer, offset, size) {
			if (this.stm) memfs.read(this.stm, HEAP8, buffer, size, this.offset + offset);
			else return 0;
		};

		CriFs.IDB = function() {
		};
		CriFs.IDB.prototype.start = function(path, offset, size, onload, onerror) {
			var idb = CriFs.idb;

			this.offset = offset;
			this.size = size;
			this.info = null;
			this.entityData = null;

			function open(path, oncomplete, onerror) {
				var tr = idb.transaction(["toc"], "readonly");
				var store = tr.objectStore("toc");
				var req = store.get(path);
				req.onsuccess = function(e) {
					var res = req.result;
					if (res) oncomplete(res);
					else {
						// ファイルが存在しない
						console.error("IDBFS: '" + path + "' is Not found.");
						onerror();
					}
				}
				req.onerror = function(e) {
					onerror();
				}
				return tr;
			};
			function load(info, offset, size, oncomplete, onerror) {
				var tr = idb.transaction(["entities"], "readonly");
				var store = tr.objectStore("entities");
				
				var entities = info.entities;
				var entityData = new Array(entities.length);
				
				function readEntity(key, index) {
					var req = store.get(key);
					req.onsuccess = function(e) {
						var data = req.result;
						//console.log(readofs, readsize, data.byteLength);
						entityData[index] = data;
					}
					req.onerror = onerror;
				}

				var position = offset;
				var remain = size;
				for (var i = 0; i < entities.length; i++) {
					var entity = entities[i];
					if (position + remain <= entity.offset) {
						break;
					}
					if (entity.offset + entity.length < position) {
						continue;
					}
					var readofs = position - entity.offset;
					var readsize = (remain > entity.length - readofs) ? entity.length - readofs : remain;
					readEntity(entity.key, i);
					position += readsize;
					remain -= readsize;
				}

				tr.oncomplete = function() {
					oncomplete(entityData);
				};
				tr.onerror = onerror;
				return tr;
			};
			this.tr = open(path, function(info) {
				this.info = info;
				size = (size > 0 && size < 0xFFFFFFFF) ? size : info.size;
				this.buffer = new ArrayBuffer(size);
				this.tr = load(info, offset, size, function(entityData) {
					this.entityData = entityData;
					this.tr = null;
					onload(info.size);
				}.bind(this), onerror);
			}.bind(this), onerror);
		};
		CriFs.IDB.prototype.stop = function() {
			if (this.tr) this.tr.abort();
		};
		CriFs.IDB.prototype.read = function(buffer, offset, size) {
			var entities = this.info.entities;
			var entityData = this.entityData;
			var position = this.offset + offset;
			var remain = size;
			for (var i = 0; i < entities.length; i++) {
				var entity = entities[i];
				if (position + remain <= entity.offset) {
					break;
				}
				if (entity.offset + entity.length < position) {
					continue;
				}
				var data = entityData[i];
				var readofs = position - entity.offset;
				var readsize = (remain > entity.length - readofs) ? entity.length - readofs : remain;
				
				var ptr = buffer + position - this.offset;
				var target = HEAP8.subarray(ptr, ptr + readsize);
				target.set(data.subarray(readofs, readofs + readsize));

				position += readsize;
				remain -= readsize;
			}
		};

		CriFs.initDB = function(dbname) {
			var indexedDB = window.indexedDB || window.webkitIndexedDB;
			if (!indexedDB) return false;
			var req = indexedDB.open(dbname);
			req.onupgradeneeded = function(eve) {
				//console.log(eve.oldVersion, eve.newVersion);
				CriFs.idb = req.result;
				CriFs.idb.createObjectStore("toc", {keyPath:"path"});
				CriFs.idb.createObjectStore("entities", {autoIncrement:true});
			};
			req.onsuccess = function(eve) {
				CriFs.idb = req.result;
				//console.log(CriFs.idb.name, CriFs.idb.version);
				//console.log(CriFs.idb.objectStoreNames);
			};
			return true;
		};

		CriFs.writeFileToIDBFS = function(path, data, oncomplete, onerror) {
			var idb = CriFs.idb;

			// 1ファイルをIDBに書き込み
			function writeFile(data, oncomplete, onerror) {
				var tr = idb.transaction(["entities"], "readwrite");
				var store = tr.objectStore("entities");
				var toc = [];

				// 分割サイズ(仮)
				var entitySize = 256 * 1024;
				
				// ファイル実体(1ブロック分)をIDBに書き込み
				function writeEntity(data, offset, length) {
					var entityData = new Uint8Array(data, offset, length);
					var req = store.put(entityData);
					req.onsuccess = function(e) {
						toc.push({key: e.target.result, offset: offset, length: length});
					}
					req.onerror = function(e) {onerror();}
				}

				for (var offset = 0; offset < data.byteLength; ) {
					var length = (offset + entitySize < data.byteLength) ? entitySize : data.byteLength - offset;
					writeEntity(data, offset, length);
					offset += length;
				}
				tr.oncomplete = function(){
					oncomplete(toc, data);
				}
				tr.onerror = function(){onerror();}
			}

			// ファイルのTOC情報を書き込み
			function writeToc(path, size, toc, oncomplete, onerror) {
				var tr = idb.transaction(["toc"], "readwrite");
				var store = tr.objectStore("toc");
				var req = store.add({
					path: path,
					size: size,
					entities: toc
				});
				//req.onsuccess = function(e) {}
				req.onerror = function(e) {onerror();}
				tr.oncomplete = function() {oncomplete();}
				tr.onerror = function() {onerror();}
			}

			// ファイル実体書込完了
			function onCompletedWriteEntities(toc, data) {
				this.tr = writeToc(path, data.byteLength, toc, 
					onCompletedWriteToc.bind(this), 
					onErrorOfWriteToc.bind(this));
			}
			// ファイルTOC書込完了
			function onCompletedWriteToc() {
				oncomplete();
			}
			// ファイル実体書込エラー
			function onErrorOfWriteEntities() {
				onerror();
			}
			// ファイルTOC書込エラー
			function onErrorOfWriteToc() {
				onerror();
			}

			writeFile(data, onCompletedWriteEntities, onErrorOfWriteEntities);
		}

		CriFs.writeFileToMEMFS = function(path, data, oncomplete, onerror) {
			try {
				var stm = memfs.open(path, "w+");
				memfs.write(stm, new Uint8Array(data), 0, data.byteLength);
				memfs.close(stm);
				oncomplete();
			} catch (e) {
				onerror();
			}
		}

		var memfs;
		if (typeof FS !== "undefined") {
			// For Unity WebGL
			CriFs.DEF = CriFs.MEM;
			// Unityのファイルシステムを使う
			memfs = FS;
		} else {
			// For Web
			CriFs.DEF = CriFs.XHR;
			// 必要最低限実装のメモリFileSystem
			memfs = {
				files: {},
				stat: function(path) {
					if (path in memfs.files) {
						return {size: memfs.files[path].buf.byteLength};
					}
					throw {};
				},
				open: function(path, opt) {
					if (opt[0] == 'w') {
						return memfs.files[path] = {buf:null};
					} else  if (path in memfs.files) {
						return memfs.files[path];
					}
					throw {};
				},
				close: function() {},
				read: function(stm, u8buf, ptr, size, offset) {
					u8buf.subarray(ptr, ptr + size).set(stm.buf.subarray(offset, offset + size));
				},
				write: function(stm, u8buf, ptr, size, offset) {
					stm.buf = new Uint8Array(new ArrayBuffer(size));
					stm.buf.set(u8buf, ptr);
				},
			};
		}

		CriFs.Installer = function() {
			this.status = 0;
			this.errorCode = 0;
			this.httpCode = 0;
			this.contentsSize = 0;
			this.recievedSize = 0;
		}
		CriFs.Installer.prototype.start = function(url, path, oncomplete, onerror) {
			var write;
			if (path.indexOf("idbfs:") == 0) {
				if (CriFs.idb == null) {
					console.error("IDBFS is not initialized.");
					this.status = 3;
					this.errorCode = 3;
					return;
				}
				write = CriFs.writeFileToIDBFS;
				path = path.slice(6);
			} else if (path.indexOf("memfs:") == 0) {
				write = CriFs.writeFileToMEMFS;
				path = path.slice(6);
			} else {
				return;
			}

			// HTTP転送完了
			function onCompletedHttpRequest(contentLength) {
				var data = this.xhr.req.response;
				this.httpCode = this.xhr.req.status;
				this.contentsSize = contentLength;
				this.xhr = null;

				write(path, data, 
					onCompletedWritingFile.bind(this), 
					onErrorOfWritingFile.bind(this));
			}
			// HTTP転送エラー
			function onErrorOfHttpRequest() {
				this.status = 3;
				this.httpCode = this.xhr.req.status;
				this.errorCode = (this.httpCode) ? 7 : 1;
			}
			// ファイル書き込み完了
			function onCompletedWritingFile() {
				this.status = 2;
			}
			// ファイル書き込みエラー
			function onErrorOfWritingFile() {
				this.status = 3;
				this.errorCode = 3;
			}

			// typedef enum CriFsWebInstallerStatusTag
 			// {
 			// 	CRIFSWEBINSTALLER_STATUS_STOP,      /*JP< 停止中		*/	/*EN< Stopping			*/
 			// 	CRIFSWEBINSTALLER_STATUS_BUSY,      /*JP< 処理中		*/	/*EN< Busy				*/
 			// 	CRIFSWEBINSTALLER_STATUS_COMPLETE,  /*JP< 完了		*/	/*EN< Complete			*/
 			// 	CRIFSWEBINSTALLER_STATUS_ERROR      /*JP< エラー		*/	/*EN< Error				*/
 			// } CriFsWebInstallerStatus;

			// typedef enum CriFsWebInstallerErrorTag
			// {
			// 	CRIFSWEBINSTALLER_ERROR_NONE,       /*JP< エラーなし						*/	/*EN< No error					*/
			// 	CRIFSWEBINSTALLER_ERROR_TIMEOUT,    /*JP< タイムアウトエラー					*/	/*EN< Timeout error				*/
			// 	CRIFSWEBINSTALLER_ERROR_MEMORY,     /*JP< メモリ確保失敗					*/	/*EN< Memory allocation error	*/
			// 	CRIFSWEBINSTALLER_ERROR_LOCALFS,    /*JP< ローカルファイルシステムエラー			*/	/*EN< Local filesystem error	*/
			// 	CRIFSWEBINSTALLER_ERROR_DNS,        /*JP< DNSエラー						*/	/*EN< DNS error					*/
			// 	CRIFSWEBINSTALLER_ERROR_CONNECTION, /*JP< 接続エラー						*/	/*EN< Connection error			*/
			// 	CRIFSWEBINSTALLER_ERROR_SSL,        /*JP< SSLエラー						*/	/*EN< SSL error					*/
			// 	CRIFSWEBINSTALLER_ERROR_HTTP,       /*JP< HTTPエラー						*/	/*EN< HTTP error				*/
			// 	CRIFSWEBINSTALLER_ERROR_INTERNAL,   /*JP< 内部エラー						*/	/*EN< Internal error			*/
			// } CriFsWebInstallerError;

			this.xhr = new CriFs.XHR();
			this.xhr.start(url, 0, 0xffffffff, 
				onCompletedHttpRequest.bind(this),
				onErrorOfHttpRequest.bind(this));

			this.xhr.req.addEventListener("progress", function(e) {
				if (e.lengthComputable) {
					this.contentsSize = e.total;
					this.recievedSize = e.loaded;
				}
			}.bind(this));

			this.status = 1;
		}
		CriFs.Installer.prototype.stop = function() {
			if (this.xhr) this.xhr.stop();
		}
	},
},
criFsWeb_Init: function() {
	CriFs.defineClasses();
},
criFsWeb_SetupIDBFS: function(dbname) {
	CriFs.initDB(Pointer_stringify(dbname));
},
criFsWeb_Start: function(handle, urlptr, offset, size, onloadCb, onerrorCb) {
	var url = Pointer_stringify(urlptr);
	var onload = function(contentLength) {
		Runtime.dynCall("vii", onloadCb, [handle, contentLength]);
	}
	var onerror = function() {
		Runtime.dynCall("vi", onerrorCb, [handle]);
	}
	//console.log("criFsWeb_Start: ", url, offset, size);
	var path, fs;
	if (url.indexOf("http:") == 0 || url.indexOf("https:") == 0) {
		path = url;
		fs = CriFs.XHR;
	} else if (url.indexOf("memfs:") == 0) {
		path = url.slice(6);
		fs = CriFs.MEM;
	} else if (url.indexOf("idbfs:") == 0) {
		if (CriFs.idb == null) {
			console.error("IDBFS is not initialized.");
			return;
		}
		path = url.slice(6);
		fs = CriFs.IDB;
	} else {
		path = url;
		fs = CriFs.DEF;
	}

	var req = new fs();
	CriFs.reqs[handle] = req;
	req.start(path, offset, size, onload, onerror);
},
criFsWeb_Stop: function(handle) {
	//console.log("criFsWeb_Stop", buffer, offset, size);
	if (handle in CriFs.reqs) {
		var req = CriFs.reqs[handle];
		req.stop();
		delete CriFs.reqs[handle];
	}	
},
criFsWeb_Read: function(handle, buffer, offset, size) {
	//console.log("criFsWeb_Read", buffer, offset, size);
	if (handle in CriFs.reqs) {
		var req = CriFs.reqs[handle];
		req.read(buffer, offset, size);
	}
},
criFsWeb_CreateInstaller: function() {
	var handle = ++CriFs.insCount;
	var installer = new CriFs.Installer();
	CriFs.ins[handle] = installer;
	return handle;
},
criFsWeb_DestroyInstaller: function(handle) {
	delete CriFs.ins[handle];
},
criFsWeb_StartInstall: function(handle, urlptr, pathptr) {
	var installer = CriFs.ins[handle];
	var url = Pointer_stringify(urlptr);
	var path = Pointer_stringify(pathptr);
	installer.start(url, path, null, null);
},
criFsWeb_StopInstall: function(handle) {
	var installer = CriFs.ins[handle];
	installer.stop();
},
criFsWeb_GetInstallerStatusInfo: function(handle, info) {
	var installer = CriFs.ins[handle];
	setValue(info + 0, installer.status, "i32");
	setValue(info + 4, installer.errorCode, "i32");
	setValue(info + 8, installer.httpCode, "i32");
	setValue(info + 12, installer.contentsSize, "i64");
	setValue(info + 20, installer.recievedSize, "i64");
}
};

autoAddDeps(LibraryCriFs, '$CriFs');
mergeInto(LibraryManager.library, LibraryCriFs);
