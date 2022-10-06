using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionAppLoggerTest.MaskHandlers
{
    public class SSNMaskHandler : IMaskHandler
    {
        public List<string> KeyList => new List<string>() { "SSN", "Social" };

        public void Handle(JProperty jProperty)
        {
            if (KeyList.Any(c => jProperty.Name.Equals(c, StringComparison.OrdinalIgnoreCase)))
            {
                jProperty.Value = MaskSSN(jProperty.Value.Value<string>());
            }
        }

        private string MaskSSN(string originalSSN)
        {
            if (originalSSN.Length < 5) return originalSSN;
            var trailingNumbers = originalSSN.Substring(originalSSN.Length - 4);
            var leadingNumbers = originalSSN.Substring(0, originalSSN.Length - 4);
            var maskedLeadingNumbers = Regex.Replace(leadingNumbers, @"[0-9]", "X");
            return maskedLeadingNumbers + trailingNumbers;
        }
    }
}
