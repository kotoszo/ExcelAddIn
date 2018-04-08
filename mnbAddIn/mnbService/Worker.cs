using System;
using System.Data;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using www.mnb.hu.webservices;
using System.Collections.Generic;

namespace mnbService
{
    static class Worker
    {
        public static string[] DateTableColumns { get { return new string[] { "id", "date" }; } }
        public static string[] CurrencyTableColumns { get { return new string[] { "id", "name", "rate" }; } }
        public static string[] ValueTableColumns { get { return new string[] { "currencyId", "dateId", "value" }; } }

        public static int MyProperty { get; set; }
        public static XDocument XmlParser(string text)
        {
            return XDocument.Parse(text);
        }
        public static string StringBuilder(List<string> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in list)
            {
                builder.Append(item);
                if (!item.Equals(list.Last())) { builder.Append(","); }
            }
            return builder.ToString();
        }
        public static DataRow ValueRow(DataRow row, object currId, object dateId, object value)
        {
            row["currencyId"] = currId;
            row["dateId"] = dateId;
            row["value"] = value;
            return row;
        }

        public static DataTable MakeTable(string name)
        {
            DataTable table = new DataTable
            {
                TableName = name
            };
            string[] array;
            switch (name)
            {
                case "Currencies":
                    array = CurrencyTableColumns;
                    break;
                case "Dates":
                    array = DateTableColumns;
                    break;
                case "Values":
                    array = ValueTableColumns;
                    break;
                default:
                    throw new Exception("Something went wrong /MakeTable/");
            }
            foreach (var item in array)
            {
                table.Columns.Add(item);
            }
            return table;
        }

        internal static DataRow CurrencyRow(DataRow row, int id, string name, string rate)
        {
            row["id"] = id;
            row["name"] = name;
            row["rate"] = rate;
            return row;
        }

        internal static DataRow DateRow(DataRow row, int id, string date)
        {
            row["id"] = id;
            row["date"] = date;
            return row;
        }
        internal static GetExchangeRatesRequestBody ExchangeBody(string currency, string startDate, string endDate)
        {
            return new GetExchangeRatesRequestBody
            {
                currencyNames = currency,
                startDate = startDate,
                endDate = endDate
            };
        }
    }
}
