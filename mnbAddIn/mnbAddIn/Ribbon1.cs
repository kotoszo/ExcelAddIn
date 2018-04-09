using popup;
using dbLogger;
using mnbService;
using System.Data;
using System.Collections.Generic;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Excel;

namespace mnbAddIn
{
    public partial class Ribbon1
    {
        string[] header;
        string[] currKeys;
        private Logger _logger;
        private Logger Logger {
            get
            {
                if (_logger == null) { _logger = new Logger(); }
                return _logger;
            }
            set { Logger = value; }
        }
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            header = new string[] { "Dátum/ISO", "Egység" };
            currKeys = new string[] { "name", "rate" };
        }

        private void mnbDownloadClick(object sender, RibbonControlEventArgs e)
        {
            Logger.SaveNewLog();
            DataSet dataSet = GetDataSet();
            Window window = e.Control.Context;
            Worksheet activeWorksheet = ((Worksheet)window.Application.ActiveSheet);

            int currentColumn = 1;
            PrintHeaderAndDates(out int currentRow, currentColumn, activeWorksheet, dataSet.Tables["Dates"]);
            currentColumn = 2;
            foreach (DataRow item in dataSet.Tables["Currencies"].Rows)
            {
                currentRow = 1;
                for (int i = 0; i < currKeys.Length; i++)
                {
                    Printer(currentRow, currentColumn, item[currKeys[i]].ToString(), activeWorksheet, false);
                    currentRow++;
                }
                string expression = "currencyId = '" + item["id"] + "'";
                var values = dataSet.Tables["Values"].Select(expression);
                foreach (var value in values)
                {
                    Printer(currentRow, currentColumn, value["value"].ToString(), activeWorksheet, false);
                    currentRow++;
                }
                currentColumn++;
            }
        }
        private void PrintHeaderAndDates(out int currentRow, 
            int currentColumn, Worksheet activeWorksheet, System.Data.DataTable table)
        {
            currentRow = 1;
            foreach (var item in header)
            {
                Printer(currentRow, currentColumn, item, activeWorksheet, false);
                currentRow++;
            }
            foreach (DataRow item in table.Rows)
            {
                Printer(currentRow, currentColumn, item["date"].ToString(), activeWorksheet, true);
                currentRow++;
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

        private void logButtonClick(object sender, RibbonControlEventArgs e)
        {
            PopUp popUP = new PopUp(Logger.AllSelect());
            if (popUP.IsSaved)
            {
                Logger.Update(popUP.DTable, Columns.reason, popUP.IdsToUpdate);
            }
        }
    }
}
