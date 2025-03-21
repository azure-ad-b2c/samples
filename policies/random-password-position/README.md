# Authenticate users with only random password positions

It is possible that you need to authenticate users with the username and only specific chars that are requested randomly from the password, for example, only type the chars of three, five, and eight position and not the completed password. Next time, different positions could be requested because they are randoms in base of a fixed password length.

>[!NOTE]
>Please, keep in mind that this is only a sketch. UI customization and javascript code could be necessary.

### REST API
In RESTApi_Authentication you are going to find an Azure Function project (Visual Studio). This project expose two functions:
* _positions_ returns three random password positions that the users should type.
* _checking_ verifies the chars (code) that were typed by the user and the required positions. 

>[!NOTE]
>Explicit password verification is not implemented in _checking_ function because it is not in the scope of this test. Codes are forced to 2, 4, and 6 values. 
>You need to develop your specific function to check that codes.

### Policies
SignUpOrSignIn.xml and SignUpOrSignInWithoutNoAuth.xml are saved in policies folder. The difference between them is that the first one requires a local authentication. The second one does not validate the user so password is not required to the user.
