# Azure AD B2C: .Net core based custom Azure AD B2C REST API

## Step 1: Create .NET Core web app

1. In Visual Studio, create a project by selecting **File** > **New** > **Project**.
1. In the **New Project** window, select **Visual C#** > **Web** > **.NET Core** > **ASP.NET Core Web Application**.
1. In the **Name** box, type a name for the application (for example, *AADB2C.WebAPI*), and then click **OK**.
1. In the **New SP.NET Core Web Application** window, select an API** template.
1. Make sure that authentication is set to **No Authentication**.
1. Select **OK** to create the project.

## Step 2: Prepare the REST API endpoint
In this step, you copy the files from this GitHub repository and add them to your project. When you add a file, make sure to change the namespace to accommodate your visual studio solution namespace.

### Step 2.1: Add data models
The models represent the input claims and output claims data in your RESTful service. Your code reads the input data by deserializing the input claims model from a JSON string to a C# object (your model). The ASP.NET web API automatically deserializes the output claims model back to JSON and then writes the serialized data to the body of the HTTP response message. 

Create a model that represents input claims by doing the following. If Solution Explorer is not already open, select **View** > **Solution Explorer**. In Solution Explorer, under the **Models** folder, add following classes:
- **B2CResponseModel.cs** this class represents the output JSON data sends back to Azure AD B2C
- **InputClaimsModel.cs** this class represents the input JSON data sends from Azure AD B2C


### Step 2.2: Add a controller
In the web API, a _controller_ is an object that handles HTTP requests. Add following controller **IdentityController.cs**

