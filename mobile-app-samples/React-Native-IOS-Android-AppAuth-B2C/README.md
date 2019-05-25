# Secure your React Native Apps with Azure AD B2C

This sample uses Azure AD B2C as an Identity Provider to your react-native apps and adapted from [FormidableLabs](https://github.com/FormidableLabs/react-native-app-auth).

## Getting Started

Refer to [this](https://facebook.github.io/react-native/docs/getting-started) to set up your IOS and Android envionment.

```
git clone https://github.com/azure-ad-b2c/samples
cd samples/mobile-app-samples/React-Native-IOS-Android-AppAuth-B2C/

npm install react-native-app-auth --save
react-native link

npm i styled-components@3.4.9

```

### Android Setup

```
cd android
./gradlew wrapper --gradle-version 4.10.2

```

### Run on Android

```
cd ..
react-native run-android
```

![Alt Text](https://media.giphy.com/media/9x4TVor33c2aX1E3WY/giphy.gif)

### Now add your Azure AD B2C Tenant configuration

#### Add redirect scheme manifest placeholder
To capture the authorization redirect, add the following 'appAuthRedirectScheme' property to the defaultConfig in android/app/build.gradle:

```
android {
  defaultConfig {
    manifestPlaceholders = [
      appAuthRedirectScheme: 'vinu'
    ]
  }
}
```
The scheme is the beginning of your OAuth Redirect URL, up to the scheme separator (:) character.

#### Modify App.js configuration:

```
const config = {
  issuer: 'https://potterworld.b2clogin.com/a5386328-3eba-4dc2-9fdc-44b001832fe5/v2.0/',
  clientId: 'cdb57ca8-3d82-42dc-a810-e6e42490f528',
  // redirectUrl: 'urn.ietf.wg.oauth.2.0.oob://oauthredirect',
  redirectUrl: 'vinu://callback',
  additionalParameters: {},
  scopes: ['openid', 'cdb57ca8-3d82-42dc-a810-e6e42490f528', 'offline_access'],

  serviceConfiguration: {
     authorizationEndpoint: 'https://potterworld.b2clogin.com/potterworld.onmicrosoft.com/b2c_1_vinu/oauth2/v2.0/authorize',
     tokenEndpoint: 'https://potterworld.b2clogin.com/potterworld.onmicrosoft.com/b2c_1_vinu/oauth2/v2.0/token',
     revocationEndpoint: 'https://potterworld.b2clogin.com/potterworld.onmicrosoft.com/b2c_1_vinu/oauth2/v2.0/logout'
   }
};
```

### Run on Android

```
cd ..
react-native run-android
```

### IOS Setup
Copying the IOS Setup from [FormidableLabs](https://github.com/FormidableLabs/react-native-app-auth) for your convenience.

1. [Install native dependencies](https://github.com/FormidableLabs/react-native-app-auth#install-native-dependencies)
2. [Register redirect URL scheme](https://github.com/FormidableLabs/react-native-app-auth#register-redirect-url-scheme)
3. [Define openURL callback in AppDelegate](https://github.com/FormidableLabs/react-native-app-auth#define-openurl-callback-in-appdelegate)

### Run on IOS

```
cd ..
react-native run-ios
```
