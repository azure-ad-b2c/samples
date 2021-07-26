# Migrating CIAM solution to B2C using Azure Table Storage 

## Setting up 
This example imports users into Azure Table Storage from a CSV file and performs B2C Seamless migration (prev called JIT-migration) of passwords.

## Deploy the Azure Function
First, deploy the Azure Function to Azure. Make sure to do the following
1. Create an Azure Storage Account
1. Create an Azure Function name `TableStorageUser` and deploy file [run.csx](./source-code/run.csx) to it.
1. Create the file [function.proj](./source-code/function.proj) in the Azure Function. This file is needed so that nuget packages will be downloaded correctly. An easy way to create this file is to go to the Console for the Function App, change directory to your function, then do a `copy function.json function.proj`, then go back to the Function App code editor and paste in the right content.  
1. Copy the ConnectString for the storage account and store it as Configuration Setting named `STORAGE_CONNECTSTRING` for the Azure Function.
1. Get the Function Url and update the three powershell [scripts here](./scripts)
 
## Importing users to B2C

Use the scripts in the b2c\scripts folder
 
```Powershell
.\client-cred-login.ps1 -c $env:B2CAppId -s $env:B2CAppKey
.\import-users-from-csv.ps1 -t "yourtenant" -f ..\..\table\scripts\newusers.csv
```

## Importing users to Azure Storage
Edit the file [newusers.csv](newusers.csv) to hold the users you want to import. Make sure to change to emails you are in possesion if you want to do password recovery later in B2C. Also, edit the mobile phone number if you plan to use MFA with B2C. If you just want to test migration, you can stick with `alice@contoso.com`. 

You run the script from a powershell commandline like this
```Powershell
.\save-users-to-tablestorage.ps1 -Path .\newusers.csv -d ";"
```
## Testing that you can retrieve users

To retrieve a user by email, run the below script
```Powershell
.\get-user-from-tablestorage.ps1 -u "alice@contoso.com"
```

To validate username and password, run the below script
```Powershell
.\signin-user-test.ps1
```
## Next step is to migrate users
To migrate users, you should continue [here](../b2c/README.md)
There is one change you need to do and that is to make the B2C policy call the Azure Function you just deployed, which is described below.

### Change B2C Custom Policy 

To let the B2C Custom Policy use Azure Table Storage for Seamless Migration, edit [TrustFrameworkExtensions.xml](https://github.com/azure-ad-b2c/samples/blob/master/policies/signin-migration/b2c/policy/TrustFrameworkExtensions.xml) TechnicalProfile `UserMigrationViaLegacyIdp` to point to the deployed Azure Function above. You only need to change ServiceUrl

```XML
<Item Key="ServiceUrl">https://your-azfunc.azurewebsites.net/api/TableStorageUser?code=....</Item>
```

## Passwords

Passwords are hashed by the Azure Function before being stored in Azure Table Storage to keep a minimal level of security



