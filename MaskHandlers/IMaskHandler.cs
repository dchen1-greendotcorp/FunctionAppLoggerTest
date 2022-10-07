using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppLoggerTest.MaskHandlers
{
    public interface IMaskHandler
    {
        List<string> KeyList { get; }

        string Mask(string orginal);
    }
}
