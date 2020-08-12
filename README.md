# Blauhaus.Auth

In the .NET Standard class library, register the Ioc dependencies:

```c#
services.RegisterAzureAuthenticationClient<AzureAuthConfig>()
```

The AzureAuthConfig file must inherit from AzureActiveDirectoryClientConfig.
TODO: document where the values come from

### On Android

In MainActivity.cs, add the following to OnCreate():

```c#
AzureAuthenticationClientService.NativeParentView = this;
```

And add the following new method:

```c#
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
    base.OnActivityResult(requestCode, resultCode, data);
    
    AuthenticationContinuationHelper
        .SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
}
``` 

In AndroidManifest.xml, add this between the <application> tags:

```xml
<activity android:name="microsoft.identity.client.BrowserTabActivity">
    <intent-filter>
        <action android:name="android.intent.action.VIEW" />
	<category android:name="android.intent.category.DEFAULT" />
	<category android:name="android.intent.category.BROWSABLE" />
	<data android:scheme="msal{client_id}" android:host="auth" />
    </intent-filter>
</activity>
```

# On iOS

Add the following override to the AppDelegate.cs:

```c#
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
    return base.OpenUrl(app, url, options);
}
```

Add a url registration to the Info.plist file:

```xml
<key>CFBundleURLTypes</key>
<array>
   <dict>
    <key>CFBundleURLName</key>
    <string>ADB2C Auth</string>
    <key>CFBundleURLSchemes</key>
    <array>
       <string>msal{client_id}</string>
    </array>
    <key>CFBundleTypeRole</key>
    <string>None</string>
   </dict>
</array>
```

To the Entitlements.plist add:
```xml
<key>keychain-access-groups</key>
<array>
  <string>$(AppIdentifierPrefix)com.app.identifier</string>
</array>
```

Also ensure that Entitlements.plist is designated under "Custom Entitlements" in the iOS Bundle Signing setup screen. 

