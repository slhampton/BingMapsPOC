using Microsoft.Maps.MapControl.WPF;

namespace WPFBingMapPOC
{
    public class Bookmark
    {
        public string Id { get; set; }
        public Location Coordinates { get; set; }
        public double ZoomLevel { get; set; }
    }
}
