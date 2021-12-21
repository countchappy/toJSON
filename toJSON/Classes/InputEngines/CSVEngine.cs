using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Dynamic;

namespace toJSON.Classes.InputEngines
{
    internal class CSVEngine : Engine
    {
        public override List<string[]> ProcessData(FileInfo fileInfo)
        {
            var rows = new List<dynamic>();
            var results = new List<string[]>();
            using (var reader = new StreamReader(fileInfo.FullName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                rows = csv.GetRecords<dynamic>().ToList();
            }

            List<string> headers = new List<string>();
            foreach (ExpandoObject row in rows)
            {
                List<string> values = new List<string>();
                foreach (KeyValuePair<string, object> cell in row)
                {
                    if (results.Count == 0)
                        headers.Add(cell.Key);
                    values.Add(cell.Value.ToString());
                }
                if (results.Count == 0)
                    results.Add(headers.ToArray());
                results.Add(values.ToArray());
            }

            return results;
        }

        public override void Shutdown()
        {
            return;
        }
    }
}
