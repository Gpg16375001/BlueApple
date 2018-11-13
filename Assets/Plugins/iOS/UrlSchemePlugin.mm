#import "UnityAppController.h"

@interface UnityController : UnityAppController

@end

NSString *m_url = nil;

@implementation UnityController

extern "C"
{
    char* _getURLScheme();
}

char* _getURLScheme()
{
    if(!m_url){
        m_url = @"";
    }
    
    char* mem = static_cast<char*>( malloc( strlen(m_url.UTF8String ) + 1 ));
    if(mem) {
        strcpy(mem, m_url.UTF8String);
    }
    m_url = nil;
    return mem;
}


- (BOOL)application:(UIApplication *)application openURL:(nonnull NSURL *)url sourceApplication:(nullable NSString *)sourceApplication annotation:(nonnull id)annotation
{
    m_url = [url absoluteString];
    return YES;
}

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler
{
    m_url = [userActivity.webpageURL absoluteString];
    return YES;
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(UnityController)
