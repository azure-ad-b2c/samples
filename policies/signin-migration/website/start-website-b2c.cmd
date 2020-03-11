@echo off

set IDP_NAME=Azure AD B2C
set B2C_TENANT=yourtenant
set B2C_POLICY=b2c_1a_usrmig_signuporsignin
set PORT=3000
set CLIENT_ID=d6...25
rem remember to scape nasty chars - https://www.robvanderwoude.com/escapechars.php
set CLIENT_SECRET=v2...91
set AUTH_DOMAIN=https://%B2C_TENANT%.b2clogin.com
set AUTHORIZE_PATH=/%B2C_TENANT%.onmicrosoft.com/%B2C_POLICY%/oauth2/v2.0/authorize
set TOKEN_PATH=/%B2C_TENANT%.onmicrosoft.com/%B2C_POLICY%/oauth2/v2.0/token      
set SCOPES=%CLIENT_ID%

set 
rem don't forget to run...
rem npm install

node website.js
