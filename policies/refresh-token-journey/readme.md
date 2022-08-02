# Refresh token journey

Demonstrate how to add a refresh token journey to your custom policy. It allows you to update the return access or ID token claims by reading the user profile from the directory, or calling a REST API services. For more information, check out the comments in the policy files.


To test the policy, follow these steps:

1. Run the following [OAuth 2.0 authorization code flow](https://docs.microsoft.com/azure/active-directory-b2c/authorization-code-flow). Replace your `tenantname` with your tenant name, and `client_id` with your client ID.

    ```http
    https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_Demo_signup_signin_RefreshTokenJourney/oauth2/v2.0/authorize?client_id=00000000-0000-0000-0000-000000000000&nonce=defaultNonce&redirect_uri=https%3A%2F%2Fjwt.ms&scope=openid%20offline_access&response_type=code&prompt=login
    ```

1. Copy the `code` that returns by Azure AD B2C

1. Redeem the [authorization code to an access/ID token](https://docs.microsoft.com/azure/active-directory-b2c/authorization-code-flow#2-get-an-access-token). The following example demostrates how to redeem the authoriztion code to and access/ID token. 

    ```http
    POST https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_Demo_signup_signin_RefreshTokenJourney/oauth2/v2.0/token

    grant_type:authorization_code
    client_secret:<your client secret>
    client_id:<your client ID> 
    scope:openid offline_access
    code:<the authoriztion code that returned by Azure AD B2C>
    ```

1. Examine the access/ID token using <https://jwt.ms> tool
1. Go to your Azure AD B2C tenant and change the user's display name
1. Refresh the token using the following HTTP request

    ```http
    POST https://yourtenant.b2clogin.com/yourtenant.onmicrosoft.com/B2C_1A_Demo_signup_signin_RefreshTokenJourney/oauth2/v2.0/token

    grant_type:refresh_token
    client_secret:<your client secret>
    client_id:<your client ID> 
    scope:openid offline_access
    refresh_token:<your refresh token>  
    ```

1. Examine the access/ID token using <https://jwt.ms> tool. You should see that the display name is updated with the latest value you updated the directory.
