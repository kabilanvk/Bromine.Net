namespace Bromine.Automation.Core.Enum
{
  public enum ActionType
  {
    None,
    // Rest
    SilentRequest,
    // General
    CompareValue,
    GetValueFromCache,
    ClearCacheValue,
    AddTemplate,
    // Selenium
    Click,
    Navigate,
    NavigateBack,
    VerifyUi,
    VerifyUrl,
    Input,
    InputEmail,
    SelectDropdown,
    UlSelectDropdown,
    AddCookie,
    DeleteCookie,
    SwitchWindow,
    Wait,
    WaitUntil,
    ExecuteJavaScript,
    TakeScreenShot,
    CloseWindow,
    VerifyCssProperties,
    VerifyAttributes,
    HandlePrompt,
    VerifyCookie,
    ExtractStringFromUrl,
    DecodeUrl,
    RewriteUrl,
    FetchDataFromUi,
    Refresh,
    AddAuthHeader,
    SwitchContextToIframe,
    // ConsoleLog
    MatchByRegex,
    MatchLog,
    ReadValueFromLog,
  }
}
