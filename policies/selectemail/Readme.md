# Select an Email from Emails provided via API

This sample policy will retrieve a String Collection from and API of emails associated with a user, it will then use javascript to allow the user to select the desired email. 

The Policy also has a Policy side validation on the email address that has been entered to ensure it is a valid selection.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.


## Policy artifacts
1. The `Get-Emails` Technical Profile will retrieve the claims from rest provider and store them in the `otheremails` claim. This technical profile also calls the `emailstodelimemails` claims transformation, to converted the string collection to a comma seperated list of values.
1. The `SelectEmail` technical profile will then present the comma separated list value to the user and prompt for an input claim called selectedemail. This claim will have a regular expresion restriction to ensure it is an email.

##  User interface changes
In this solution, the comma separated list of values provided to the page as a readonly claim by the REST API will be used to generate the dropdown. CSS will also be used to hide the B2C Elements.

An example of these can be seen in the [selfAssertedDynamicDropDown.cshtml](./html-templates/selfAssertedDynamicDropDown.cshtml) file.

Below is an exert of the javascript from the above file;
```JavaScript
function makeSelect( options) {
    var select = document.createElement( 'select' ),
        optionElem;

    options.split(",").forEach(function( item ) {

        optionElem = document.createElement( 'option' );

        optionElem.value =
            optionElem.textContent =
                item;

        select.appendChild( optionElem );
    });
    select.setAttribute("id", "emailselect");
    return select;
}
var options = $("#emailsdelim")[0].value
$("label[for='selectedemail']").after(makeSelect(options))

$("#selectedemail")[0].value = $("#emailselect option:selected")[0].value

$("#emailselect").change(function() {
$("#selectedemail")[0].value = $("#emailselect option:selected")[0].value
});
```

Also below is an example response from the REST API in the sample (https://selectemailwebapi.azurewebsites.net/api/identity/checkemails)
```JSON
{
    "version":"1.0.0.0",
    "status":200,
    "userMessage":"",
    "emails":
        [
            "test@contoso.com",
            "fake@gmail.com",
            "john@fabrikam.com"
        ]
}
```

## Quick start
1. Modify the policy by replacing all instances of `yourtenant.onmicrosoft.com` with your tenant name.
1. Host the `selfAssertedDynamicDropDown.cshtml` in a [storage account](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-ui-customization#hosting-the-page-content) and [configure CORS](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-ui-customization#3-configure-cors) to allow your b2clogin.com domain.
1. Host the .Net Core 3.0 API at your cloud provider.
1. Modify the `api.selfasserted.selectemail` content definition's `LoadUri` to the HTTP endpoint which hosts your selfAssertedDynamicDropDown.cshtml. 
1. Modify the `Get-Emails` technical profile's `serviceUrl` to point to your REST API endpoint which responds with the JSON payload of the email array.
1. Upload the policy files into your tenant.

## Live version
See a live version [here](https://b2cprod.b2clogin.com/b2cprod.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1A_SignUpOrSignin_MultipleEmailSelect&client_id=51d907f8-db14-4460-a1fd-27eaeb2a74da&nonce=defaultNonce&redirect_uri=https://jwt.ms/&scope=openid&response_type=id_token).

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
