using System;
using System.Collections.Generic;
using System.Text;

namespace huypq.Logging
{
    public interface ILogWriter
    {
        void Write(string msg);
    }
}
