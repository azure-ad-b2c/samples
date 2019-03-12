# Azure AD B2C custom user info endpoint

To use the user info endpoint, you need:

Setting the well-known configuration. Azure AD B2C has an OpenID Connect metadata endpoint, which allows an app to fetch information about Azure AD B2C at runtime. This information includes endpoints, token contents, and token signing keys. There is a JSON metadata document for each user flow in your B2C tenant. For example, the metadata document for the b2c_1_sign_in user flow in your-tenant.onmicrosoft.com is located at:

```
https://your-tenant.b2clogin.com/your-tenant.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=b2c_1_sign_in
```

You need to copy the metadata and paste it in the `wwwroot\.well-known\openid-configuration`. Add follwing line, which should point to your user info endpoint.

```JSON
"userinfo_endpoint" : "https://app-name.azurewebsites.net/api/user_info"
```

Your final metadata should look like similar to the following one:

```JSON
{
  "issuer": "https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/v2.0/",
  "authorization_endpoint": "https://your-tenant.b2clogin.com/your-tenant.onmicrosoft.com/b2c_1_spa_susi/oauth2/v2.0/authorize",
  "token_endpoint": "https://your-tenant.b2clogin.com/your-tenant.onmicrosoft.com/b2c_1_spa_susi/oauth2/v2.0/token",
  "end_session_endpoint": "https://your-tenant.b2clogin.com/your-tenant.onmicrosoft.com/b2c_1_spa_susi/oauth2/v2.0/logout",
  "jwks_uri": "https://your-tenant.b2clogin.com/your-tenant.onmicrosoft.com/b2c_1_spa_susi/discovery/v2.0/keys",
  "userinfo_endpoint" : "https://app-name.azurewebsites.net/api/user_info",
  "response_modes_supported": [
    "query",
    "fragment",
    "form_post"
  ],
....
```

Deploy this app and set your relying party application to this well-known configuration, instead of the Azure AD B2C well-known configuration  