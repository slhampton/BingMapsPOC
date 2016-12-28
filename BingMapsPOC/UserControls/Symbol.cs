using System.Drawing;

namespace UserControls
{
    public static class Symbol
    {
        public static string GetImageLocation(string typeId)
        {
            switch (typeId)
            {
                case "car":
                    return "Icons\\Car-48.png";
                case "pcc":
                    return "Icons\\Clinic-48.png";
                case "home":
                    return "Icons\\Home-48.png";
                default:
                    return "Icons\\SUV-48.png";
            }
        }

        public static string GetPOILocation(string typeId)
        {
            switch (typeId)
            {
                case "8060":
                    return "Icons\\Hospital-50.png";
                case "3578":
                    return "Icons\\Bank Cards-48.png";
                case "8211":
                    return "Icons\\School-48.png";
                case "9565":
                    return "Icons\\Pill-48.png";
                default:
                    return "Icons\\Hospital-50.png";
            }
        }
    }
}