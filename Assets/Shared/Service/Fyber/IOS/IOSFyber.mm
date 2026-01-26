#import <IASDKCore/IASDKCore.h>

extern "C" void Fyber_setGDPRConsentString(const char *gdprConsentString) {
    [IASDKCore.sharedInstance setGDPRConsentString:[NSString stringWithUTF8String:gdprConsentString]];
}

extern "C" void Fyber_setGDPRConsent(const char *v) {
    NSString *str = [NSString stringWithUTF8String:v];
    if ([str isEqualToString: @"true"])
    {
        [IASDKCore.sharedInstance setGDPRConsent:IAGDPRConsentTypeGiven];
    }
    else
    {
        [IASDKCore.sharedInstance setGDPRConsent:IAGDPRConsentTypeDenied];
    }
}

extern "C" void Fyber_setCCPAString(const char *ccpaString) {
    IASDKCore.sharedInstance.CCPAString = [NSString stringWithUTF8String:ccpaString];
}

extern "C" void Fyber_clearCCPAString() {
    IASDKCore.sharedInstance.CCPAString = nil;
}

extern "C" void Fyber_log(const char *logString) {
    NSLog(@"Fyber_log:%@",[NSString stringWithUTF8String:logString]);
}

