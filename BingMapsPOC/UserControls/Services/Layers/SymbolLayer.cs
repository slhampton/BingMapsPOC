namespace UserControls.Services.Layers
{
    public class SymbolLayer : BingLayer
    {
        public void AddRandomPins()
        {
            for (var i = 0; i < 3; i++)
            {
                var lat = random.Next(52, 56);
                var lon = random.Next(-7, 0);
                DrawCustomPins(new Microsoft.Maps.MapControl.WPF.Location(lat, lon), null, null, this);
            }
        }
    }
}
