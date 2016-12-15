using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Map.Native;
using DevExpress.XtraMap;

namespace BingMapsPOC
{
    public class Bookmark
    {
        public string Id { get; set; }
        public GeoPoint Coordinates { get; set; }
        public double ZoomLevel { get; set; }
    }
}
