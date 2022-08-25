# Azure AD B2C: Measure the time of the user journey

This sample shows how to measure the time takes the user complete the sign-up or sign-in flow. You can change to policy to add a self-asserted technical profile when the `validTimespan` claim is `False`.

The time window is configure in seconds in the _CompareStartAndEndTimes_ claims transformation. For more information, check the comments that start with 'Demo:'.

The sample starts checking the time before the first orchestration step, before the user signs-in. You can remove the first orchestration step and call the _GetEndDateTime_ claims transformation from other technical profiles, such as _SelfAsserted-LocalAccountSignin-Email_ output claims (after the sign-in is completed), or from the _LocalAccountSignUpWithLogonEmail_ input claim (just before the sign-up starts.)