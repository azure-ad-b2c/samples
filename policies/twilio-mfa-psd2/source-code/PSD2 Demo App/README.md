# Azure AD B2C PSD2 Demo App

This sample contains a solution file that contains two projects: `TaskWebApp` and `TaskService`.

- `TaskWebApp` is a "To-do" ASP.NET MVC web application where the user can sign in and create a transaction.

The sample covers the following:

- Calling an OpenID Connect identity provider (Azure AD B2C) via Step Up policy

## How To Run This Sample

### Step 1: Get your own Azure AD B2C tenant

First, you'll need an Azure AD B2C tenant. If you don't have an existing Azure AD B2C tenant that you can use for testing purposes, you can create your own by following these [instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started/).

### Step 2: Register your ASP.NET Web Application with Azure AD B2C

Follow the instructions at [register a Web Application with Azure AD B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-dotnet-susi)

Your web application registration should include the following information:

- Provide a descriptive Name for your web appliation, for example, `My Test ASP.NET Web Application`. You can identify this application by its Name within the Azure portal.
- Mark **Yes** for the **Include web app / web API** option.
- Set the Reply URL to `https://localhost:44316/` This is the port number that this ASP.NET Web Application sample is configured to run on. 
- Create your application.
- Once the application is created, you need to create a Web App client secret. Go to the **Keys** page for your Web App registration and click **Generate Key**. Note: You will only see the secret once. Make sure you copy it.
- Open your `My Test ASP.NET Web Application` and open the **API Access** window (in the left nav menu). Click Add and select the name of the Web API you registered previously, for example `My Test ASP.NET Web API`. Select the scope(s) you defined previously, for example, `read` and `write` and hit **Ok**.

### Step 3: Configure your Visual Studio project with your Azure AD B2C app registrations

In this section, you will change the code in both projects to use your tenant. 

:warning: Since both projects have a `Web.config` file, pay close attention which `Web.config` file you are modifying.

#### Step 5a: Modify the `TaskWebApp` project

1. Open the `Web.config` file for the `TaskWebApp` project.
1. Find the key `ida:Tenant` and replace the value with your `<your-tenant-name>.onmicrosoft.com`.
1. Find the key `ida:AadInstance` and replace the value with your `<your-tenant-name>.b2clogin.com`.
1. Find the key `ida:TenantId` and replace the value with your Directory ID.
1. Find the key `ida:ClientId` and replace the value with the Application ID from your web application `My Test ASP.NET Web Application` registration in the Azure portal.
1. Find the key `ida:ClientSecret` and replace the value with the Client secret from your web application in in the Azure portal.

## Additional information

Additional information regarding this sample can be found in our documentation:

* [How to build a .NET web app using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-dotnet-susi)
* [How to build a .NET web API secured using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-api-dotnet)
* [How to call a .NET web api using a .NET web app](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-api-dotnet)

## Questions & Issues

Please file any questions or problems with the sample as a github issue. You can also post on [StackOverflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) with the tag `azure-ad-b2c`.
