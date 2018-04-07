using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using www.mnb.hu.webservices;

namespace mnbService
{
    public class Service
    {
        MNBArfolyamServiceSoapClient client;
        private int startYear, startMonth, startDay;
        private int endYear, endMonth, endDay;
        string chosenCurrency;

        public Service(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            chosenCurrency = "HUF";
            client = new MNBArfolyamServiceSoapClient();

            this.startYear = startYear; this.startMonth = startMonth; this.startDay = startDay;
            this.endYear = endYear; this.endMonth = endMonth; this.endDay = endDay;

            // client.Close();  ///////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }
}
