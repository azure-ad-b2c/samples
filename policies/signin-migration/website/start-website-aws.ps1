
$env:IDP_NAME="AWS Cognito"
$env:PORT=3000
$env:CLIENT_ID="26...k3"
$env:CLIENT_SECRET="80..u5"
$env:AUTH_DOMAIN="https://your-app-name.auth.eu-west-1.amazoncognito.com"
$env:AUTHORIZE_PATH="/oauth2/authorize"
$env:TOKEN_PATH="/oauth2/token"      
$env:SCOPES="your-app-name-Scopes/demo.read"

# don't forget to run...
# npm install

& node website.js
