using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;

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
    }
}
