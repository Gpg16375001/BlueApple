#import <StoreKit/StoreKit.h>

//=================================================================================
//Unity側との連携
//=================================================================================


extern "C" {
    //レビューを催促する
    void _RequestReview(){
        [SKStoreReviewController requestReview];
    }
}