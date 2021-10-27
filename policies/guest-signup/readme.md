# Guest Signup or Signin

Grant a guest user access over an existing user's directory using a unique identifier and without registering the user in the directory.

A guest user in an application will usually have the following attributes:
* They are invited to the application.
* They will have a single interaction/session with the application.
* They will not return.

We will use a key to identify that specific guest user. 
For instance, an objectId by which we can declare that once they are signed in, they will have access to a specific data set or location in the application. 

### Example:

A cloud storage service for consumers who are mainly interested in easy, yet secure and audited, sharing of access to specific files in their private cloud storage 
with just about anyone who has a phone or mail account.

ContosoCloud users sign-in to the directory with their known credentials.
Guest users are invited with an invite link and follow through this flow:

![Invite flow](./media/invite_flow.png)

[See this post if you want to read more about 'Guest Access using Azure Active Directory B2C custom policies']()