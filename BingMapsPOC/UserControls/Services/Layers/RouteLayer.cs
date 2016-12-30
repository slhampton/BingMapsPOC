using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace UserControls.Services.Layers
{
    public class RouteLayer : BingLayer
    {
        private MapPolyline routeLine;
        private Microsoft.Maps.MapControl.WPF.Location startLocation;
        private Microsoft.Maps.MapControl.WPF.Location endLocation;
        public List<Microsoft.Maps.MapControl.WPF.Location> StopLocations = new List<Microsoft.Maps.MapControl.WPF.Location>();

        public RouteLayer()
        {
            this.Tag = "route";
        }

        public bool CanPlanRoute => this.startLocation != null && this.endLocation != null;

        public async Task<Route> PlanRoute()
        {
            // Calculate the route
            var route = await BingMapsService.PlanRoute(this.startLocation, this.endLocation, this.StopLocations);

            // Remove any existing route, calculate the new route and add it to the route layer

            this.routeLine = this.DrawRouteLine(route);

            return route;
        }

        public MapPolyline DrawRouteLine(Route route)
        {
            this.Children.Remove(this.routeLine);

            var routePath = route.RoutePath.Line.Coordinates;
            var locations = new LocationCollection();

            foreach (var coordinates in routePath)
            {
                if (coordinates.Length >= 2)
                {
                    locations.Add(new Microsoft.Maps.MapControl.WPF.Location(coordinates[0], coordinates[1]));
                }
            }

            // Create a MapPolyline of the route and add it to the map
            this.routeLine = new MapPolyline
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 5,
                Opacity = 0.7,
                Locations = locations
            };

            this.Children.Add(this.routeLine);

            return this.routeLine;
        }

        public void AddStart(Microsoft.Maps.MapControl.WPF.Location location)
        {
            this.startLocation = location;

            // The pushpin to add to the map.
            var startPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Green),
                Location = this.startLocation
            };

            // Adds the pushpin to the map.
            this.Children.Add(startPin);
        }

        public void AddEnd(Microsoft.Maps.MapControl.WPF.Location location)
        {

            this.endLocation = location;

            // The pushpin to add to the map.
            var endPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Red),
                Location = this.endLocation
            };

            // Adds the pushpin to the map.
            this.Children.Add(endPin);
        }

        public void AddStopLocation(Microsoft.Maps.MapControl.WPF.Location location)
        {
            var stopLocation = location;
            this.StopLocations.Add(stopLocation);

            // The pushpin to add to the map.
            var stopPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Gray),
                Location = stopLocation
            };

            // Adds the pushpin to the map.
            this.Children.Add(stopPin);
        }

        public void ClearRoute()
        {
            this.startLocation = null;
            this.endLocation = null;
            this.StopLocations = new List<Microsoft.Maps.MapControl.WPF.Location>();
            this.routeLine = null;

        }
    }
}
