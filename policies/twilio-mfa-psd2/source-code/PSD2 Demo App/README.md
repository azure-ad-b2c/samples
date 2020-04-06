---
page_type: sample
description: "The sample covers calling an OpenID Connect identity provider (Azure AD B2C) and acquiring a token from Azure AD B2C using MSAL."
languages:
  - csharp
products:
  - dotnet
  - azure
  - azure-active-directory
---

# Azure AD B2C: Call an ASP.NET Web API from an ASP.NET Web App

This sample contains a solution file that contains two projects: `TaskWebApp` and `TaskService`.

- `TaskWebApp` is a "To-do" ASP.NET MVC web application where the users enters or updates their to-do items.  These CRUD operations are performed by a backend web API. The web app displays the information returned from the ASP.NET Web API.
- `TaskService` is the backend ASP.NET API that manages and stores each user's to-do list.

The sample covers the following:

- Calling an OpenID Connect identity provider (Azure AD B2C)
- Acquiring a token from Azure AD B2C using MSAL

## How To Run This Sample

There are two ways to run this sample:

1. **Using the demo environment** - The sample is already configured to use a demo environment and can be run by downloading this repository and running the app on your machine. Follow the steps listed below in the section [Using the demo environment](#Using-the-demo-environment)
2. **Using your own Azure AD B2C tenant** - Once you have the sample running locally using the demo tenant, you can configure the sample to use your own Azure AD B2C tenant instead. Follow the steps listed below in the section [Using your own Azure AD B2C tenant](#Using-your-own-Azure-AD-B2C-Tenant)

## Using the demo environment

This sample demonstrates how you can sign in or sign up for an account at "Wingtip Toys" (the demo environment for this sample) using a ASP.NET MVC Web Application.

Once singed in, you can create and edit your todo items.

### Step 1: Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi.git`

### Step 2: Run the project

Open the `B2C-WebAPI-DotNet.sln` in Visual Studio. 

You will need to run both the `TaskWebApp` and `TaskService` projects at the same time.

1. In Solution Explorer, right-click on the solution and open the **Common Properties - Startup Project** window.
2. Select **Multiple startup projects**.
3. Change the **Action** for both projects from **None** to **Start** as shown in the image below.

![Visual Studio using multiple startup projects](https://user-images.githubusercontent.com/11529908/30752751-1e8dd9f2-9f72-11e7-9755-c3f5b6b753a5.png)

The sample demonstrates the following functionality once signed-in: 

1. Click your **``<Display Name>``** in upper right corner to edit your profile or reset your password. 
2. Click **Claims** to view the claims associated with the signed-in user's id token. 
3. Click **Todo** to create and view your todo items. These CRUD operations are performed by calling the corresponding Web API running in the solution.
4. Sign out and sign in as a different user. Create tasks for this second user. Notice how the tasks are stored per-user on the API, because the API extracts the user's identity from the access token it receives.

## Using your own Azure AD B2C Tenant

In the previous section, you learned how to run the sample application using the demo environment. In this section, you'll learn how to configure the ASP.NET Web Application and the ASP.NET Web API to work with your own Azure AD B2C Tenant.

### Step 1: Get your own Azure AD B2C tenant

First, you'll need an Azure AD B2C tenant. If you don't have an existing Azure AD B2C tenant that you can use for testing purposes, you can create your own by following these [instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started/).

### Step 2: Create your own policies

This sample uses three types of policies: a unified sign-up/sign-in policy, a profile editing policy, and a password reset policy. Create one policy of each type by following [the built-in policy instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-reference-policies). You may choose to include as many or as few identity providers as you wish.

If you already have existing policies in your Azure AD B2C tenant, feel free to re-use those policies in this sample.

Make sure that all the three policies return **User's Object ID** and **Display Name** on **Application Claims**. To do that, on Azure Portal, go to your B2C Directory then click **User flows (policies)** on the left menu and select your policy. Then click on **Application claims** and make sure that **User's Object ID** and **Display Name** is checked.

### Step 3: Register your ASP.NET Web API with Azure AD B2C

Follow the instructions at [register a Web API with Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-web-api) to register the ASP.NET Web API sample with your tenant. Registering your Web API allows you to define the scopes that your ASP.NET Web Application will request access tokens for.

Provide the following values for the ASP.NET Web API registration:

- Provide a descriptive Name for the ASP.NET Web API, for example, `My Test ASP.NET Web API`. You will identify this application by its Name whenever working in the Azure portal.
- Mark **Yes** for the **Web App/Web API** setting for your application.
- Set the **Reply URL** to `https://localhost:44332/`. This is the port number that this ASP.NET Web API sample is configured to run on. 
- Set the **AppID URI** to `demoapi`. This AppID URI is a unique identifier representing this particular ASP.NET Web API. The AppID URI is used to construct the scopes that are configured in your ASP.NET Web Application. For example, in this ASP.NET Web API sample, the scope will have the value `https://<your-tenant-name>.onmicrosoft.com/demoapi/read`
- Create the application.
- Once the application is created, open your `My Test ASP.NET Web API` application and then open the **Published Scopes** window (in the left nav menu). Add the following 2 scopes:
  - **Scope** named `read` followed by a description `demoing a read scenario`.
  - **Scope** named `write` followed by a description `demoing a write scenario`.
- Click **Save**.

### Step 4: Register your ASP.NET Web Application with Azure AD B2C

Follow the instructions at [register a Web Application with Azure AD B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-dotnet-susi)

Your web application registration should include the following information:

- Provide a descriptive Name for your web appliation, for example, `My Test ASP.NET Web Application`. You can identify this application by its Name within the Azure portal.
- Mark **Yes** for the **Include web app / web API** option.
- Set the Reply URL to `https://localhost:44316/` This is the port number that this ASP.NET Web Application sample is configured to run on. 
- Create your application.
- Once the application is created, you need to create a Web App client secret. Go to the **Keys** page for your Web App registration and click **Generate Key**. Note: You will only see the secret once. Make sure you copy it.
- Open your `My Test ASP.NET Web Application` and open the **API Access** window (in the left nav menu). Click Add and select the name of the Web API you registered previously, for example `My Test ASP.NET Web API`. Select the scope(s) you defined previously, for example, `read` and `write` and hit **Ok**.

### Step 5: Configure your Visual Studio project with your Azure AD B2C app registrations

In this section, you will change the code in both projects to use your tenant. 

:warning: Since both projects have a `Web.config` file, pay close attention which `Web.config` file you are modifying.

#### Step 5a: Modify the `TaskWebApp` project

1. Open the `Web.config` file for the `TaskWebApp` project.
1. Find the key `ida:Tenant` and replace the value with your `<your-tenant-name>.onmicrosoft.com`.
1. Find the key `ida:AadInstance` and replace the value with your `<your-tenant-name>.b2clogin.com`.
1. Find the key `ida:TenantId` and replace the value with your Directory ID.
1. Find the key `ida:ClientId` and replace the value with the Application ID from your web application `My Test ASP.NET Web Application` registration in the Azure portal.
1. Find the key `ida:ClientSecret` and replace the value with the Client secret from your web application in in the Azure portal.
1. Find the keys representing the policies, e.g. `ida:SignUpSignInPolicyId` and replace the values with the corresponding policy names you created, e.g. `b2c_1_SiUpIn`
1. Comment out the aadb2cplayground site and uncomment the `locahost:44332` for the TaskServiceUrl – this is the localhost port that the Web API will run on. Your code should look like the following below.

    ```csharp
    <!--<add key="api:TaskServiceUrl" value="https://aadb2cplayground.azurewebsites.net/" />-->
    
    <add key="api:TaskServiceUrl" value="https://localhost:44332/"/>
    ```

1. Change the `api:ApiIdentifier` key value to the App ID URI of the API you specified in the Web API registration. This App ID URI tells B2C which API your Web Application wants permissions to.

    ```csharp
    <!--<add key="api:ApiIdentifier" value="https://fabrikamb2c.onmicrosoft.com/api/" />—>

    <add key="api:ApiIdentifier" value="https://<your-tenant-name>.onmicrosoft.com/demoapi/" />
    ```

    :memo: Make sure to include the trailing '/' at the end of your `ApiIdentifier` value. 

1. Find the keys representing the scopes, e.g. `api:ReadScope` and replace the values with the corresponding scope names you created, e.g. `read`


#### Step 5b: Modify the `TaskService` project

1. Open the `Web.config` file for the `TaskService` project.
1. Find the key `ida:Tenant` and replace the value with your `<your-tenant-name>.onmicrosoft.com`.
1. Find the key `ida:AadInstance` and replace the value with your `<your-tenant-name>.b2clogin.com`.
1. Find the key `ida:ClientId` and replace the value with the Application ID from your web API `My Test ASP.NET Web API` registration in the Azure portal.
1. Find the key `ida:SignUpSignInPolicyId` and replace the value with the policy name you created, e.g. `b2c_1_SiUpIn`
1. Find the keys representing the scopes, e.g. `api:ReadScope` and `api:WriteScope` and replace the values with the corresponding scope names you created if needed, e.g. `read` and `write`

#### Step 5c: Run both projects

You need to run both projects at the same time. If you did not complete the demo tenant instructions above, you need to [configure Visual Studio for multiple startup projects](#Run-the-project).

You can now perform all the previous steps as seen in the demo tenant environment.

## Known Issues

- MSAL cache needs a TenantId along with the user's ObjectId to function. It retrieves these two from the claims returned in the id_token. As TenantId is not guranteed to be present in id_tokens issued by B2C unless the steps [listed in this document](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/AAD-B2C-specifics#caching-with-b2c-in-msalnet), 
if you are following the workarounds listed in the doc and tenantId claim (tid) is available in the user's token, then please change the code in [ClaimsPrincipalsExtension.cs](https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi/blob/nvalluri-b2c/TaskWebApp/Utils/ClaimsPrincipalExtension.cs) GetB2CMsalAccountId() to let MSAL pick this from the claims instead.

## Next Steps

Customize your user experience further by supporting more identity providers.  Checkout the docs belows to learn how to add additional providers:

[Microsoft](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-msa-app)

[Facebook](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-fb-app)

[Google](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-goog-app)

[Amazon](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-amzn-app)

[LinkedIn](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-li-app)


## Additional information

Additional information regarding this sample can be found in our documentation:

* [How to build a .NET web app using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-dotnet-susi)
* [How to build a .NET web API secured using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-api-dotnet)
* [How to call a .NET web api using a .NET web app](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-api-dotnet)

## Questions & Issues

Please file any questions or problems with the sample as a github issue. You can also post on [StackOverflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) with the tag `azure-ad-b2c`.
