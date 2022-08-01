using ParkSquare.RealTimeTrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailsIO.RTT
{
    public class ClientConfig : IClientConfig
    {
        public string BaseUrl => "https://api.rtt.io/api/v1/";

        public string Username => "rttapi_zackaryh8";

        public string Password => "";
    }
}
