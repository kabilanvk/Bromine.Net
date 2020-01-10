using Bromine.Automation.Core.Helpers;

namespace Bromine.Automation.Core.Enum
{
    public enum EnvironmentType
    {
        [Value("null")]
        Null,
        [Value("QA")]
        QA,
        [Value("Stage")]
        Stage,
        [Value("Beta")]
        Beta,
        [Value("Load")]
        Load,
        [Value("Prod")]
        Prod,
        [Value("Local")]
        Local
    }
}
