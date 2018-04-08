using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using www.mnb.hu.webservices;
using System.Collections.Generic;

namespace mnbService
{
    public class Service : IDisposable
    {
        MNBArfolyamServiceSoapClient client;
        private string startYear, startMonth, startDay;
        private string endYear, endMonth, endDay;
        private string chosenCurrency;
        string RequestEndDate { get { return endYear + "-" + endMonth + "-" + endDay; } }
        string RequestStartDate { get { return startYear + "-" + startMonth + "-" + startDay; } }

        public Service(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            client = new MNBArfolyamServiceSoapClient();
            chosenCurrency = "HUF";
            this.startYear = startYear.ToString();
            this.startMonth = startMonth < 10 ? "0"+startMonth:startMonth.ToString();
            this.startDay = startDay < 10 ? "0" + startDay : startDay.ToString();
            this.endYear = endYear.ToString();
            this.endMonth = endMonth < 10 ? "0" + endMonth : endMonth.ToString();
            this.endDay = endDay < 10 ? "0" + endDay : endDay.ToString();
        }
        public List<string> Currencies()
        {
            List<string> currencies = new List<string>();
            XDocument XInfo = Worker.XmlParser(client.GetInfo(new GetInfoRequestBody()).GetInfoResult);
            foreach (var item in XInfo.Root.Elements("Currencies").Nodes())
            {
                currencies.Add((item as XElement).Value);
            }
            return currencies;
        }
        private string allCurrency;
        public DataTable CurrencyTable(List<string> list)
        {
            DataTable table = Worker.MakeTable("Currencies");
            allCurrency = Worker.StringBuilder(list);
            var req = new GetCurrencyUnitsRequestBody
            {
                currencyNames = allCurrency
            };
            var res = client.GetCurrencyUnits(req);
            XDocument XDoc = Worker.XmlParser(res.GetCurrencyUnitsResult);
            int id = 0;
            foreach (var item in XDoc.Root.Elements().Elements())
            {
                var name = item.Attribute("curr").Value;
                var rate = item.Value;
                table.Rows.Add(Worker.CurrencyRow(table.NewRow(), id, name, rate));
                id++;
            }
            return table;
        }
        public DataTable DateTable()
        {
            DataTable table = Worker.MakeTable("Dates");
            var req = Worker.ExchangeBody(chosenCurrency, RequestStartDate, RequestEndDate);
            var res = client.GetExchangeRates(req);
            XDocument xDoc = Worker.XmlParser(res.GetExchangeRatesResult);
            int id = 0;
            foreach (var item in xDoc.Root.Elements().Reverse())
            {
                table.Rows.Add(Worker.DateRow(table.NewRow(), id, item.Attribute("date").Value));
                id++;
            }
            return table;
        }
        public DataTable ValueTable(DataTable currencyTable, DataTable dateTable)
        {
            DataTable table = Worker.MakeTable("Values");
            var req = Worker.ExchangeBody(allCurrency, RequestStartDate, RequestEndDate);
            var res = client.GetExchangeRates(req);
            XDocument xDoc = Worker.XmlParser(res.GetExchangeRatesResult);
            var doc = xDoc.Root.Elements().Reverse();
            foreach (var item in doc)
            {
                var date = item.Attribute("date").Value;
                string expression1 = "date = '" + date.ToString() + "'";
                var dateId = dateTable.Select(expression1).First()["id"];
                foreach (var jitem in item.Elements())
                {
                    var curr = jitem.Attribute("curr").Value;
                    string expression2 = "name = '" + curr + "'";
                    var currId = currencyTable.Select(expression2).First()["id"];
                    var value = jitem.Value;
                    table.Rows.Add(Worker.ValueRow(table.NewRow(), currId, dateId, value));
                }
            }
            return table;
        }
        public void Dispose()
        {
            client.Close();
        }
    }
}
