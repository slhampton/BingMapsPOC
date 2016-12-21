using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private readonly List<Location> stopLocations = new List<Location>();
        private List<MapLayer> InitialLayers = new List<MapLayer>();
        private Random random = new Random();

        public MapControl()
        {
            InitializeComponent();

            this.routeLayer = new MapLayer() { Tag = "route" };
            this.Map.Children.Add(this.routeLayer);

            this.AddLayers();

            this.Map.Center = new Location(53, -5);
            this.Map.ZoomLevel = 7;
        }

        private void AddLayers()
        {
            InitialLayers.AddRange(new List<MapLayer>
            {
                new MapLayer { Tag = "car" } ,
                new MapLayer { Tag = "pcc" },
                new MapLayer { Tag = "home" }
            });

            foreach (var layer in InitialLayers)
            {
                this.AddPins(layer);
                this.Map.Children.Add(layer);
            }
        }

        private void HideLayer(string layerToRemove)
        {
            foreach (var layer in InitialLayers)
            {
                if (layer.Tag.ToString() == layerToRemove)
                {
                    layer.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ShowLayer(string layerToRemove)
        {
            foreach (var layer in InitialLayers)
            {
                if (layer.Tag.ToString() == layerToRemove)
                {
                    layer.Visibility = Visibility.Visible;
                }
            }
        }

        private void AddPins(MapLayer layer)
        {
            for (var i = 0; i < 3; i++)
            {
                var lat = random.Next(52, 56);
                var lon = random.Next(-7, 0);
                DrawCustomPin(new Location(lat, lon), layer);
            }
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
                AddSymbol(location);
            }
            else
            {
                DrawPin(location);
            }
        }

        private void DrawPin(Location location)
        {
            // The pushpin to add to the map.
            var pin = new Pushpin
            {
                Location = location
            };
            // Adds the pushpin to the map.
            this.Map.Children.Add(pin);
        }

        private void DrawCustomPin(Location location, MapLayer layer)
        {
            const double radius = 20.0;
            var finalImage = new Image
            {
                Width = radius * 2,
                Height = radius * 2
            };
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(Symbol.GetImageLocation(layer.Tag.ToString()), UriKind.Relative);
            logo.EndInit();
            finalImage.Source = logo;

            var tt = new ToolTip { Content = $"CaseNo = {random.Next(10000, 99999)}" };
            finalImage.ToolTip = tt;

            MapLayer.SetPosition(finalImage, location);
            layer.Children.Add(finalImage);
        }

        private void AddSymbol(Location location)
        {
            const double radius = 20.0;
            var finalImage = new Image
            {
                Width = radius * 2,
                Height = radius * 2
            };
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("Icons\\SUV-48.png", UriKind.Relative);
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
            // Check that we have a start and end location
            if (this.startLocation == null)
            {
                MessageBox.Show("A start location is required.");
                return;
            }

            if (this.endLocation == null)
            {
                MessageBox.Show("A end location is required.");
                return;
            }

            // Calculate the route and add to the route layer
            var route = await BingMapsService.PlanRoute(this.startLocation, this.endLocation, this.stopLocations);
            if (route == null)
            {
                MessageBox.Show("Error planing route.");
                return;
            }
            this.routeLayer.Children.Add(route);

            // Show the section of the map relating to the route
            var routeCentre = new LocationRect(route.Locations);
            this.Map.SetView(routeCentre);

            // Zoom out so that you can see the start and end pushpins
            this.Map.ZoomLevel--;
        }

        private void ClearRoute_Click(object sender, RoutedEventArgs e)
        {
            this.startLocation = null;
            this.endLocation = null;
            this.Start.IsEnabled = true;
            this.End.IsEnabled = true;
            this.routeLayer.Children.Clear();
        }

        private void AddStartPoint(object sender, RoutedEventArgs e)
        {
            this.startLocation = this.Map.ViewportPointToLocation(this.pointClicked);

            // The pushpin to add to the map.
            var startPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Green),
                Location = this.startLocation,
                Tag = "start"
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(startPin);

            this.Start.IsEnabled = false;
        }

        private void AddEndPoint(object sender, RoutedEventArgs e)
        {
            this.endLocation = this.Map.ViewportPointToLocation(this.pointClicked);

            // The pushpin to add to the map.
            var endPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Red),
                Location = this.endLocation,
                Tag = "end"
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(endPin);

            this.End.IsEnabled = false;
        }

        private void AddStop(object sender, RoutedEventArgs e)
        {
            var stopLocation = this.Map.ViewportPointToLocation(this.pointClicked);
            this.stopLocations.Add(stopLocation);

            // The pushpin to add to the map.
            var stopPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Gray),
                Location = stopLocation,
                Tag = (stopLocations.Count + 1).ToString()
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(stopPin);

            // We can only have 10 viaWaypoints (https://msdn.microsoft.com/en-us/library/ff701717.aspx)
            if (stopLocations.Count == 10)
            {
                this.Stop.IsEnabled = false;
            }
        }

        private void mouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.pointClicked = e.GetPosition(this);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var layer = ((CheckBox)sender).Tag.ToString();
            HideLayer(layer);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var layer = ((CheckBox)sender).Tag.ToString();
            ShowLayer(layer);
        }
    }
}
