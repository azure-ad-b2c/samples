# Delete my account

This sample policy shows how to delete a local or social account from the directory. To delete an account, user needs to sign-in. The policy checks whether the account exists in the directory (specially for social account that user can sign-in first time). If account exists, the policy presents a warning page and let the user choose to continue. On continue, the policy invokes an Azure AD technical profile that deletes the account and present the account has been deleted message. 

![User flow](media/user-flow.png)


## Disclaimer
The sample app is developed and managed by the open-source community in GitHub. The application is not part of Azure AD B2C product and it's not supported under any Microsoft standard support program or service. 
The app is provided AS IS without warranty of any kind.


## Notes
This sample policy is based on [SocialAndLocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccounts). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
