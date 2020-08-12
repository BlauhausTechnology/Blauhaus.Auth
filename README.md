# Blauhaus.Auth

Refer: 
https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/authentication/azure-ad-b2c

## Azure Setup for mobile app / API. 

1. Create an Azure AD B2C Tenant and associate it with your subscriber. Set up User Flows for sign up / sign in, reset password, and (optional) edit profile.
2. Go to Azure AD B2C | App registrations (making sure the Directory for your AD B2C tenant is set, not the default directory)
3. Create a new App Registration. No redirect URI is required, but do tick the "Grant admin consent to openid and offline_access permissions" box. Save.
4. Click on the App Registration and go to Authentication. Add Platform < Mobile and desktop applications. Select the MSAL client Refirect URI and note it (eg msal2bbeff31-6gcf-487d-86d6-552ec81828c2://auth). Select YES for "Treat application as a public client."
5. Click on Expose an API > Add A Scope. Add an Application ID URI if required. Add a scope called something like Read.And.Write for generic API access. Click the "Grant Admin Access" button. 
6. Click on API permissions > Add A Permission. Choose My APIs  and select the name of your App Registration. Select the Scope you created and click Add Permissions. 

## Server-Side Setup

### Authentication and Authorization

For a new project: Create an ASP.NET Core project, and select the API template. Enable authentication, select "Individual User Accounts, and then choose "Connect to an existing user store in the cloud". Enter the domain (tenantname.onmicrosoft.com), applicationId and sign up policy.

This will install Microsoft.AspNetCore.Authentication.AzureADB2C.UI and add the "AddAuthentication" part to ConfigureServices, and add the necessary fields to appsettings.json. These are added under an "AzureAdB2C" key like so:

* "AzureAdB2C:SignUpSignInPolicyId": "B2C_1_Tenant_SignUp_SignIn",
* "AzureAdB2C:Instance": "https://tenantname.b2clogin.com/tfp/",
* "AzureAdB2C:Domain": "tenantname.onmicrosoft.com",
* "AzureAdB2C:ClientId": "2bbeff31-6fcd-4sdsa-86d6-554ec89828c2",

These can also be added manually and should really go in User Secrets / Azure Key Vault. 

To test it out, fire a request at the server with an HttpHeader of the form:
 "Authorization": "Bearer {AccessTokenFromAzureAdB2C}"}

### Accessing Azure user data via Graph API

//TODO

## App Setup

### Android
In the MainActivity OnCreate() method, after the LoadApplication() call, add the following line:

```c#
AzureAuthenticationClientService.NativeParentView = this;
```

And add the following new method to MainActivity:
```c#
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
    base.OnActivityResult(requestCode, resultCode, data);
    
    AuthenticationContinuationHelper
        .SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
}
``` 

Also register the browser handler Activity in the AndroidManifest.xml file:

```xml
<!-- Activity to handle browser auth-->
<activity android:name="microsoft.identity.client.BrowserTabActivity">
	<intent-filter>
		<action android:name="android.intent.action.VIEW" />
		<category android:name="android.intent.category.DEFAULT" />
		<category android:name="android.intent.category.BROWSABLE" />
		<data android:scheme="msal{ApplicationId}" android:host="auth" />
	</intent-filter>
</activity>
```

### iOS
Open up the Info.plist file, go to the Advanced tab and click Add URL Type. Give it an identifier like "Azure AD B2C" and add the scheme (msal{ApplicationId}) which will look like msal2bbeff31-6gcf-487d-86d6-552ec81828c2. Role can be None. 

Opn Entitlements.plist > Keychain. Enable keychain and provide a group name. Put in $(CFBundleIdentifier) if you want to use the same as the Bundle Id. 

For example these might look like this:

Info.plist:
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

Entitlements.plist:
```xml
<key>keychain-access-groups</key>
<array>
  <string>$(AppIdentifierPrefix)com.app.identifier</string>
</array>
```

Add the following to the AppDelegate:

```c#
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
	AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
	return base.OpenUrl(app, url, options);
}
```

### UWP
No specific setup required.

### General
1. Install the Blauhaus.Auth.Client.Azure package into the App projects (native and shared)
2. Create an AuthConfig file inheriting from AzureActiveDirectoryClientConfig and set the following values:
   * TenantName: tenantname
   * TenantId: tenantname.onmicrosoft.com
   * ApplicationId: Application (client) ID
   * IosKeychainSecurityGroups: The name of the Keychain group added to the iOS Entitlements.plist file. 
   * Scopes: Set to a string array consisting of { "offline_access", "openid", "https://{tenantname}.onmicrosoft.com/{ApplicationIdUri}/Read.And.Write" }, 
   * SigninPolicyName, ResetPasswordPolicyName and EditProfilePolicyName: the names of your User Flows
   
Register the Ioc dependencies:
```c#
services.RegisterAzureAuthenticationClient<AuthConfig>()
```
 

