using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace toJSON.Classes.InputEngines
{
    class ExcelEngine : Engine
    {
        XLWorkbook CurrentWorkbook = null;

        public override List<string[]> ProcessData(System.IO.FileInfo fileInfo)
        {
            CurrentWorkbook = new XLWorkbook(fileInfo.FullName);
            IXLWorksheet worksheet = CurrentWorkbook.Worksheet(1);

            List<string[]> rows = new List<string[]>();
            var lastRowUsed = worksheet.LastRowUsed().RowNumber();
            var lastCellUsed = worksheet.LastCellUsed().Address.ColumnNumber;

            for (int r = 0; r < lastRowUsed; r++)
            {
                var row = worksheet.Row(r+1);
                string[] cells = new string[lastCellUsed];
                for (int c = 0; c < lastCellUsed; c++)
                {
                    cells[c] = row.Cell(c+1).Value.ToString();
                    if (cells[c].Equals("N/A")) cells[c] = null;
                }
                rows.Add(cells);
            }

            var filteredRows = rows.Where(r => (r.Any(c => (c != null && c.Trim().Length > 0)))).ToList();

            return filteredRows;
        }

        public override void Shutdown()
        {
            CurrentWorkbook.Dispose();
        }
    }
}
