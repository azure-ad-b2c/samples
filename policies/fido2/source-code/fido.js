const base64url = require('base64url');
const cbor = require('cbor');
const uuid = require('uuid-parse');
const jwkToPem = require('jwk-to-pem');
const jwt = require('jsonwebtoken');
const crypto = require('crypto');
const url = require('url');

//Yoel const storage = require('./storage.js');

const hostname = process.env.HOSTNAME || "localhost";
const jwt_secret = process.env.JWT_SECRET || "defaultsecret";


const fido = {};

/**
 * Gets an opaque challenge for the client.
 * Internally, this challenge is a JWT with a timeout.
 * @returns {string} challenge
 */
fido.getChallenge = () => {
    return jwt.sign({}, jwt_secret, {
        expiresIn: 120 * 1000
    });
};

/**
 * Creates a FIDO credential and stores it
 * @param {any} attestation AuthenticatorAttestationResponse received from client
 */
fido.makeCredential = async (attestation) => {
    //https://w3c.github.io/webauthn/#registering-a-new-credential

    if (!attestation.id)
        throw new Error("id is missing");

    if (!attestation.attestationObject)
        throw new Error("attestationObject is missing")

    if (!attestation.clientDataJSON)
        throw new Error("clientDataJSON is missing");

    //Step 1-2: Let C be the parsed the client data claimed as collected during
    //the credential creation
    let C;
    try {
        C = JSON.parse(attestation.clientDataJSON);
    } catch (e) {
        throw new Error("clientDataJSON could not be parsed");
    }

    //Step 3-6: Verify client data
    validateClientData(C, "webauthn.create");
    //Step 7: Compute the hash of response.clientDataJSON using SHA-256.
    const clientDataHash = sha256(attestation.clientDataJSON);

    //Step 8: Perform CBOR decoding on the attestationObject
    let attestationObject;
    try {
        attestationObject = cbor.decodeFirstSync(Buffer.from(attestation.attestationObject, 'base64'));
    } catch (e) {
        throw new Error("attestationObject could not be decoded");
    }
    //Step 8.1: Parse authData data inside the attestationObject
    const authenticatorData = parseAuthenticatorData(attestationObject.authData);
    //Step 8.2: authenticatorData should contain attestedCredentialData
    if (!authenticatorData.attestedCredentialData)
        throw new Exception("Did not see AD flag in authenticatorData");

    //Step 9: Verify that the RP ID hash in authData is indeed the SHA-256 hash
    //of the RP ID expected by the RP.
    if (!authenticatorData.rpIdHash.equals(sha256(hostname))) {
        throw new Error("RPID hash does not match expected value: sha256(" + rpId + ")");
    }

    //Step 10: Verify that the User Present bit of the flags in authData is set
    if ((authenticatorData.flags & 0b00000001) == 0) {
        throw new Error("User Present bit was not set.");
    }

    //Step 11: Verify that the User Verified bit of the flags in authData is set
    if ((authenticatorData.flags & 0b00000100) == 0) {
        throw new Error("User Verified bit was not set.");
    }

    //Steps 12-19 are skipped because this is a sample app.

    //Store the credential
    /*Yoel const credential = await storage.Credentials.create({
        id: authenticatorData.attestedCredentialData.credentialId.toString('base64'),
        publicKeyJwk: authenticatorData.attestedCredentialData.publicKeyJwk,
        signCount: authenticatorData.signCount
    });

    return credential;*/
    return {
        id: authenticatorData.attestedCredentialData.credentialId.toString('base64'),
        publicKeyJwk: authenticatorData.attestedCredentialData.publicKeyJwk,
        test: "ddddd"
    }
};

/**
 * Verifies a FIDO assertion
 * @param {any} assertion AuthenticatorAssertionResponse received from client
 * @return {any} credential object
 */
