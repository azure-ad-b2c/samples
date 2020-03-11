@echo off

set IDP_NAME=AWS Cognito
set PORT=3000
set CLIENT_ID=26...k3
set CLIENT_SECRET=80...u5
set AUTH_DOMAIN=https://your-app-name.auth.your-region.amazoncognito.com
set AUTHORIZE_PATH=/oauth2/authorize
set TOKEN_PATH=/oauth2/token      
set SCOPES=your-app-name-Scopes/demo.read

rem don't forget to run...
rem npm install

node website.js
