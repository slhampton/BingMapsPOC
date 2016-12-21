using System.Drawing;

namespace UserControls
{
    public static class Symbol
    {
        public static string GetImageLocation(string relativeUri)
        {
            switch (relativeUri)
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
    }
}