fido.verifyAssertion = async (assertion) => {

    // https://w3c.github.io/webauthn/#verifying-assertion

    // Step 1 and 2 are skipped because this is a sample app

    // Step 3: Using credential’s id attribute look up the corresponding
    // credential public key.
    /*Yoel let credential = await storage.Credentials.findOne({
        id: assertion.id
    });

    if (!credential) {
        throw new Error("Could not find credential with that ID");
    }*/

    const publicKeyStr = assertion.publicKeyJwk1 + assertion.publicKeyJwk2;
    if (!publicKeyStr)
        throw new Error("Could not read stored credential public key");

    const publicKey = JSON.parse(publicKeyStr);

    // Step 4: Let cData, authData and sig denote the value of credential’s
    // response's clientDataJSON, authenticatorData, and signature respectively
    const cData = assertion.clientDataJSON;
    const authData = Buffer.from(assertion.authenticatorData, 'base64');
    const sig = Buffer.from(assertion.signature, 'base64');

    // Step 5 and 6: Let C be the decoded client data claimed by the signature.
    let C;
    try {
        C = JSON.parse(cData);
    } catch (e) {
        throw new Error("clientDataJSON could not be parsed");
    }
    //Step 7-10: Verify client data
    validateClientData(C, "webauthn.get");

    //Parse authenticator data used for the next few steps
    const authenticatorData = parseAuthenticatorData(authData);

    //Step 11: Verify that the rpIdHash in authData is the SHA-256 hash of the
    //RP ID expected by the Relying Party.
    if (!authenticatorData.rpIdHash.equals(sha256(hostname))) {
        throw new Error("RPID hash does not match expected value: sha256(" + rpId + ")");
    }

    //Step 12: Verify that the User Present bit of the flags in authData is set
    if ((authenticatorData.flags & 0b00000001) == 0) {
        throw new Error("User Present bit was not set.");
    }

    //Step 13: Verify that the User Verified bit of the flags in authData is set
    if ((authenticatorData.flags & 0b00000100) == 0) {
        throw new Error("User Verified bit was not set.");
    }

    //Step 14: Verify that the values of the client extension outputs in
    //clientExtensionResults and the authenticator extension outputs in the
    //extensions in authData are as expected
    if (authenticatorData.extensionData) {
        //We didn't request any extensions. If extensionData is defined, fail.
        throw new Error("Received unexpected extension data");
    }

    //Step 15: Let hash be the result of computing a hash over the cData using
    //SHA-256.
    const hash = sha256(cData);

    //Step 16: Using the credential public key looked up in step 3, verify
    //that sig is a valid signature over the binary concatenation of authData
    //and hash.
    const verify = (publicKey.kty === "RSA") ? crypto.createVerify('RSA-SHA256') : crypto.createVerify('sha256');
    verify.update(authData);
    verify.update(hash);
    if (!verify.verify(jwkToPem(publicKey), sig))
        throw new Error("Could not verify signature");

    //Step 17: verify signCount
    /*if (authenticatorData.signCount != 0 &&
        authenticatorData.signCount < credential.signCount) {
        throw new Error("Received signCount of " + authenticatorData.signCount +
            " expected signCount > " + credential.signCount);
    }*/

    //Update signCount
    /*Yoel
    credential = await storage.Credentials.findOneAndUpdate({
        id: credential.id
    }, {
            signCount: authenticatorData.signCount
        }, { new: true });
    */
   
    //Return credential object that was verified
    return {};
};

/**
 * Parses authData buffer and returns an authenticator data object
 * @param {Buffer} authData
 * @returns {AuthenticatorData} Parsed AuthenticatorData object
 * @typedef {Object} AuthenticatorData
 * @property {Buffer} rpIdHash
 * @property {number} flags
 * @property {number} signCount
 * @property {AttestedCredentialData} attestedCredentialData
 * @property {string} extensionData
 * @typedef {Object} AttestedCredentialData
 * @property {string} aaguid
 * @property {any} publicKeyJwk
 * @property {string} credentialId
 * @property {number} credentialIdLength
 */
