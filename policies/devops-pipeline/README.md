# DevOps Pipeline

This example shows how to create a CI/CD pipeline for IEF policies using Azure DevOps.

## Scenario

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
3. 1A_LBASE, 1A_LSBASE
4. 1A_LSS, 1A_LPR
5. 1A_LSSS

<br/>
<br/>

## Local Deployment

To deploy locally, you need to install the cli tool.

### Via npm
```sh
npm install -g ieftool
```

### Via yarn
```sh
yarn global add ieftool
```

Then deploy

```sh
ieftool deploy -t { tenant } -c { client_id } -s { client_secret } -p ./src

```

| option | description |
|--|--|
| tenant | The B2C tenant, this can either be the **tenantId** or the **tenant name** (mytenant.onmicrosoft.com)|
| client_id | The client id of an app registration in B2C that has permissions for TrustFrameworkPolicies |
| client_secret | The client secret of an app registration in B2C that has permissions for TrustFrameworkPolicies |
| source_path | The path to your b2c policies. In the tree structure above it would be ``./src`` 


<br/>
<br/>

## Notes
This sample policy is based on [LocalAccounts starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/LocalAccounts).