# IEFTOOL Github Actions

This example shows how to create a CI/CD pipeline for IEF policies using Github Actions which covers two main scenarios. ``Configuration`` and ``Deployment``

## Scenario - Configuration

Azure B2C IEF policies are mainly written in xml and acts like an instruction set for your user flows in Azure B2C. This presents quite a challenge though when you are maintaining multiple tenants representing multiple environments a ``non-prod`` and ``prod`` for example.

The ieftool helps with this scenario by providing a way for you to inject variables into your xml policies using a ``yaml configuration file``


``ieftool.config``
```yaml
tenantId: mynonprodtenant.onmicrosoft.com
deploymentMode: Development
```

``src/BasePolicy.xml``
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<TrustFrameworkPolicy 
    ...
    TenantId="{{ tenantId }}"  
    DeploymentMode="{{ deploymentMode }}">
  ...
</xml>
```
Run the build command

```sh
# ieftool build [source dir] [target dir] -c [config path]
ieftool build src output -c ieftool.config
```

The policies are then compiled into

``output/BasePolicy.xml``
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<TrustFrameworkPolicy 
    ...
    TenantId="mytenant.onmicrosoft.com"  
    DeploymentMode="Development">
  ...
</xml>
```

## Scenario - Deployments

The folder structure of the B2C policies normally doesn't follow a dependency tree and is mainly based on the name of the file. This makes it hard to create a simple bash or pwsh script to upload the policies in the correct order.

This tool makes it easier for B2C policies to be uploaded in-order based on the inheritance of a policy. Uploads are also faster because policies are uploaded by batch depending on its position on the inheritance tree.


```pre
src/
├─ social/
│  ├─ base.xml (1A_SBASE)
│  ├─ signupsignin.xml (1A_SSS)
├─ local/
│  ├─ base.xml (1A_LBASE)
│  ├─ signupsignin.xml (1A_LSS)
│  ├─ passwordreset.xml (1A_LPR)
├─ base.xml (1A_BASE)
├─ extension.xml (1A_EXT)

```

The example folder structure above has the following inheritance tree.

```pre
                1A_BASE
                    |
                 1A_EXT
                /      \
          1A_LBASE    1A_SBASE
           /    \        \      
       1A_LSS  1A_LPR    1A_SSS
```

These policies are then batched by their hierarchy in the tree, as well as their parent policy. The order of upload would then be.

1. 1A_Base
2. 1A_EXT
3. 1A_LBASE, 1A_SBASE
4. 1A_LSS, 1A_LPR
5. 1A_LSSS

<br/>
<br/>

## Credentials

Create an ``Application Registration`` in your Azure B2C tenant, follow [this guide](https://docs.microsoft.com/en-us/azure/active-directory-b2c/microsoft-graph-get-started?tabs=app-reg-ga).

Make sure to grant **Microsoft Graph > Policy > Policy.ReadWrite.TrustFramework** in **API Permissions**

## Setting up Github

To ensure your ``secrets`` stay ``secrets``. The following values should be stored as ``secrets`` in your github repository/organization

|Secret|Description|
|-|-|
|B2C_TENANT_ID|Azure B2C tenant ID|
|B2C_CLIENT_ID|Azure B2C application client ID|
|B2C_CLIENT_SECRET|Azure B2C application client secret|

You can find an example workflow in .github/workflows/deploy.yml

## Local Deployment

To deploy locally, you need to install the cli tool.

### Via curl
```sh
curl https://raw.githubusercontent.com/judedaryl/go-ieftool/main/install.sh | bash
```

### Download the binary

The binaries are available in github [go-ieftool](https://github.com/judedaryl/go-ieftool/releases/latest)

Select the binary for your system. Available binaries:
* darwin-amd64 ( macOS intel chip )
* darwin-arm64 ( macOS m1 chip )
* linux-amd64 ( linux x64 )
* windows-amd64 ( windows x64 )

Then deploy

```sh
export B2C_TENANT_ID=__TENANT_ID__
export B2C_CLIENT_ID=__CLIENT_ID__
export B2C_CLIENT_SECRET=__CLIENT_SECRET__

ieftool deploy ./policy
```

```
Usage:
  ieftool deploy [path to policies]
```

### Required Environment Variables

| variable | description |
|--|--|
| B2C_TENANT_ID | The B2C tenant, this can either be the **tenantId** (guid) or the **tenant name** (mytenant.onmicrosoft.com)|
| B2C_CLIENT_ID | The client id of an app registration in B2C that has permissions for TrustFrameworkPolicies |
| B2C_CLIENT_SECRET | The client secret of an app registration in B2C that has permissions for TrustFrameworkPolicies |


<br/>
<br/>

## Notes
This sample policy is based on [LocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/LocalAccounts).