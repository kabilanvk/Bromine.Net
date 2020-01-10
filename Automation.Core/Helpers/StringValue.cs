using System;

namespace Bromine.Automation.Core.Helpers
{
    public class Value : Attribute
    {
        public string[] V { get; protected set; }

        public Value(string value)
        {
            V = value.Split(',');
        }

        public static string Name(System.Enum value)
        {
            // Get the type
            var type = value.GetType();

            // Get fieldinfo for this type
            var fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(Value), false) as Value[];

            // Return the first if there was a match.
            return (attribs != null && attribs.Length > 0) ? attribs[0].V[0] : null;
        }
    }
}
