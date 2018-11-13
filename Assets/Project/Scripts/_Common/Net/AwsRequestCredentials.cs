using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

using SmileLab;


/// <summary>
/// AWS API Gatewayを用いた通信時の通信情報.
/// </summary>
public class AwsRequestCredentials : CognitoAWSCredentials
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public AwsRequestCredentials(string identityPoolId, RegionEndpoint region, string providerName) : base(identityPoolId, region)
    {
        m_provider = providerName;
        AddLogin(m_provider, string.Empty);
        ClearCredentials();
    }

    /// <summary>
    /// 開発者Credentialに対してのログイン認証処理.ログインAPIコール直後これをやらないとSyncできないので注意.
    /// </summary>
    public void AuthLoggedin(string identityId, string token)
    {
        bool isChangeId = (m_identityId != null && m_identityId != identityId);
        bool isChangeToken = (m_token != null && m_token != token);
        m_identityId = identityId;
        m_token = token;

        if (isChangeId || isChangeToken) {
            if (isChangeId) {
                ClearIdentityCache ();
            }
            ClearCredentials ();
        }
    }

    // リフレッシュを待たないとログインできない.
    protected override IdentityState RefreshIdentity()
    {
        Debug.Log("AwsRequestCredentials invoke RefreshIdentity");
        IdentityState state = null;
        if (string.IsNullOrEmpty (m_identityId) || string.IsNullOrEmpty (m_token)) {
            ManualResetEvent waitLock = new ManualResetEvent (false);
            MainThreadDispatcher.StartUpdateMicroCoroutine (WaitAuthenticated ((s) => {
                state = s;
                waitLock.Set ();
            }));
            waitLock.WaitOne ();
        } else {
            state = new IdentityState(m_identityId, m_provider, m_token, false);
        }
        return state;
    }
    // ログイン認証待機.
    private IEnumerator WaitAuthenticated(Action<IdentityState> didLogin)
    {
        while(string.IsNullOrEmpty(m_identityId) || string.IsNullOrEmpty(m_token)){
            yield return null;
        }
        var state = new IdentityState(m_identityId, m_provider, m_token, false);
        didLogin(state);
    }

    private string m_provider;
    private string m_identityId;
    private string m_token;
}
