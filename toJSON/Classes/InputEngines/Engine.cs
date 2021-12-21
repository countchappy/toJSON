using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.InputEngines
{
    abstract class Engine
    {
        public abstract List<string[]> ProcessData(System.IO.FileInfo fileInfo);

        public abstract void Shutdown();
    }
}
