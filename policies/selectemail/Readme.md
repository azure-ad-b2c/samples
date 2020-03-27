# Select an Email from Emails provided via API

This sample policy will retrieve a String Collection from and API of emails associated with a user, it will then use javascript to allow the user to select the desired email. 

The Policy also has a Policy side validation on the email address that has been entered to ensure it is a valid selection.

> Note:  This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Demo:** comment inside the policy XML files. Make the necessary changes in the **Demo action required** sections.


## Policy artifacts
1. The `Get-Emails` Technical Profile will retrive the claims from rest provider and store them in the `otheremails` claim. This technical profile also calls the `emailstodelimemails` claims transformation, to converted the string collection to a comma seperated list of values.
1. The `SelectEmail` technical profile will then present the comma seperated list value to the user and prompt for an input claim called selectedemail. This claim will have a regular expresion restriction to ensure it is an email.

##  User interface changes
In this solution, the comma seperated list of values provided to the page as a readonly claim will be used to generate the dropdown. CSS will also be used to hide the B2C Elements.

An example of these can be seen in the [selfAsserted.cshtml](./html-templates/selfAsserted.cshtml) file.

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




## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).
