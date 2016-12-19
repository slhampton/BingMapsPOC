﻿using DevExpress.XtraMap;

namespace BingMapsPOC
{
    public class Bookmark
    {
        public string Id { get; set; }
        public GeoPoint Coordinates { get; set; }
        public double ZoomLevel { get; set; }
    }
}
