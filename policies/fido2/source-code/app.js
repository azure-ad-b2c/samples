const express = require("express");
const app = express();
const fido = require('./fido.js');
const bodyParser = require('body-parser');
const enforce = require('express-sslify');

if (process.env.ENFORCE_SSL_HEROKU === "true") {
    app.use(enforce.HTTPS({ trustProtoHeader: true }));
} else if (process.env.ENFORCE_SSL_AZURE === "true") {
    app.use(enforce.HTTPS({ trustAzureHeader: true }));
}
app.use(express.static('public'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));


app.get('/challenge', async (req, res) => {
    try {
        const challenge = await fido.getChallenge();
        res.json({
            result: challenge
        });
    } catch (e) {
        res.json({
            error: e.message
        });
    };
});

app.post('/credentials', async (req, res) => {
    try {
        const credential = await fido.makeCredential(req.body);
        const publicKeyJwkStr = JSON.stringify(credential.publicKeyJwk);
        var publicKeyJwk1 = '';
        var publicKeyJwk2 = '';

        if (publicKeyJwkStr.length > 245 )
        {
            publicKeyJwk1 = publicKeyJwkStr.substr(0,245);
            publicKeyJwk2 = publicKeyJwkStr.substr(245);
        }
        else
        {
            publicKeyJwk1 = publicKeyJwkStr;
        }

        res.json({
            publicKeyJwk: publicKeyJwkStr,
            publicKeyJwk1: publicKeyJwk1,
            publicKeyJwk2: publicKeyJwk2
        });
    } catch (e) {
        res.status(409).json({ version: "1.0", status: 409, userMessage: 'ERROR: ' + e.message });
    }
});

app.post('/assertion', async (req, res) => {
    try {
        const credential = await fido.verifyAssertion(req.body);
        res.json(credential);
    } catch (e) {

        res.status(409).json({ version: "1.0", status: 409, userMessage: 'ERROR (assertion): ' + e.message });
    }
});

app.listen(process.env.PORT || 3000, () => console.log('App launched.'));
