# A B2C IEF Custom Policy which allows login via Phone Number (OTP)

## ⚠️ Security Warning — SMS Toll Fraud (IRSF)

**Before deploying phone-based authentication in production, implement SMS toll fraud mitigations.**

Azure AD B2C phone-based authentication sends SMS to all supported countries by default without built-in geo-restriction or spending caps. This creates exposure to [International Revenue Share Fraud (IRSF)](https://en.wikipedia.org/wiki/International_revenue_share_fraud), where attackers programmatically trigger SMS OTP deliveries to premium-rate numbers, generating significant costs.

**Recommended mitigations:**

1. **Restrict country codes** — Use the `countryList` allow-list to limit SMS to countries where your users are located. A ready-to-paste snippet is provided below.
2. **Enable CAPTCHA** — [Add CAPTCHA to sign-up and sign-in](https://learn.microsoft.com/en-us/azure/active-directory-b2c/add-captcha) to prevent automated attacks.
3. **Conditional Access** — [Block sign-ins based on location](https://learn.microsoft.com/en-us/azure/active-directory-b2c/conditional-access-user-flow).
4. **Monitor** — Use the [Azure Monitor workbook for phone authentication failures](https://learn.microsoft.com/en-us/azure/active-directory-b2c/phone-based-mfa) to detect anomalies.

👉 Full mitigation guide: [Phone-based MFA — Mitigate fraudulent sign-ups for custom policy](https://learn.microsoft.com/en-us/azure/active-directory-b2c/phone-based-mfa#mitigate-fraudulent-sign-ups-for-custom-policy)

### `countryList` allow-list — now enforced out-of-the-box

This sample now ships with a **Nordic-only allow-list** (`NO`, `SE`, `DK`, `FI`, + `IS` / `FO` / `AX` on the Nordic localizations). All other country codes are blocked at the UX layer — SMS cannot be triggered to them.

The enforcement lives in **two places**, both edited in this PR:

1. **[`policy/TrustFrameworkLocalization.xml`](policy/TrustFrameworkLocalization.xml)** — `countryList` in the `api.phonefactor.sv` and `api.phonefactor.nb` `LocalizedResources`. The previous full world list (~240 countries) has been split into two commented reference tiers right below the active allow-list:
   - ⚠️ **HIGH-RISK (IRSF / SMS-pumping hotspots)** — must not be uncommented without complementary controls (CAPTCHA, Conditional Access, Azure Monitor anomaly alerts, per-tenant SMS spending cap).
   - Lower-risk (OECD / EU / major commercial markets) — still review before enabling.

2. **[`policy/phone-signup-signin.xml`](policy/phone-signup-signin.xml)** — a new `BuildingBlocks > Localization` block at the top of the relying party policy adds the same Nordic allow-list for the default English UX path, alongside a prepended `api.phonefactor` `ContentDefinition` that wires in `api.phonefactor.en`.

### Action required before production

Extend the `countryList` JSON in both files with the ISO 3166-1 alpha-2 codes of the countries where your users are. **Do not paste a global list** — each entry opens an SMS egress path that an attacker can pivot to.

```xml
<!-- policy/phone-signup-signin.xml — extend this JSON -->
<LocalizedString ElementType="UxElement" StringId="countryList"><![CDATA[{"NO":"Norway","SE":"Sweden","DK":"Denmark","FI":"Finland","IS":"Iceland","FR":"France","US":"United States"}]]></LocalizedString>
```

Apply the same edit to the `api.phonefactor.sv` and `api.phonefactor.nb` entries in `TrustFrameworkLocalization.xml` with localized country names.

---

## Updated version notes
This sample has been updated. The previous version is in the zip file [phone_SUSI_old.zip](policy/phone_SUSI_old.zip) for your conveniance.

The sample still shows Phone Authentication where the user is asked to give their phone number and being challanges by a text mesage (SMS) or voice callback. If the phone number previously didn't exist in the B2C tenant, a user object is created, meaning SignUp is implicit during sign in.

The sample is based on the Base and Extension from the [SocialAndLocalAccountsWithMfa](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa) Starter Pack.
 
### Localization 
The file [TrustFrameworkLocalization.xml](policy/TrustFrameworkLocalization.xml) contains lanugage localizations to Swedish and Norweigan. If you append ``ui_locales=sv-se`` or ``ui_locales=nb-no`` as a query string parameter when you test the policy, the UX will be in Swedish or Norweigan. If you test this in the XCode Simulator for iOS, for example, you need to change ``General > Language & Region > iPhone Language`` in the simulator to switch language

### UX Customization
Since the signin page runs in the browser, styling the UX is something you may want to do to keep the visual style close to your smartphone app. For this purpose you may edit the [TrustFrameworkLocalization.xml](policy/TrustFrameworkLocalization.xml) file in the ContentDefinitions for ``api.selfasserted`` and ``api.phonefactor`` as indicated. Azure supports [hosting UX content in Azure Blob Storage](https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-ui-customization#hosting-the-page-content). Follow those instructions and upload the [selfAsserted.cshtml](policy/html/selfAsserted.cshtml) and the [multifactor-1.0.0.cshtml](policy/html/multifactor-1.0.0.cshtml) files. Before you upload them you need to edit them and replace ***yourtenant*** with your tenant name and ***yourstorageaccountname*** with your Azure Storage account name.

The CSS in the cshtml files are only modified to have a more visible border around the input fields and to add a custom logo.  

The cshtml files also make use of javascript. On the first page [selfAsserted.cshtml](policy/html/selfAsserted.cshtml) The javascript changes the input type of the national phone number field to ``<input type='tel'>`` so that the numeric only keyboard is displayed on the smartphone (see first screenshot). The javascript in the subsequent pages, served by [multifactor-1.0.0.cshtml](policy/html/multifactor-1.0.0.cshtml), we set the ``autocomplete='one-time-code'`` to [enable autofill of the verifiaction code](https://developer.apple.com/documentation/security/password_autofill/enabling_password_autofill_on_an_html_input_element).

### Screenshots

![A Start page mobile phone of policy.](media/phone-susi-00.png) ![A Send code mobile page.](media/phone-susi-02.png) ![A Verify code mobile page with input box.](media/phone-susi-03.png) 

## Phone Authentication Documentation

[Phone Authentication Microsoft docs](https://docs.microsoft.com/en-us/azure/active-directory-b2c/phone-authentication) provides different samples of this implementation.

## Community Help and Support
Use [Stack Overflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) to get support from the community. Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before. Make sure that your questions or comments are tagged with [azure-ad-b2c].
If you find a bug in the sample, please raise the issue on [GitHub Issues](https://github.com/azure-ad-b2c/samples/issues).
To provide product feedback, visit the Azure Active Directory B2C [Feedback page](https://feedback.azure.com/forums/169401-azure-active-directory?category_id=160596).

## Scenario
Where you would like to login users solely on Phone Number and MFA via SMS or Phone Call.
This approach is passwordless.

## Notes
This sample policy is based on [SocialAndLocalAccountsWithMFA starter pack](https://github.com/Azure-Samples/active-directory-b2c-custom-policy-starterpack/tree/master/SocialAndLocalAccountsWithMfa). All changes are marked with **Sample:** comment inside the policy XML files. Make the necessary changes in the **Sample action required** sections. 
