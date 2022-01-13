# Azure AD B2C account linkage

With Azure AD B2C an account can have multiple identities, local (username and password) or social/enterprise identity (such as Facebook or AAD). For more information, see [User profile attributes](https://docs.microsoft.com/azure/active-directory-b2c/user-profile-attributes) This Azure AD B2C sample demonstrates how to link and unlink existing Azure AD B2C account to a social identity.

With the link and unlink policy, we append and remove federated identities to the `Identities` collection. Whether it's a federated account, we add new `userIdentitie`, or a local account and we add the first `userIdentities` to the collection. So, an account may look like this one:
```JSON
{
  "displayName": "John Smith",
  "identities": [
    {
      "signInType": "emailAddress",
      "issuer": "contoso.onmicrosoft.com",
      "issuerAssignedId": "jsmith@yahoo.com"
    },
    {
      "signInType": "federated",
      "issuer": "facebook.com",
      "issuerAssignedId": "5eecb0cd"
    },
    {
      "signInType": "federated",
      "issuer": "live.com",
      "issuerAssignedId": "345678"
    }
  ]
}
```


## Link and unlink flow

1. To link a local or federated account to another federated identity, user fist sign-in (with a local or federated account).

1. The policy reads the account form the directory, and checks the value of the `userIdentities` attribute.  The policy extracts the names of the issuer to a string collection. Based on this string collection, the policy show/hide the technical profile. For example:

    - If the collection is empty, user will see the four options to link with: Facebook, Microsoft, Google, and Twitter. While hiding the unlink with Facebook, Microsoft, Google, and Twitter. 
    - After user linked a Facebook account, on the next time user execute the link policy, the user will see link: Microsoft, Google, and Twitter, and unlink Facebook.
1. Whe user clicks on:
    - One of the **link** federated account buttons, which takes the user to the identity provider to complete the sign-in. The policy tries to find such an account in the directory. If found, the policy displays an error message "You facebook.com identity already exists...". If not found, the policy adds the new federated identity to the `userIdenitites` collection and update the account.
    - One of the **unlink** federated account buttons, which takes the user to the federated identity provider to complete the sign-in. After user complete the sign-in with the selected identity provider, the policy removed that issuer from the  `userIdenitites` collection and update the account.

1. Azure AD B2C issues an access token.

The following diagram illustrates the linking flow:

<img alt="An image of sign-in flow between four different stages." src="media/link-flow.png" >

## Technical profiles

For each federated identity provider, there are four technical profiles. For example:

- **Facebook-OAUTH-Base** define the basic functionality to sign-in with Facebook account.
- **Facebook-OAUTH-SignIn** includes the `Facebook-OAUTH-Base` technical profile, and set the output claims, output claims transformation, and session manager to **sign-in** with Facebook.
- **Facebook-OAUTH-Link** includes the `Facebook-OAUTH-Base` technical profile, and set the output claims, output claims transformation, and session manager to sign-in with Facebook to **link** an account.
- **Facebook-Unlink** remove the Facebook issuer from the userIdentities collection.

## Community Help and Support

Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Notes

This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
