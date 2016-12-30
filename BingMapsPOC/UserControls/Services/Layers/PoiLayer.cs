using System;
using System.Collections.Generic;

namespace UserControls.Services.Layers
{
    public class PoiLayer : BingLayer
    {
        public PoiLayer()
        {
            this.Tag = "poi";
        }

        public async void FindNearby(Point centre)
        {
            this.Children.Clear();
            var centreLocation = new Microsoft.Maps.MapControl.WPF.Location(centre.Coordinates[0], centre.Coordinates[1]);

            // Example entity types corresponding to: School, ATM, Hospital and Pharmacy
            // For more entity types see: https://msdn.microsoft.com/en-us/library/hh478191.aspx
            var entityTypes = new List<string> { "8211", "3578", "8060", "9565" };

            var places = await BingSpatialDataService.FindNearby(centreLocation, entityTypes);

            foreach (var place in places)
            {
                var location = new Microsoft.Maps.MapControl.WPF.Location(Convert.ToDouble(place.Latitude), Convert.ToDouble(place.Longitude));
                DrawCustomPins(location, place.DisplayName, place.EntityTypeID, this);
            }
        }
    }
}
