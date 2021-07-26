# Migrating CIAM solution from OpenLDAP to B2C

## Setting up an OpenLDAP server
This example uses an OpenLDAP docker container that can be found here [https://github.com/wshihadeh/ldap_server](https://github.com/wshihadeh/ldap_server]). If you git clone that project onto a machine that has docker, you can then build it and start it according to the instructions found in the readme file in the github repo.

If you have started the docker container, you can then login to the management portal to see that everything works by navigating your browser to [http://127.0.0.1:8090/](http://127.0.0.1:8090/). The default admin userid is ***cn=admin,dc=shihadeh,dc=intern*** and the password can be found in the [docker-compose.yml](https://github.com/wshihadeh/ldap_server/blob/master/docker-compose.yml) file.

If you are an LDAP wizard, you can modify the [bootstrap.ldif](https://github.com/wshihadeh/ldap_server/blob/master/bootstrap.ldif) file to load the users you like. Otherwise you have 3 users (admin_gh@gmail.com, developer@gmail.com, maintainer@gmail.com) and two groups (Admins, Maintainers).
 
## Exporing the users and groups from the LDAP server
This sample contains the powershell script [export-from-openldap.ps1](https://github.com/azure-ad-b2c/samples/blob/master/policies/signin-migration/ldap/scripts/export-from-openldap.ps1) that makes an LDAP query and exports the users and groups to two different CSV files, named users.csv and groups.csv by default.

You run the script from a powershell commandline like this
```Powershell
.\export-from-openldap.ps1 -LDAPServer "127.0.0.1" -dn "dc=shihadeh,dc=intern" -u "admin" -p ******* -f .\users.csv -g .\groups.csv -d ";"
```

or if you accept all defaults, the short form is this
```Powershell
.\export-from-openldap.ps1 -p ******* 
```

This will produce two CSV files like below. The file format of the CSV files are shared with thw AWS Cognito sample, so therefor you have an ***id*** column that contains nothing. You don't have mobile phone numbers for the users either. Note that this OpenLDAP solution exports the passwords in ***clear text*** (yes, you read correct!), so when we import the users we don't need the JIT-password-migration part.

```CSV
id;userName;emailAddress;emailVerified;accountEnabled;displayName;surname;givenname;mobile;phoneNumberVerified
;developer;developer@gmail.com;True;True;;Developer;developer;;False
;maintainer;maintainer@gmail.com;True;True;;Maintainer;maintainer;;False
;admin_gh;admin_gh@gmail.com;True;True;;AdminGithub;admin_gh;;False
```

The CSV file for groups is organized so that each row contains the group name and one user member. If you have 5 members in the group, your will get 5 rows where the group name is repeated but with a unique username. This is so that importing will be easier to script.

```CSV
id;groupName;member
;Admins;admin
;Maintaners;maintainer
;Maintaners;developer
```

## Importing users to B2C

You initially follow the guidance in the [B2C readme.md](https://github.com/azure-ad-b2c/samples/tree/master/policies/signin-migration/b2c) file up to the point where it says **Migrate the users** (and you don't need to deploy the Azure Function if you just are playing with this sample since we don't need it).

You should make sure you have run this part so you have your access token ready for importing the users and groups.

```Powershell
.\client-cred-login.ps1 -t yourtenant.onmicrosoft.com -c $env:client_id -s $env:client_secret
Environment variable set $env:OAUTH_access_token
Access Token valid until  YYYY-MM-DDTHH:MM:SS
```

```Powershell
PS C:\> .\import-users-from-csv.ps1 -Path .\users.csv -Tenant "yourtenant" -ImportPassword
Creating user: developer / developer@gmail.com
 == 9661b46c-8739-4154-b6b5-3f29e0e13e6f
Creating user: maintainer / maintainer@gmail.com
 == 1873203a-b835-430d-b16a-868850158b19
Creating user: admin_gh / admin_gh@gmail.com
 == 0d15fad9-1be0-4aea-b900-c5cb9510f5fe
Imported 3 users
```

```Powershell
PS C:\> .\import-groups-from-csv.ps1 -Path .\groups.csv -Tenant "yourtenant"
Creating group: Admins
810081b6-8ec6-4228-9bb6-c83d7d858002
User admin not found. Can''t add to group Admins
Creating group: Maintaners
683a4056-c07d-478a-9f90-abdfb4b5dfd4
Add member maintainer to group Maintaners
Add member developer to group Maintaners
3 rows read
Imported 2 groups and 2 members
```

The user ***admin*** is expected not to be found as that user wasn't exported from OpenLDAP as it was the internal admin. Yet it was a member of a group and hence the error.

## Verifying that a user can login

To verify that users can login you can test a B2C policy and signin as admin_gh@gmail.com with a password you can find in the [bootstrap.ldif](https://github.com/wshihadeh/ldap_server/blob/master/bootstrap.ldif) file. 

