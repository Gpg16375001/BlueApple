using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Amazon;
using MessagePack;
using SmileLab.Net;
using SmileLab;
using SmileLab.Net.API;

/// <summary>
/// AWS SDK "API Gateway"を用いた通信リクエストのAPI.
/// </summary>
public class AwsRequestAPI
{
    private static bool _authTokenChanged = false;
    private static string _authToken = string.Empty;
    /// <summary>
    /// リクエストトークン.
    /// </summary>
    public static string AuthToken {
        get { return _authToken; }
        set {
            _authTokenChanged = _authToken != value;
            _authToken = value;
        }
    }

    public Func<int, Action, Action, bool> CheckMasterVersion;


    /// <summary>
    /// 準備OK？Certificationを実施するまではfalse.
    /// </summary>
    public bool IsReady { get; private set; }


    /// <summary>
    /// 通信実行.
    /// </summary>
    public void Exec<T>(BaseSendAPI send, Action<T> didLoad) where T : BaseReceiveAPI
    {
        // ※ユーザー認証など初期の通信には認証情報を含められない.
        SetAuthToken();
		// 署名を追加.
		SetSignature(send);
  
		byte[] post = Encoding.UTF8.GetBytes(JsonUtility.ToJson(send));
        NetRequestManager.Post(send.URL, post, CommonHeader, 
                               res => CallbackDidLoad(send, res, didLoad),
                               e => CallbackDidError(send, e, didLoad));
    }

    void SetAuthToken()
    {
        if (_authTokenChanged) {
            if (CommonHeader.ContainsKey ("Authorization")) {
                CommonHeader.Remove ("Authorization");
            } 
            if (!string.IsNullOrEmpty (AuthToken)) {
                CommonHeader.Add ("Authorization", AuthToken);
            }
            _authTokenChanged = false;
        }
    }
	void SetSignature(BaseSendAPI send)
	{
		var sig = (send.Date + ":" + JsonUtility.ToJson(send) + ":" + AuthToken).ToHashMD5();
        if (CommonHeader.ContainsKey("Signature")) {
			CommonHeader.Remove("Signature");
        }
		CommonHeader.Add("Signature", sig);
	}

