
using GreenDotShares;
using Newtonsoft.Json;
using System;

namespace FunctionAppLoggerTest
{
    public class MaskService : IMaskService
    {

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
            string result=string.Empty;

            if(!t.Equals(null))
            {
                result= "David Mask:" + JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented);
            }
            
            if(exception!=null)
            {
                result += $", Exception={JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented)}";
            }
            return result;
        }
    }
}
