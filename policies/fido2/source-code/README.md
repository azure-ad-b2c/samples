# About this sample app
This is a simple NodeJS app that demonstrates the Web Authentication APIs.

You can see a live version at https://webauthnsample.azurewebsites.net

## Deploying a local instance
1. Download and install [NodeJS 8.9 or newer](https://nodejs.org/en/)
2. Download and install [VS Code](https://code.visualstudio.com/)
3. Download and install [MongoDB Community](https://www.mongodb.com/download-center#community)
4. Clone this repository
5. Open this repository in VS Code
6. Run npm install in the root directory
7. Launch program - configurations should already be set
8. In Edge, navigate to localhost:3000

## Deploying to Azure
First, in Azure Portal:

- Create an app services web instance. It must be for Node, and OS Type Linux.
- Create a Cosmos DB instance with API set to mongodb

Before deploying, you'll need to define the following environment variables inside app services application settings so they can be accessed by this NodeJS app at runtime:

- MONGODB_URL - connection URL to your mongodb. Get it from Cosmos DB settings. Pick the latest Node.js 3.0 connection string under Quick Start.
- JWT_SECRET - some long random string, it is used to sign the Challenge JWT sent to the client.
- HOSTNAME - hostname of your B2C tenant (eg: contoso.b2clogin.com)
- ENFORCE_SSL_AZURE - "true"
- WEBSITE_NODE_DEFAULT_VERSION - "8.9.4"

Deploying the code
- Run `npm install` against the project to download the dependancies.
- Use a FTP application to FTP across the entire project to the `wwwroot` folder of the Web App.
- The FTP connection settings are within the Quick Start of the web app.

## Code of Conduct
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
