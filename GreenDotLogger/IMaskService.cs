using System;
using System.Text;

namespace GreenDotLogger
{
    public interface IMaskService
    {
        string  Mask<T>(T t, Exception exception);
    }
}
