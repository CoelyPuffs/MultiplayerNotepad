using System;
using System.Collections.Generic;
using System.Text;

namespace TCPDataProcessor
{
    abstract class DataProcessor
    {
        abstract public void Process(string msg) { }
    }
}
