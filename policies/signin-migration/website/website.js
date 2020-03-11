'use strict';

const createApplication = require('./');

createApplication(({ app, callbackUrl }) => {

    const oauth2 = require('simple-oauth2').create({
    client: {
      id: process.env.CLIENT_ID,
      secret: process.env.CLIENT_SECRET,
    },
    auth: {
      tokenHost: process.env.AUTH_DOMAIN,
      tokenPath: process.env.TOKEN_PATH,
      authorizePath: process.env.AUTHORIZE_PATH,
    },
    options: {
      authorizationMethod: 'body',
    },
  });

  // Authorization uri definition
  const authorizationUri = oauth2.authorizationCode.authorizeURL({
    redirect_uri: callbackUrl,
    scope: 'openid email profile ' + process.env.SCOPES,
  });

  // Initial page redirecting to Github
  app.get('/auth', (req, res) => {
    console.log(authorizationUri);
    res.redirect(authorizationUri);
  });

  // Callback service parsing the authorization token and asking for the access token
  app.get('/callback', async (req, res) => {
    const { code } = req.query;
    const options = {
      code,
      redirect_uri: callbackUrl,
    };

    try {
      console.log('code: ' + code);
      const result = await oauth2.authorizationCode.getToken(options);

      console.log('The resulting token: ');
      console.log( result );

      console.log("returning 200")
      const jwtMsUrlAT = 'https://jwt.ms/?#access_token=' + result.access_token;
      const jwtMsUrlIT = 'https://jwt.ms/?#id_token=' + result.id_token;
      //return res.status(200).json('Authentication successful');
      return res.send('Authentication successful<br/><br/><a href="' + jwtMsUrlAT + '">View access token with jwt.ms</a><br/><a href="' + jwtMsUrlIT + '">View id token with jwt.ms</a>');
    } catch (error) {
      console.error('Access Token Error', error.message);
      return res.status(500).json('Authentication failed');
    }
  });

  app.get('/', (req, res) => {
    res.send('Hello<br/><a href="/auth">Login with ' + process.env.IDP_NAME + '</a>');
  });
});
