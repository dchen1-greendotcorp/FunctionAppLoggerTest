
using FunctionAppLoggerTest.MaskHandlers;
using GreenDotShares;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FunctionAppLoggerTest
{
    public class MaskService : IMaskService
    {
        private readonly List<IMaskHandler> maskHandlers;

        public MaskService(IEnumerable<IMaskHandler> maskHandlers)
        {
            this.maskHandlers = maskHandlers.ToList();
        }
        /// <summary>
        /// Test only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string Mask<T>(T t, Exception exception)
        {
            var data = t as IEnumerable<KeyValuePair<string, object>>;
            if (data == null)
            {
                throw new ArgumentException($"{t} is not correct format.");
            }

            string key = "{OriginalFormat}";
            Dictionary<string, object> dict = new Dictionary<string, object>(data);
            string serialized;
            if (dict.Count > 1)
            {
                dict.Remove(key);
                serialized = JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                var val = dict[key];
                serialized = JsonConvert.SerializeObject(val, Newtonsoft.Json.Formatting.Indented);
            }

            var jtoken = JToken.Parse(serialized);

            var token = RecursiveMask(jtoken);

            var result = JsonConvert.SerializeObject(token, Newtonsoft.Json.Formatting.Indented);

            return result;
        }

        private JToken RecursiveMask(JToken jtoken)
        {
            if (jtoken.Children().Any())
            {
                foreach (var child in jtoken.Children())
                {
                    RecursiveMask(child);

                    if (child.Type == JTokenType.Property)
                    {
                        var property = child as Newtonsoft.Json.Linq.JProperty;
                        var handlers = maskHandlers.Where(c => c.KeyList.Any(d => d.Equals(property.Name, StringComparison.OrdinalIgnoreCase))).ToList();
                        if(handlers.Any())
                        {
                            var handler = handlers.First();
                            property.Value = handler.Mask(property.Value.Value<string>());
                        }
                    }
                }
            }
            return jtoken;
        }
    }
}
