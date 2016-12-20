﻿using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace UserControls.Services
{
    public static class BingMapsService
    {
        private const string BaseUrl = "http://dev.virtualearth.net/REST/v1";
        private const string BingApiKey = "AsJ2NC7HdNc4k8T03GPuBT5IwzYUJATkSTWfaog7uEJ0lsx_XLS3st1ZStjitTm2";

        public static Image AddIcon(Location location)
        {
            var uri =
                new Uri(
                    $"{BaseUrl}/Imagery/Map/Road/47.610,-122.107/6?mapSize=100,600&pushpin=47.610,-122.107;40;P10&key={BingApiKey}");


            //using (var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders
            //        .Accept
            //        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            //    var response = client.SendAsync(request).Result;
            //    response.EnsureSuccessStatusCode();

            Image img = null;
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream str = response.GetResponseStream())
                {
                    img = Image.FromStream(str);
                }
            }

            return img;
        }

        public static async Task<MapPolyline> PlanRoute(Microsoft.Maps.MapControl.WPF.Location from, Microsoft.Maps.MapControl.WPF.Location to)
        {
            var uri = new Uri($"{BaseUrl}/Routes/Driving?wp.0={from}&wp.1={to}&rpo=Points&key={BingApiKey}");
            var routeLine = new MapPolyline();

            //Make a request and get the response
            var r = await GetResponse(uri);

            if (r != null &&
                r.ResourceSets != null &&
                r.ResourceSets.Length > 0 &&
                r.ResourceSets[0].Resources != null &&
                r.ResourceSets[0].Resources.Length > 0)
            {
                var route = r.ResourceSets[0].Resources[0] as Route;

                //Get the route line data
                var routePath = route.RoutePath.Line.Coordinates;
                var locations = new LocationCollection();

                foreach (var t in routePath)
                {
                    if (t.Length >= 2)
                    {
                        locations.Add(new Microsoft.Maps.MapControl.WPF.Location(t[0], t[1]));
                    }
                }

                //Create a MapPolyline of the route and add it to the map
                routeLine = new MapPolyline
                {
                    Stroke = new SolidColorBrush(Colors.Blue),
                    StrokeThickness = 5,
                    Opacity = 0.7,
                    Locations = locations
                };
            }

            return routeLine;
        }

        private static async Task<Response> GetResponse(Uri uri)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(uri);

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                return ser.ReadObject(stream) as Response;
            }
        }
    }
}
