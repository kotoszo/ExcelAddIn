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
        public Service()
        {
            MNBArfolyamServiceSoapClient client = new MNBArfolyamServiceSoapClient();
            client.Close();
        }
    }
}
