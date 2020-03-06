# Azure AD B2C custom introspection endpoint

As Azure AD B2C utilises JWT based tokens as opposed to opaque tokens there is no requirement to implement an introspection endpoint, however if your application requires an introspection endpoint you can utilise the below code based off the user_info example.

For information on introspection endpoint see the Spec - https://tools.ietf.org/html/rfc7662

To use the introspection endpoint, you need:

Deploy this app and set your relying parties introspection call to  /oauth2/introspect endpoint

NOTE: This is a very basic implementation to verify the expiry date has not been bet (exp). Further enhancments can be made to make a GRAPH call to check the user is still valid.
ALso note as the spec does not identify specific authentication to be used by this endpoint, this demonstration endpoint does not implement authentication, leaving it up to the implementor to select the authentication required.