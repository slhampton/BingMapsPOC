﻿using System;
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

        public MapControl()
        {
            InitializeComponent();
            
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
                ((Button) sender).Tag = $"{center.Latitude},{center.Longitude},{center.Altitude} {zoom}";
            }
            else
            {
                center = (Location) locationConverter.ConvertFrom(tagInfo[0]);
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

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = this.Map.ViewportPointToLocation(mousePosition);

            // The pushpin to add to the map.
            Pushpin pin = new Pushpin
            {
                Location = pinLocation
            };

            // Adds the pushpin to the map.
            this.Map.Children.Add(pin);
        }

        private void MapWithPushpins_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Location location;
            var point = e.GetPosition(this);
            this.Map.TryViewportPointToLocation(point, out location);

            var radius = 12.0;
            var finalImage = new Image();
            finalImage.Width = 40;
            finalImage.Height = 40;
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("car.png", UriKind.Relative);
            logo.EndInit();
            finalImage.Source = logo;

            var tt = new ToolTip();
            tt.Content = "Location = " + location;
            finalImage.ToolTip = tt;
            var p0 = this.Map.LocationToViewportPoint(location);
            var p1 = new Point(p0.X - radius, p0.Y - radius);
            var loc = this.Map.ViewportPointToLocation(p1);
            MapLayer.SetPosition(finalImage, loc);
            this.Map.Children.Add(finalImage);
        }
    }
}
