#include "Unity/WWWConnection.h"

// キャッシュしないで通信する用のWWWカスタムプロバイダ。キャッシュしたくないだけなのでここでの中身は空。
@interface UnityWWWCustomRequestProvider : UnityWWWRequestDefaultProvider
{
}
+ (NSMutableURLRequest*)allocRequestForHTTPMethod:(NSString*)method url:(NSURL*)url headers:(NSDictionary*)headers;
@end
@implementation UnityWWWCustomRequestProvider
+ (NSMutableURLRequest*)allocRequestForHTTPMethod:(NSString*)method url:(NSURL*)url headers:(NSDictionary*)headers
{
    NSMutableURLRequest* request = [super allocRequestForHTTPMethod:method url:url headers:headers];

    // キャッシュしない設定
    request.cachePolicy = NSURLRequestReloadIgnoringLocalCacheData;
    [request setHTTPShouldHandleCookies:NO];

    return request;
}
@end

// カスタムリクエストのハンドリングクラス
@interface UnityWWWCustomConnectionDelegate : UnityWWWConnectionDelegate
{
}
@end
@implementation UnityWWWCustomConnectionDelegate
- (NSCachedURLResponse*)connection:(NSURLConnection*)connection willCacheResponse:(NSCachedURLResponse*)cachedResponse
{
    // nilを返すことでcash無効
    return nil;
}
- (void)connection:(NSURLConnection*)connection didReceiveResponse:(NSURLResponse*)response
{
    // とりあえず接続が発生したらログを出しておく
    [super connection:connection didReceiveResponse:response];
    if([response isMemberOfClass:[NSHTTPURLResponse class]])
        ::printf_console("We've got response with status: %d\n", [(NSHTTPURLResponse*)response statusCode]);
}
@end

IMPL_WWW_DELEGATE_SUBCLASS(UnityWWWCustomConnectionDelegate);
IMPL_WWW_REQUEST_PROVIDER(UnityWWWCustomRequestProvider);