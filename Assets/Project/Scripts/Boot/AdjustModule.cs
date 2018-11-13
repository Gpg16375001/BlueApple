using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

using SmileLab;

public class AdjustModule : ViewBase {
    public static AdjustModule SharedInstance { get; private set; }

    void Awake()
    {
        if(SharedInstance != null) {
            SharedInstance.Dispose();
        }
        SharedInstance = this;
    }

    public string appToken;

	public void Init () {
        AdjustEnvironment environment = AdjustEnvironment.Sandbox;
        AdjustLogLevel logLevel = AdjustLogLevel.Info;
        bool eventBuffering = false;
        bool sendInBackground = false;
        bool launchDeferredDeeplink = true;

#if DEFINE_RELEASE
        environment = AdjustEnvironment.Production;
        logLevel = AdjustLogLevel.Suppress;
#endif
        AdjustConfig config = new AdjustConfig(appToken, environment, false);
        config.setLogLevel(logLevel);
        config.setSendInBackground(sendInBackground);
        config.setEventBufferingEnabled(eventBuffering);
        config.setLaunchDeferredDeeplink(launchDeferredDeeplink);
        config.setAppSecret(1, 1960099722, 1496639470, 292428053, 2055538249);
        Adjust.start(config);

        PurchaseManager.SharedInstance.SucceedEvent += PaymentSucceedEvent;
	}

    private void PaymentSucceedEvent(SkuItem item)
    {
        AdjustEvent adjustEvent = new AdjustEvent("cg0nl9");
        adjustEvent.setRevenue(item.price, item.currencyCode);
        adjustEvent.addCallbackParameter("uid", AwsModule.UserData.UserID.ToString());
        Adjust.trackEvent (adjustEvent);
    }
}
