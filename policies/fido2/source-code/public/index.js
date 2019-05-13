
$(window).on('load', function () {
    $("#register").on('click', () => registerButtonClicked());
    $("#authenticate").on('click', () => authenticateButtonClicked());

    //Update UI to reflect availability of platform authenticator
    if (PublicKeyCredential && typeof PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable !== "function") {
        markPlatformAuthenticatorUnavailable();
    } else if (PublicKeyCredential && typeof PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable === "function") {
        PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable().then(available => {
            if (!available) {
                markPlatformAuthenticatorUnavailable();
            }
        }).catch(e=>{
            markPlatformAuthenticatorUnavailable();
        });
    }
});

/**
 * Marks platform authenticator as unavailable in UI
 */
function markPlatformAuthenticatorUnavailable() {
    $('label[for="attachmentPlatform"]').html('On bound (platform) authenticator <span class="errorText">- Reported as not available</span>');
}

/**
 * Disables all input controls and buttons on the page
 */
function disableControls() {
    $('#register').attr('disabled','');
    $('#authenticate').attr('disabled','');
    $("#status").addClass('hidden');
}

/**
 * Enables all input controls and buttons on the page
 */
function enableControls() {
    $('#register').removeAttr('disabled');
    $('#authenticate').removeAttr('disabled');
    $("#status").removeClass('hidden');
}

/**
 * Handler for create button being pressed
 */
function registerButtonClicked() {
    disableControls();
    $("#registerSpinner").removeClass("hidden");

    getChallenge().then(challenge => {
        return createCredential(challenge);
    }).then(credential => {
        localStorage.setItem("credentialId", credential.id);
        $("#status").text("Successfully created credential with ID: " + credential.id);
        $("#registerSpinner").addClass("hidden");
        enableControls();
    }).catch(e => {
        $("#status").text("Error: " + e);
        $("#registerSpinner").addClass("hidden");
        enableControls();
    });
}

/**
 * Handler for get button being pressed
 */
function authenticateButtonClicked() {
    disableControls();
    $("#authenticateSpinner").removeClass("hidden");

    getChallenge().then(challenge => {
        return getAssertion(challenge);
    }).then(credential => {
        $("#status").text("Successfully verified credential with ID: " + credential.id);
        $("#authenticateSpinner").addClass("hidden");
        enableControls();
    }).catch(e => {
        $("#status").text("Error: " + e);
        $("#authenticateSpinner").addClass("hidden");
        enableControls();
    });
}

/**
 * Retrieves a challenge from the server
 * @returns {Promise} Promise resolving to a ArrayBuffer challenge
 */
function getChallenge() {
    return rest_get(
        "/challenge"
    ).then(response => {
        if (response.error) {
            return Promise.reject(error);
        }
        else {
            var challenge = stringToArrayBuffer(response.result);
            return Promise.resolve(challenge);
        }
    });
}

/**
 * Calls the .create() webauthn APIs and sends returns to server
 * @param {ArrayBuffer} challenge challenge to use
 * @return {any} server response object
 */
function createCredential(challenge) {
    if (!PublicKeyCredential || typeof PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable !== "function")
        return Promise.reject("WebAuthn APIs are not available on this user agent.");

    var attachment = $("input[name='attachment']:checked").val();

    var createCredentialOptions = {
        rp: {
            name: "WebAuthn Sample App",
            icon: "https://example.com/rpIcon.png"
        },
        user: {
            id: stringToArrayBuffer("some.user.id"),
            name: "bob.smith@contoso.com",
            displayName: "Bob Smith",
            icon: "https://example.com/userIcon.png"
        },
        pubKeyCredParams: [
            {
                //External authenticators support the ES256 algorithm
                type: "public-key",
                alg: -7                 
            }, 
            {
                //Windows Hello supports the RS256 algorithm
                type: "public-key",
                alg: -257
            }
        ],
        authenticatorSelection: {
            //Select authenticators that support username-less flows
            requireResidentKey: true,
            //Select authenticators that have a second factor (e.g. PIN, Bio)
            userVerification: "required",
            //Selects between bound or detachable authenticators
            authenticatorAttachment: attachment
        },
        //Since Edge shows UI, it is better to select larger timeout values
        timeout: 50000,
        //an opaque challenge that the authenticator signs over
        challenge: challenge,
        //prevent re-registration by specifying existing credentials here
        excludeCredentials: [],
        //specifies whether you need an attestation statement
        attestation: "none"
    };

    return navigator.credentials.create({
        publicKey: createCredentialOptions
    }).then(rawAttestation => {
        var attestation = {
            id: base64encode(rawAttestation.rawId),
            clientDataJSON: arrayBufferToString(rawAttestation.response.clientDataJSON),
            attestationObject: base64encode(rawAttestation.response.attestationObject)
        };

        console.log("=== Attestation response ===");
        logVariable("id (base64)", attestation.id);
        logVariable("clientDataJSON", attestation.clientDataJSON);
        logVariable("attestationObject (base64)", attestation.attestationObject);

        return rest_put("/credentials", attestation);
    }).then(response => {
        if (response.error) {
            return Promise.reject(response.error);
        } else {
            return Promise.resolve(response.result);
        }
    });
}

