using System;
using System.Collections.Generic;
using System.Text;

namespace GreenDotLogger
{
    public interface IMaskHandler
    {
        /// <summary>
        /// Each handler should have unique keys
        /// </summary>
        List<string> KeyList { get; }

        string Mask(string orginal);
    }
}