    // コールバック：通信成功.
    void CallbackDidLoad<T>(BaseSendAPI send, UnityWebRequest response, Action<T> didLoad) where T : BaseReceiveAPI
    {
        // TODO : エラーに応じたリブートなどの処理が必要.エラーも定数一覧出力.
        if (response.isNetworkError) {
            Debug.LogError("[AwsRequestAPI] CallbackDidLoad Error!! : "+response.error);
            LockInputManager.SharedInstance.IsLock = false;
            // こちらは通信環境などによるエラーのため出たら「通信環境がいいところで再度試してみてね」でタイトルに戻す。
            PopupManager.OpenPopupSystemOK("通信エラーが発生しました。通信環境の良いところで再度お試しください。", () => ScreenChanger.SharedInstance.Reboot());
            return;
        }
        else if(response.isHttpError) {
            Debug.LogError("[AwsRequestAPI] CallbackDidLoad Error!! : "+response.error+"\nresCode="+response.responseCode);
            if (response.responseCode == 401 && send.retryCount < 5) {
                // 認証エラーはAuthを取り直してリトライする。
                // 複数回やってダメならエラー処理にしてタイトルに戻す
                AuthToken = string.Empty;
                RequestUserAuth (ret => {
                    if (ret) {
                        // retry
                        if(!ret) {
                            LockInputManager.SharedInstance.IsLock = false;
                            PopupManager.OpenPopupSystemOK("ユーザー認証に失敗しました。再起動します。", () => ScreenChanger.SharedInstance.Reboot());
                            return;
                        }
                        SetAuthToken();
						SetSignature(send);

                        send.retryCount++;
                        byte[] post = Encoding.UTF8.GetBytes (JsonUtility.ToJson (send));
                        NetRequestManager.Post (send.URL, post, CommonHeader, 
                            res => CallbackDidLoad (send, res, didLoad),
                            e => CallbackDidError (send, e, didLoad));
                    }
                });
            } else if (response.responseCode == 503 && send.retryCount < 5) {
                LockInputManager.SharedInstance.UnlockBlockBegin ();
                // サーバータイムアウトの場合はユーザー確認をした上で再度リクエストを投げる。
                PopupManager.OpenPopupSystemYN("タイムアウトしました。リトライしますか？",
                    () => {
                        LockInputManager.SharedInstance.UnlockBlockEnd ();
                        send.retryCount++;
                        byte[] post = Encoding.UTF8.GetBytes (JsonUtility.ToJson (send));
                        NetRequestManager.Post (send.URL, post, CommonHeader, 
                            res => CallbackDidLoad (send, res, didLoad),
                            e => CallbackDidError (send, e, didLoad));
                    }, 
                    () => {
                        LockInputManager.SharedInstance.UnlockBlockEnd ();
                        LockInputManager.SharedInstance.IsLock = false;
                        PopupManager.OpenPopupSystemOK("通信が完了しないため、再起動します。", () => ScreenChanger.SharedInstance.Reboot());
                    }
                );
            } else {
                LockInputManager.SharedInstance.IsLock = false;
                // エラー復帰ができないことが濃厚なのでタイトルに戻す。
                PopupManager.OpenPopupSystemOK("通信エラーが発生しました。再起動します。", () => ScreenChanger.SharedInstance.Reboot());
            }
            return;
        }
        Debug.Log("[AwsRequestAPI] CallbackDidLoad Success : url="+response.url+"\nresCode="+response.responseCode);
        var roc = MessagePackSerializer.Deserialize<T> (response.downloadHandler.data);
		if(roc.ResultCode == (int)ServerResultCodeEnum.UNDER_MAINTENANCE){
			
		}
        if(didLoad != null){
            didLoad(roc);
        }
    }
    // コールバック：通信エラー.
    private void CallbackDidError<T>(BaseSendAPI send, Exception exception, Action<T> didLoad) where T : BaseReceiveAPI
    {
        Debug.LogError("[AwsRequestAPI] CallbackDidError!! : " + exception.Message);
        if (exception != null && exception is UniRx.WebRequest.UnityWebRequestErrorException) {
            var webRequsetError = exception as UniRx.WebRequest.UnityWebRequestErrorException;
            if (webRequsetError.Request.isHttpError){
                if (webRequsetError.Request.responseCode == 401 && send.retryCount < 5) {
                    AuthToken = string.Empty;
                    RequestUserAuth (ret => {
                        if (ret) {
                            // retry
                            if(!ret) {
                                LockInputManager.SharedInstance.IsLock = false;
                                PopupManager.OpenPopupSystemOK("認証でエラーが発生しました。\n再起動します。", () => ScreenChanger.SharedInstance.Reboot());

                                return;
                            }
                            SetAuthToken();
							SetSignature(send);
                            
                            send.retryCount++;
                            byte[] post = Encoding.UTF8.GetBytes (JsonUtility.ToJson (send));
                            NetRequestManager.Post (send.URL, post, CommonHeader, 
                                res => CallbackDidLoad (send, res, didLoad),
                                e => CallbackDidError (send, e, didLoad));
                        }
                    });
                    return;
                }
            } else if (webRequsetError.Request.responseCode == 503 && send.retryCount < 5) {
                // サーバータイムアウトの場合はユーザー確認をした上で再度リクエストを投げる。
                var prevLock = LockInputManager.SharedInstance.IsLock;
                LockInputManager.SharedInstance.IsLock = false;
                PopupManager.OpenPopupSystemYN("通信がタイムアウトしました。\nリトライしますか？",
                    () => {
                        send.retryCount++;
                        byte[] post = Encoding.UTF8.GetBytes (JsonUtility.ToJson (send));

                        if(prevLock) {
                            LockInputManager.SharedInstance.IsLock = true;
                        }

                        NetRequestManager.Post (send.URL, post, CommonHeader, 
                            res => CallbackDidLoad (send, res, didLoad),
                            e => CallbackDidError (send, e, didLoad));
                    }, 
                    () => {
                        PopupManager.OpenPopupSystemOK("通信が完了できないため、\n再起動します。", () => ScreenChanger.SharedInstance.Reboot());
                    }
                );
            }
        }
        if (exception != null && exception is TimeoutException) {
            if(send.retryCount < 5) {
                // サーバータイムアウトの場合はユーザー確認をした上で再度リクエストを投げる。
                var prevLock = LockInputManager.SharedInstance.IsLock;
                LockInputManager.SharedInstance.IsLock = false;
                PopupManager.OpenPopupSystemYN("通信がタイムアウトしました。\nリトライしますか？",
                    () => {
                        send.retryCount++;
                        byte[] post = Encoding.UTF8.GetBytes (JsonUtility.ToJson (send));

                        if(prevLock) {
                            LockInputManager.SharedInstance.IsLock = true;
                        }
                        NetRequestManager.Post (send.URL, post, CommonHeader, 
                            res => CallbackDidLoad (send, res, didLoad),
                            e => CallbackDidError (send, e, didLoad));
                    }, 
                    () => {
                        PopupManager.OpenPopupSystemOK("通信が完了できないため、\n再起動します。", () => ScreenChanger.SharedInstance.Reboot());
                    }
                );
                return;
            }
        }
        if(didLoad != null) {
            didLoad(null);
        }
    }

