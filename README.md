# Bromine.Net
A full fledged automation framework with Json based tasks backed with selenium UI framework.

JSON + Selenium + .Net Core + Mozilla FireFox

It has lot of features to run an large suite of tests with detailed logging for troubleshooting. I use it for my enterprise product.

Features like,
* Parallel Run
* Headless browser support with FireFox
* Test summary report
* JSON file based test (No code needed)
* Templating
* Variable substitution
* Browse console logs reading and validation
* Retry tests (configuration)
* etc..

The example tasks are below, will try to add more documentation for all available tasks.

`[
{
"Navigate": {
"Url": "https://www.google.com/"
}
},
{
"Click": {
"Ele": {
"Html element id": "Id"
}
}
},
{
"Input": {
"Ele": [
{
"Type": "Id",
"Expr": "Html element id",
"Input": "Value to populate"
}
]
}
},
{
"VerifyUi": {
"Ele": {
"Html element's xpath": "Xpath"
}
}
}
]`

## Available Task types

**Rest Api call**
* SilentRequest

**General data validation**
* CompareValue
* GetValueFromCache
* ClearCacheValue
* AddTemplate

**Selenium UI**
* Click
* Navigate
* NavigateBack
* VerifyUi
* VerifyUrl
* Input
* InputEmail
* SelectDropdown
* UlSelectDropdown
* AddCookie
* DeleteCookie
* SwitchWindow
* Wait
* WaitUntil
* ExecuteJavaScript
* TakeScreenShot
* CloseWindow
* VerifyCssProperties
* VerifyAttributes
* HandlePrompt
* VerifyCookie
* ExtractStringFromUrl
* DecodeUrl
* RewriteUrl
* FetchDataFromUi
* Refresh
* AddAuthHeader
* SwitchContextToIframe

**Console log validation**
* MatchByRegex
* MatchLog
* ReadValueFromLog