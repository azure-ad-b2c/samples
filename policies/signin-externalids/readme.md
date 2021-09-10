# Sign-in with External SigninNames from REST API

## Scenario
As the [Identities attribute](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/user-profile-attributes#identities-attribute) within Azure AD B2C only supports 10 objectIdentitiy assignments, you may have a business requirement to store more than 10.
This sample demonstrates how you can store multiple signin names for your users within an external service.

## The Azure Function
The Azure Function code in [run.csx](source-code/run.csx) is a sample service to sotre username. THe implementation is a very simple representation and stores usernames and equivelent UPN's witin a dictionary. In an enterprise deployment this would likely be stored within a corporate database.
THe service will expect a `username` value to be passed. This can either be via a query string or JSON body. When retreived it will then return a json payoad with the corresponding UPN. Or if there is no user found it will return an error which will be displayed to the  user.

To deploy this Azure Function, create an Azure Function App and then create a function called `getSignInName` that is a `C# HttpTrigger` with access level `anonymous`. Then paste over the code with [run.csx](source-code/run.csx), make the changes required and save the file. If you decide to call your Azure Function anything else, you need to change the name in the [Policy](policy/SignUpOrSignin_ExtermalUsername.xml#L49).


## Implementation
To implement this use case follow steps below;
1. Ensure you have followed the ["Get Started with custom policies"](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-get-started) steps within the Microsoft documentation site. 
1. Change the refernces in the [Policy](policy/SignUpOrSignin_ExternalUsername.xml) from "yourtenant.onmicrosoft.com" to the name of your B2C Tenant.
1. Update the `REST-GetsignInName` technical profile to reflect your Azure function URLas per the [Microsoft docmentation](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-rest-api-intro).
1. [Uplaod](https://docs.microsoft.com/en-gb/azure/active-directory-b2c/custom-policy-get-started#upload-the-policies) and run your policy.



## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).