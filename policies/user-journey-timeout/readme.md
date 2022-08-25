# Azure AD B2C: Measure the time of the user journey

This sample shows how to measure the time takes the user complete the sign-up or sign-in flow. You can change to policy to add a self-asserted technical profile when the `validTimespan` claim is `False`.

The time window is configure in seconds in the _CompareStartAndEndTimes_ claims transformation. For more information, check the comments that start with 'Demo:'