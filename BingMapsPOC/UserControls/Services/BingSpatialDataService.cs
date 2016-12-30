using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UserControls.Services
{
    public class BingSpatialDataService
    {
        private const string BaseUrl = "http://spatial.virtualearth.net/REST/v1/data";
        private const string BingApiKey = "AsJ2NC7HdNc4k8T03GPuBT5IwzYUJATkSTWfaog7uEJ0lsx_XLS3st1ZStjitTm2";

        // Data Sources
        private const string EuropeDataSource = "NavteqEU";
        private const string EuropeAccessId = "c2ae584bbccc4916a0acf75d1e6947b4";
        private const string TrafficIncidentAccessId = "8F77935E46704C718E45F52D0D5550A6";

        public static async Task<List<POI>> FindNearby(Microsoft.Maps.MapControl.WPF.Location center, List<string> entityTypes)
        {
            var filter = string.Empty;

            if (entityTypes != null)
            {
                var sb = new StringBuilder();
                sb.Append("&$filter=");

                foreach (var type in entityTypes)
                {
                    sb.Append($"EntityTypeId%20EQ%20'{type}'");
                    if (type != entityTypes.Last())
                    {
                        sb.Append("%20OR%20");
                    }
                }
                filter = sb.ToString();
            }

            var uri = new Uri($"{BaseUrl}/{EuropeAccessId}/{EuropeDataSource}/NavteqPOIs?spatialFilter=nearby({center.Latitude}, {center.Longitude}, 1)" +
                              $"&$select=DisplayName,EntityTypeID,EntityID,Latitude,Longitude&$format=json&$top=250{filter}&key={BingApiKey}");

            // Make a request and get the response
            var r = await GetResponse(uri);

            return r.Select(
                poi =>
                new POI
                {
                    DisplayName = poi.DisplayName,
                    EntityTypeID = poi.EntityTypeID,
                    Latitude = poi.Latitude,
                    Longitude = poi.Longitude
                })
                .ToList();
        }

        private static async Task<List<Result>> GetResponse(Uri uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);

            var s = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<JsonResponse>(s);

            return result.ResultSet.Results.ToList();
        }
    }
}
