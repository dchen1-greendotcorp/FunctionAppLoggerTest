using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppLoggerTest.MaskHandlers
{
    public interface IMaskHandler
    {
        List<string> KeyList { get; }
        //void Handle(JProperty jProperty);

        string Mask(string orginal);
    }
}
