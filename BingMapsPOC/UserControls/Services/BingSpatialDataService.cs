using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserControls.Services
{
    public class BingSpatialDataService
    {
        private const string BaseUrl = "http://spatial.virtualearth.net/REST/v1";
        private const string BingApiKey = "AsJ2NC7HdNc4k8T03GPuBT5IwzYUJATkSTWfaog7uEJ0lsx_XLS3st1ZStjitTm2";

        // Data Sources
        private const string NavteqeuAccessId = "c2ae584bbccc4916a0acf75d1e6947b4";
        private const string TrafficIncidentAccessId = "8F77935E46704C718E45F52D0D5550A6";

        // Entity types (https://msdn.microsoft.com/en-us/library/hh478191.aspx)
        private const string Hospital = "Hospital";
        private const string Pharmacy = "Pharmacy";
    }
}
