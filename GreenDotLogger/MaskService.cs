using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GreenDotLogger;
using System.Linq;

namespace GreenDotLogger
{
    public class MaskService : IMaskService
    {
        private readonly Dictionary<string, IMaskHandler> _handlerDict;
        public MaskService(IEnumerable<IMaskHandler> maskHandlers)
        {
            _handlerDict = GenerateHandlerDict(maskHandlers);
        }

        private Dictionary<string, IMaskHandler> GenerateHandlerDict(IEnumerable<IMaskHandler> maskHandlers)
        {
            Dictionary<string, IMaskHandler> dict = new Dictionary<string, IMaskHandler>();
            foreach (var handler in maskHandlers)
            {
                foreach (var key in handler.KeyList)
                {
                    dict[key.ToLower()] = handler;
                }
            }
            return dict;
        }


        /// <summary>
        /// Use json serializer to generate logging data formate
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
                serialized = JsonConvert.SerializeObject(dict, Formatting.Indented,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            else
            {
                var val = dict[key];
                serialized = JsonConvert.SerializeObject(val, Formatting.Indented,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }

            var jtoken = JToken.Parse(serialized);

            var token = RecursiveMask(jtoken);

            var result = JsonConvert.SerializeObject(token, Formatting.Indented,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

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
                        var property = child as JProperty;
                        if (_handlerDict.ContainsKey(property.Name.ToLower()))
                        {
                            var handler = _handlerDict[property.Name.ToLower()];
                            property.Value = handler.Mask(property.Value.Value<string>());
                        }
                    }
                }
            }
            return jtoken;
        }
    }
}