    //>通信リクエスト：ユーザー認証.
    public void RequestUserAuth(Action<bool> didConnectEnd)
    {
        var localData = AwsModule.UserData;
		var pass = localData.GetAuthPass() + ":glare:" + GameTime.SharedInstance.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz");

		SendAPI.AuthIndex(localData.GetAuthUserName(), EncryptHelper.ToHashMD5(pass),
            (success, res) => {
                if(!success || res == null) {
                    if(res != null && res.ResultCode == (int)ServerResultCodeEnum.INVALID_PASSWORD) {
                        // 他に引き継ぎされている
                        LockInputManager.SharedInstance.IsLock = false;
                        PopupManager.OpenPopupSystemOK(
                            "認証情報が無効になっています。\n認証情報の初期化を行い再起動します。",
                            () => {
                                AwsLocalUserData.DeleteUserInfoFile();
                                ScreenChanger.SharedInstance.Reboot();
                            }
                        );
                        return;
                    }

                    didConnectEnd(false);
                    return; // エラー.
                }
                AwsRequestAPI.AuthToken = res.Token;
                AwsModule.AuthLoggedin(res.IdentityId, res.Token);

                // ここでSmartBeat用のデータ登録をしておく
                SmartBeat.SmartBeat.setUserId(AwsModule.UserData.UserID.ToString());
                SmartBeat.SmartBeat.addExtraData("CustomerId", AwsModule.UserData.CustomerID);

                didConnectEnd(true);
            }
        );
    }

    public void CheckResultCode<T>(T response, Action<bool, T> didLoad, bool checkVersion = true) where T : BaseReceiveAPI
    {
        if (response == null) {
            // どうしようもないのでタイトルに戻す。
            LockInputManager.SharedInstance.IsLock = false;
            PopupManager.OpenPopupSystemOK("通信の結果を取得できなかったため、\nアプリを再起動します。", () => {
                didLoad(false, response);
                ScreenChanger.SharedInstance.Reboot();
            });
            return;
        }


    	var ResultCode = (ServerResultCodeEnum)response.ResultCode;
        if (ResultCode != ServerResultCodeEnum.SUCCESS && MasterDataTable.server_result_code != null) {
            var resultData = MasterDataTable.server_result_code [ResultCode];
            if (resultData != null && resultData.proc != ServerResultCodeProcEnum.None) {
				Debug.Log(response.ErrorMessage);
				resultData.proc.Execute (ResultCode, response.ErrorMessage);
            }
        }

        if (checkVersion && CheckMasterVersion != null) {
            if (CheckMasterVersion (response.MasterVersion, () => didLoad(ResultCode == ServerResultCodeEnum.SUCCESS, response), () => didLoad(false, response))) {
                return;
            }
        }
        didLoad(ResultCode == ServerResultCodeEnum.SUCCESS, response);
    }

	public void CheckResultCodeRetry<T>(T response, Action<bool, T> didLoad,
        bool checkVersion = true, bool throughError = false, Action retryFunc = null, Action retireFunc = null) where T : BaseReceiveAPI
	{
		if (response == null) {
			// どうしようもないのでタイトルに戻す。
			var prevLock = LockInputManager.SharedInstance.IsLock;
			LockInputManager.SharedInstance.IsLock = false;
            if (retryFunc != null) {
                PopupManager.OpenPopupSystemYN ("通信の結果を取得できませんでした。\nリトライしますか？",
                    () => {
                        if (prevLock) {
                            LockInputManager.SharedInstance.IsLock = true;
                        }
                        retryFunc ();
                    },
                    () => {
                        if (retireFunc != null) {
                            retireFunc ();
                        } else {
                            PopupManager.OpenPopupSystemOK ("通信の結果を取得できなかったため、\nアプリを再起動します。", () => {
                                didLoad (false, response);
                                ScreenChanger.SharedInstance.Reboot ();
                            });
                        }
                    });
            } else {
                if (retireFunc != null) {
                    retireFunc ();
                } else {
                    PopupManager.OpenPopupSystemOK ("通信の結果を取得できなかったため、\nアプリを再起動します。", () => {
                        didLoad (false, response);
                        ScreenChanger.SharedInstance.Reboot ();
                    });
                }
            }
			return;
		}

		var ResultCode = (ServerResultCodeEnum)response.ResultCode;
		if (!throughError && ResultCode != ServerResultCodeEnum.SUCCESS && MasterDataTable.server_result_code != null) {
			var resultData = MasterDataTable.server_result_code [ResultCode];
			if (resultData != null && resultData.proc != ServerResultCodeProcEnum.None) {
				Debug.Log(response.ErrorMessage);
				resultData.proc.Execute (ResultCode, response.ErrorMessage);
			}
		}

		if (checkVersion && CheckMasterVersion != null) {
			if (CheckMasterVersion (response.MasterVersion, () => didLoad(ResultCode == ServerResultCodeEnum.SUCCESS, response), () => didLoad(false, response))) {
				return;
			}
		}
		didLoad(ResultCode == ServerResultCodeEnum.SUCCESS, response);
	}

    // 初期化.
    public AwsRequestAPI()
    {
        IsReady = false;

        CommonHeader.Add("Accept", "application/x-msgpack");
        CommonHeader.Add("Content-Type", "application/json; charset=UTF-8");
		CommonHeader.Add("x-api-key", ClientDefine.KEY_AWS_API);
    }


    private Dictionary<string, string> CommonHeader = new Dictionary<string, string>();
}