/**
 * Calls the .get() API and sends result to server to verify
 * @param {ArrayBuffer} challenge 
 * @return {any} server response object
 */
function getAssertion(challenge) {
    if (!PublicKeyCredential)
        return Promise.reject("WebAuthn APIs are not available on this user agent.");

    var allowCredentials = [];
    var allowCredentialsSelection = $("input[name='allowCredentials']:checked").val();

    if (allowCredentialsSelection === "filled") {
        var credentialId = localStorage.getItem("credentialId");

        if (!credentialId)
            return Promise.reject("Please create a credential first");

        allowCredentials = [{
            type: "public-key",
            id: Uint8Array.from(atob(credentialId), c=>c.charCodeAt(0)).buffer
        }];
    }

    var getAssertionOptions = {
        //specifies which credential IDs are allowed to authenticate the user
        //if empty, any credential can authenticate the users
        allowCredentials: allowCredentials,
        //an opaque challenge that the authenticator signs over
        challenge: challenge,
        //Since Edge shows UI, it is better to select larger timeout values
        timeout: 50000
    };

    return navigator.credentials.get({
        publicKey: getAssertionOptions
    }).then(rawAssertion => {
        var assertion = {
            id: base64encode(rawAssertion.rawId),
            clientDataJSON: arrayBufferToString(rawAssertion.response.clientDataJSON),
            userHandle: base64encode(rawAssertion.response.userHandle),
            signature: base64encode(rawAssertion.response.signature),
            authenticatorData: base64encode(rawAssertion.response.authenticatorData)
        };

        console.log("=== Assertion response ===");
        logVariable("id (base64)", assertion.id);
        logVariable("userHandle (base64)", assertion.userHandle);
        logVariable("authenticatorData (base64)", assertion.authenticatorData);
        logVariable("clientDataJSON", assertion.clientDataJSON);
        logVariable("signature (base64)", assertion.signature);

        return rest_put("/assertion", assertion);
    }).then(response => {
        if (response.error) {
            return Promise.reject(response.error);
        } else {
            return Promise.resolve(response.result);
        }
    });
}

/**
 * Base64 encodes an array buffer
 * @param {ArrayBuffer} arrayBuffer 
 */
function base64encode(arrayBuffer) {
    if (!arrayBuffer || arrayBuffer.length == 0)
        return undefined;

    return btoa(String.fromCharCode.apply(null, new Uint8Array(arrayBuffer)));
}

/**
 * Converts an array buffer to a UTF-8 string
 * @param {ArrayBuffer} arrayBuffer 
 * @returns {string}
 */
function arrayBufferToString(arrayBuffer) {
    return String.fromCharCode.apply(null, new Uint8Array(arrayBuffer));
}

/**
 * Converts a string to an ArrayBuffer
 * @param {string} string string to convert
 * @returns {ArrayBuffer}
 */
function stringToArrayBuffer(str){
    return Uint8Array.from(str, c => c.charCodeAt(0)).buffer;
}
/**
 * Logs a variable to console
 * @param {string} name variable name
 * @param {string} text variable content
 */
function logVariable(name, text) {
    console.log(name + ": " + text);
}

/**
 * Performs an HTTP get operation
 * @param {string} endpoint endpoint URL
 * @returns {Promise} Promise resolving to javascript object received back
 */
function rest_get(endpoint) {
    return fetch(endpoint, {
        method: "GET",
        credentials: "same-origin"
    }).then(response => {
        return response.json();
    });
}

/**
 * Performs an HTTP put operation
 * @param {string} endpoint endpoint URL
 * @param {any} object 
 * @returns {Promise} Promise resolving to javascript object received back
 */
function rest_put(endpoint, object) {
    return fetch(endpoint, {
        method: "PUT",
        credentials: "same-origin",
        body: JSON.stringify(object),
        headers: {
            "content-type": "application/json"
        }
    }).then(response => {
        return response.json();
    });
}
