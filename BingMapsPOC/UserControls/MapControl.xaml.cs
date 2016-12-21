using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using UserControls.Services;
using Location = Microsoft.Maps.MapControl.WPF.Location;
using Point = System.Windows.Point;

namespace UserControls
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        readonly LocationConverter locationConverter = new LocationConverter();
        private Location endLocation;
        private Location startLocation;
        private readonly MapLayer routeLayer;
        private Point pointClicked;

        public MapControl()
        {
            InitializeComponent();

            this.routeLayer = new MapLayer() { Tag = "route" };
            this.Map.Children.Add(this.routeLayer);

            this.Map.Center = new Location(53, -5);
            this.Map.ZoomLevel = 7;

        }

        private void ChangeMapView_Click(object sender, RoutedEventArgs e)
        {
            // Parse the information of the button's Tag property
            var tagInfo = ((Button)sender).Tag.ToString().Split(' ');
            Location center;
            double zoom;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                center = this.Map.Center;
                zoom = this.Map.ZoomLevel;
                ((Button)sender).Tag = $"{center.Latitude},{center.Longitude},{center.Altitude} {zoom}";
            }
            else
            {
                center = (Location)locationConverter.ConvertFrom(tagInfo[0]);
                zoom = System.Convert.ToDouble(tagInfo[1]);

                // Set the map view
                this.Map.SetView(center, zoom);
            }

        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            this.Map.ZoomLevel++;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            this.Map.ZoomLevel--;
        }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Get the mouse click coordinates
            var point = e.GetPosition(this);

            // Convert the mouse coordinates to a location on the map
            var location = this.Map.ViewportPointToLocation(point);

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                DrawCar(location);
            }
            else
            {
                DrawPin(location);
            }
        }

        private void DrawPin(Location location)
        {
            // The pushpin to add to the map.
            var pin = new Pushpin()
            {
                Location = location
            };
            // Adds the pushpin to the map.
            this.Map.Children.Add(pin);
        }

        private void DrawCar(Location location)
        {
            const double radius = 20.0;
            var finalImage = new Image
            {
                Width = radius * 2,
                Height = radius * 2
            };
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("SUV-48.png", UriKind.Relative);
            logo.EndInit();
            finalImage.Source = logo;

            var tt = new ToolTip { Content = "CaseNo = 12345" };
            finalImage.ToolTip = tt;

            var p0 = this.Map.LocationToViewportPoint(location);
            var p1 = new Point(p0.X - radius, p0.Y - radius);
            var loc = this.Map.ViewportPointToLocation(p1);
            MapLayer.SetPosition(finalImage, loc);
            this.Map.Children.Add(finalImage);
        }

        private async void PlanRoute_Click(object sender, RoutedEventArgs e)
        {
            // Calculate the route and add to the route layer
            var route = await BingMapsService.PlanRoute(this.startLocation, this.endLocation);
            this.routeLayer.Children.Add(route);

            // Show the section of the map relating to the route
            var routeCentre = new LocationRect(route.Locations[0], route.Locations[route.Locations.Count - 1]);
            this.Map.SetView(routeCentre);

            // Zoom out so that you can see the start and end pushpins
            this.Map.ZoomLevel--;
        }

        private async void ClearRoute_Click(object sender, RoutedEventArgs e)
        {
            this.startLocation = null;
            this.endLocation = null;
            this.routeLayer.Children.Clear();
        }

        private void AddStartPoint(object sender, RoutedEventArgs e)
        {
            this.startLocation = this.Map.ViewportPointToLocation(this.pointClicked);

            // The pushpin to add to the map.
            var startPin = new Pushpin()
            {
                Location = this.startLocation,
                Tag = "start"
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(startPin);

            ((MenuItem)sender).IsEnabled = false;
        }

        private void AddEndPoint(object sender, RoutedEventArgs e)
        {
            this.endLocation = this.Map.ViewportPointToLocation(this.pointClicked);

            // The pushpin to add to the map.
            var endPin = new Pushpin()
            {
                Location = this.endLocation,
                Tag = "end"
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(endPin);

            ((MenuItem)sender).IsEnabled = false;
        }

        private void mouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.pointClicked = e.GetPosition(this);
        }
    }
}