const parseAuthenticatorData = authData => {
    try {
        const authenticatorData = {};

        authenticatorData.rpIdHash = authData.slice(0, 32);
        authenticatorData.flags = authData[32];
        authenticatorData.signCount = (authData[33] << 24) | (authData[34] << 16) | (authData[35] << 8) | (authData[36]);

        if (authenticatorData.flags & 64) {
            const attestedCredentialData = {};
            attestedCredentialData.aaguid = uuid.unparse(authData.slice(37, 53)).toUpperCase();
            attestedCredentialData.credentialIdLength = (authData[53] << 8) | authData[54];
            attestedCredentialData.credentialId = authData.slice(55, 55 + attestedCredentialData.credentialIdLength);
            //Public key is the first CBOR element of the remaining buffer
            const publicKeyCoseBuffer = authData.slice(55 + attestedCredentialData.credentialIdLength, authData.length);

            //convert public key to JWK for storage
            attestedCredentialData.publicKeyJwk = coseToJwk(publicKeyCoseBuffer);

            authenticatorData.attestedCredentialData = attestedCredentialData;
        }

        if (authenticatorData.flags & 128) {
            //has extension data

            let extensionDataCbor;

            if (authenticatorData.attestedCredentialData) {
                //if we have attesttestedCredentialData, then extension data is
                //the second element
                extensionDataCbor = cbor.decodeAllSync(authData.slice(55 + authenticatorData.attestedCredentialData.credentialIdLength, authData.length));
                extensionDataCbor = extensionDataCbor[1];
            } else {
                //Else it's the first element
                extensionDataCbor = cbor.decodeFirstSync(authData.slice(37, authData.length));
            }

            authenticatorData.extensionData = cbor.encode(extensionDataCbor).toString('base64');
        }

        return authenticatorData;
    } catch (e) {
        throw new Error("Authenticator Data could not be parsed")
    }
}

/**
 * Validates CollectedClientData
 * @param {any} clientData JSON parsed client data object received from client
 * @param {string} type Operation type: webauthn.create or webauthn.get
 */
const validateClientData = (clientData, type) => {
    if (clientData.type !== type)
        throw new Error("collectedClientData type was expected to be " + type);

    let origin;
    try {
        origin = url.parse(clientData.origin);
    } catch (e) {
        throw new Error("Invalid origin in collectedClientData");
    }

    if (origin.hostname !== hostname)
        throw new Error("Invalid origin in collectedClientData. Expected hostname " + hostname);

    if (hostname !== "localhost" && origin.protocol !== "https:")
        throw new Error("Invalid origin in collectedClientData. Expected HTTPS protocol.");

    let decodedChallenge;
    try {
        decodedChallenge = jwt.verify(base64url.decode(clientData.challenge), jwt_secret);
    } catch (err) {
        throw new Error("Invalid challenge in collectedClientData");
    }
};

/**
 * Converts a COSE key to a JWK
 * @param {Buffer} cose Buffer containing COSE key data
 * @returns {any} JWK object
 */
const coseToJwk = cose => {
    try {
        let publicKeyJwk = {};
        const publicKeyCbor = cbor.decodeFirstSync(cose);

        if (publicKeyCbor.get(3) == -7) {
            publicKeyJwk = {
                kty: "EC",
                crv: "P-256",
                x: publicKeyCbor.get(-2).toString('base64'),
                y: publicKeyCbor.get(-3).toString('base64')
            }
        } else if (publicKeyCbor.get(3) == -257) {
            publicKeyJwk = {
                kty: "RSA",
                n: publicKeyCbor.get(-1).toString('base64'),
                e: publicKeyCbor.get(-2).toString('base64')
            }
        } else {
            throw new Error("Unknown public key algorithm");
        }

        return publicKeyJwk;
    } catch (e) {
        throw new Error("Could not decode COSE Key");
    }
}

/**
 * Evaluates the sha256 hash of a buffer
 * @param {Buffer} data
 * @returns sha256 of the input data
 */
const sha256 = data => {
    const hash = crypto.createHash('sha256');
    hash.update(data);
    return hash.digest();
}

module.exports = fido;

