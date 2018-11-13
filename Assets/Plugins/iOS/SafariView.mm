#import <SafariServices/SafariServices.h>

extern UIViewController * UnityGetGLViewController();
extern void UnitySendMessage( const char * className, const char * methodName, const char * param );
 
extern "C"
{
  NSString * gameObjectName;
  @interface SafariViewController : UIViewController<SFSafariViewControllerDelegate>
  // ...
  @end
 
  @implementation SafariViewController
  - (void)safariViewControllerDidFinish:(SFSafariViewController *)controller {
    UnitySendMessage([gameObjectName UTF8String], "DidFinish", "");
  }
  // 指定したページのロード完了したら呼ばれる
  // （内部で遷移したとしても、最初のページのロード完了時しか呼ばれない）
  - (void)safariViewController:(SFSafariViewController *)controller
    didCompleteInitialLoad:(BOOL)didLoadSuccessfully
  {
  }
  // アクションボタン（下にあるボタン群）が押されたときに呼ばれる
  - (NSArray<UIActivity *> *)safariViewController:(SFSafariViewController *)controller
    activityItemsForURL:(NSURL *)URL
    title:(NSString *)title
  {
      return @[];
  }
  @end
 
  SafariViewController * svc;
  

  void SafariViewInit(const char * gameObjectName_)
  {
    gameObjectName = [NSString stringWithUTF8String:gameObjectName_];
  }

  void LaunchUrl(const char * url)
  {
    // Get the instance of ViewController that Unity is displaying now
    UIViewController * uvc = UnityGetGLViewController();
 
    // Generate an NSURL object based on the C string passed from C#
    NSURL * URL = [NSURL URLWithString: [[NSString alloc] initWithUTF8String:url]];
 
    // Create an SFSafariViewController object from the generated URL
    SFSafariViewController * sfvc = [[SFSafariViewController alloc] initWithURL:URL];
 
    // Assign a delegate to handle when the user presses the 'Done' button
    svc = [[SafariViewController alloc] init];
    sfvc.delegate = svc;
 
    // Start the generated SFSafariViewController object
    [uvc presentViewController:sfvc animated:YES completion:^{}];
  }

  void Dismiss()
  {
    UIViewController * uvc = UnityGetGLViewController();
    [uvc dismissViewControllerAnimated:YES completion:^{}];
  }
}

