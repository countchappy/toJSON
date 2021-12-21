using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace toJSON.Classes.InputEngines
{
    internal class DelimitedEngine : Engine
    {
        public string Delimiter { get; set; }

        public DelimitedEngine(string Delimiter)
        {
            this.Delimiter = Regex.Unescape(Delimiter);
        }

        public override List<string[]> ProcessData(FileInfo fileInfo)
        {
            var results = new List<string[]>();
            var lines = File.ReadAllLines(fileInfo.FullName);
            foreach (var line in lines)
            {
                var s = Regex.Split(line, Delimiter);
                for(int i = 0; i < s.Length; i++) s[i] = s[i].Trim();
                results.Add(s);
            }
            return results;
        }

        public override void Shutdown()
        {
            return;
        }
    }
}
