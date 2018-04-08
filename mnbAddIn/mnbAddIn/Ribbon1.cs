using mnbService;
using System.Data;
using System.Collections.Generic;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Excel;

namespace mnbAddIn
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e) { }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            DataSet dataSet = GetDataSet();
            var currencyTable = dataSet.Tables["Currencies"];
            var dateTable = dataSet.Tables["Dates"];
            var valueTable = dataSet.Tables["Values"];
            Window window = e.Control.Context;
            Worksheet activeWorksheet = ((Worksheet)window.Application.ActiveSheet);

            int currentRow = 1, currentColumn = 1;
            Printer(currentRow, currentColumn, "Dátum/ISO", activeWorksheet, false);
            currentRow++;
            Printer(currentRow, currentColumn, "Egység", activeWorksheet, false);
            currentRow++;
            foreach (DataRow item in dateTable.Rows)
            {
                Printer(currentRow, currentColumn, item["date"].ToString(), activeWorksheet, true);
                currentRow++;
            }
            currentColumn = 2;
            foreach (DataRow item in currencyTable.Rows)
            {
                currentRow = 1;
                Printer(currentRow, currentColumn, item["name"].ToString(), activeWorksheet, false);
                currentRow++;
                Printer(currentRow, currentColumn, item["rate"].ToString(), activeWorksheet, false);
                currentRow++;
                string expression = "currencyId = '" + item["id"] + "'";
                var values = valueTable.Select(expression);
                foreach (var value in values)
                {
                    Printer(currentRow, currentColumn, value["value"].ToString(), activeWorksheet, false);
                    currentRow++;
                }
                currentColumn++;
            }
            
        }
        private void Printer(int row, int column, string item, Worksheet sheet, bool isDate)
        {
            if (isDate)
            {
                item = item.Replace("-", ".");
            }
            ((Range)sheet.Cells[row, column]).Value2 = item;
        }
        private DataSet GetDataSet()
        {
            DataSet dataSet = new DataSet();
            using (Service serv = new Service(2017, 1, 1, 2017, 12, 31))
            {
                List<string> list = serv.Currencies();
                var currencyTable = serv.CurrencyTable(list);
                dataSet.Tables.Add(currencyTable);
                var dateTable = serv.DateTable();
                dataSet.Tables.Add(dateTable);
                var valueTable = serv.ValueTable(currencyTable, dateTable);
                dataSet.Tables.Add(valueTable);
            };
            return dataSet;
        }
    }
}
