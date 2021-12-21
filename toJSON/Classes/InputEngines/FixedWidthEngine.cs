using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.InputEngines
{
    internal class FixedWidthEngine : Engine
    {
        private int[] HeaderMapping;

        private int[] RowMapping;

        public override List<string[]> ProcessData(FileInfo fileInfo)
        {
            var lines = File.ReadAllLines(fileInfo.FullName);
            var results = new List<string[]>();
            

            for (var ln = 0; ln < lines.Length; ln++)
            {
                var row = new List<string>();
                var startIndex = 0;
                var l = lines[ln];
                var m = (RowMapping != null && ln > 0) ? RowMapping : HeaderMapping;
                foreach (int i in m)
                {
                    var length = i;
                    if (startIndex + length > l.Length) length = l.Length - startIndex;
                    row.Add(l.Substring(startIndex, length).Trim());
                    startIndex += i;
                }
                results.Add(row.ToArray());
            }
            return results;
        }

        public override void Shutdown()
        {
            return;
        }

        public FixedWidthEngine(FileInfo fixedWidthFile)
        {
            string[] lines = File.ReadAllLines(fixedWidthFile.FullName);
            if (lines.Length > 0)
            {
                HeaderMapping = TransformMapping(lines[0]);
            }
            if (lines.Length > 1)
            {
                RowMapping = TransformMapping(lines[1]);
            }
        }

        private int[] TransformMapping(string line)
        {
            var temp = new List<int>();
            var splits = line.Split(',');
            foreach (var split in splits)
            {
                int s;
                if (int.TryParse(split, out s)) temp.Add(s);
            }
            return temp.ToArray();
        }
    }
}
