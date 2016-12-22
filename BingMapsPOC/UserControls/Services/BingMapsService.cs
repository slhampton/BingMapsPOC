using System;
using System.Collections.Generic;
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

        public static async Task<MapPolyline> PlanRoute(Microsoft.Maps.MapControl.WPF.Location startPoint, Microsoft.Maps.MapControl.WPF.Location endPoint, List<Microsoft.Maps.MapControl.WPF.Location> viaWaypointLocations)
        {
            var viaWaypoints = string.Empty;
            var waypointIndex = 1;

            // Add via waypoints
            if (viaWaypointLocations != null)
            {
                foreach (var waypointLocation in viaWaypointLocations)
                {
                    viaWaypoints += $"vwp.{waypointIndex}={waypointLocation}&";
                    // Increment the waypoint / viaWaypoint index
                    waypointIndex++;
                }
            }

            var uri = new Uri($"{BaseUrl}/Routes/Driving?wp.0={startPoint}&{viaWaypoints}wp.{waypointIndex}={endPoint}&rpo=Points&key={BingApiKey}");
            MapPolyline routeLine = null;

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
                    Stroke = new SolidColorBrush(Colors.Green),
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

