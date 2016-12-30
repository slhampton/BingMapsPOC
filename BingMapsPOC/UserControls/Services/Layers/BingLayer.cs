using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;

namespace UserControls.Services.Layers
{
    public class BingLayer : MapLayer
    {
        public readonly Random random = new Random();

        protected void DrawCustomPins(Microsoft.Maps.MapControl.WPF.Location location, string poiName, string typeId, MapLayer layer)
        {
            string toolTip;
            if (poiName != null)
            {
                toolTip = poiName;
            }
            else
            {
                toolTip = $"CaseNo = {random.Next(10000, 99999)}";
            }

            var finalImage = GetImage(toolTip, typeId, layer);

            MapLayer.SetPosition(finalImage, location);
            layer.Children.Add(finalImage);
        }

        private static Image GetImage(string toolTip, string typeId, MapLayer layer)
        {
            const double radius = 20.0;
            var finalImage = new Image
            {
                Width = radius * 2,
                Height = radius * 2
            };

            var logo = new BitmapImage();
            logo.BeginInit();

            logo.UriSource = layer.Tag.ToString() == "poi" ? new Uri(Symbol.GetPOILocation(typeId), UriKind.Relative) : new Uri(Symbol.GetImageLocation(layer.Tag.ToString()), UriKind.Relative);

            logo.EndInit();
            finalImage.Source = logo;

            var tt = new ToolTip { Content = toolTip };
            finalImage.ToolTip = tt;
            return finalImage;
        }
    }
}
