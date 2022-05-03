# OAuth 2.0 Client credentials flow

The OAuth 2.0 client credentials grant flow permits an app (confidential client) to use its own credentials, instead of impersonating a user, to authenticate when calling web resource, such as REST API. This type of grant is commonly used for server-to-server interactions that must run in the background, without immediate interaction with a user. These types of applications are often referred to as daemons or service accounts. This policy demonstrates how to customize the user journey of the Client credentials flow.

## Building blocks


|Component  |Type  |Notes  |
|---------|---------|---------|
|JwtIssuer |[JWT token issuer technical profile](https://docs.microsoft.com/azure/active-directory-b2c/jwt-issuer-technical-profile)| Point to the the *ClientCredentialsJourney* user journey using the `ClientCredentialsUserJourneyId` metadata. |
|ClientCredentialsJourney| [User journey](https://docs.microsoft.com/azure/active-directory-b2c/userjourneys) | The user journey where you add your custom logic to enrich the access token.|
|ClientCredentials_Setup| Client credentials technical profile | Must be the first orchestration step of the user journey. You can use this technical profile to return claims or run claims transformation. |
|TokenAugmentation| [Claims transformation technical profile](https://docs.microsoft.com/azure/active-directory-b2c/claims-transformation-technical-profile)| Demonstrates how to augment the token by calling a claims transformation. You can use a [RESTful technical profile](https://docs.microsoft.com/azure/active-directory-b2c/restful-technical-profile) to enrich the token claims. |
|SendClaims|[JWT token issuer technical profile](https://docs.microsoft.com/azure/active-directory-b2c/jwt-issuer-technical-profile)| Issue the access token. Must be the last orchestration step of the user journey.|

