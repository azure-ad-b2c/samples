
$env:IDP_NAME="Azure AD B2C"
$env:B2C_TENANT="yourtenant"
$env:B2C_POLICY="b2c_1a_usrmig_signuporsignin"
$env:PORT="3000"
$env:CLIENT_ID="d6...25"
# escape with `
$env:CLIENT_SECRET="v2...91"
$env:AUTH_DOMAIN="https://$env:B2C_TENANT.b2clogin.com"
$env:AUTHORIZE_PATH="/$env:B2C_TENANT.onmicrosoft.com/$env:B2C_POLICY/oauth2/v2.0/authorize"
$env:TOKEN_PATH="/$env:B2C_TENANT.onmicrosoft.com/$env:B2C_POLICY/oauth2/v2.0/token"
$env:SCOPES="$env:CLIENT_ID"

# don't forget to run...
# npm install

& node website.js
