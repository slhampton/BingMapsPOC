using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using UserControls.Services;
using UserControls.Services.Layers;
using Image = System.Windows.Controls.Image;
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
        private readonly MapLayer baseLayer;
        private readonly MapLayer drawLayer;
        private readonly RouteLayer routeLayer;
        private readonly PoiLayer poiLayer;
        private Point pointClicked;
        private readonly List<MapLayer> SymbolLayers = new List<MapLayer>();
        private readonly Random random = new Random();
        private bool isDrawing = false;
        private Location center;
        private MapPolygon currentShape;
        private string instructions;
        private Route route;
        private Services.Point instructionPoint;


        public MapControl()
        {
            InitializeComponent();

            this.drawLayer = new MapLayer { Tag = "draw" };
            this.baseLayer = new MapLayer { Tag = "base" };
            this.routeLayer = new RouteLayer();
            this.poiLayer = new PoiLayer();
            this.Map.Children.Add(this.baseLayer);
            this.Map.Children.Add(this.drawLayer);
            this.Map.Children.Add(this.routeLayer);
            this.Map.Children.Add(this.poiLayer);

            this.AddLayers();

            // Could set different default Center and ZoomLevel depending on current region / configuration
            this.Map.Center = new Location(53, -5);
            this.Map.ZoomLevel = 7;
            this.InstructionsColumn.Width = new GridLength(0);
        }

        private void AddLayers()
        {
            var layers = new List<string>()
            {
                "car",
                "pcc",
                "home"
            };

            foreach (var layer in layers)
            {
                var symbolLayer = new SymbolLayer { Tag = layer };
                symbolLayer.AddRandomPins();
                this.Map.Children.Add(symbolLayer);
                this.SymbolLayers.Add(symbolLayer);
            }
        }

        private void HideLayer(string layerToRemove)
        {
            foreach (var layer in SymbolLayers)
            {
                if (layer.Tag.ToString() == layerToRemove)
                {
                    layer.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ShowLayer(string layerToRemove)
        {
            foreach (var layer in SymbolLayers)
            {
                if (layer.Tag.ToString() == layerToRemove)
                {
                    layer.Visibility = Visibility.Visible;
                }
            }
        }

        private void Bookmark_Click(object sender, RoutedEventArgs e)
        {
            // Parse the information of the button's Tag property
            var tagInfo = ((Button)sender).Tag.ToString().Split(' ');
            Location mapCentre;
            double zoom;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                mapCentre = this.Map.Center;
                zoom = this.Map.ZoomLevel;
                ((Button)sender).Tag = $"{mapCentre.Latitude},{mapCentre.Longitude},{mapCentre.Altitude} {zoom}";
            }
            else
            {
                mapCentre = (Location)locationConverter.ConvertFrom(tagInfo[0]);
                zoom = Convert.ToDouble(tagInfo[1]);

                // Set the map view
                this.Map.SetView(mapCentre, zoom);
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                this.Map.Center = this.Map.ViewportPointToLocation(this.pointClicked);
            }
            this.Map.ZoomLevel++;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                this.Map.Center = this.Map.ViewportPointToLocation(this.pointClicked);
            }
            this.Map.ZoomLevel--;
        }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Convert the mouse coordinates to a location on the map
            var location = this.Map.ViewportPointToLocation(e.GetPosition(this.Map));

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                DrawCustomPins(location, null, null, this.baseLayer);
            }
            else
            {
                DrawPin(location);
            }
        }

        private void DrawPin(Location location)
        {
            // The pushpin to add to the map.
            var pin = new Pushpin
            {
                Location = location,
                //Background = new SolidColorBrush(colour)
            };
            // Adds the pushpin to the map.
            this.baseLayer.Children.Add(pin);
        }

        private void DrawCustomPins(Location location, string poiName, string typeId, MapLayer layer)
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

            logo.UriSource = layer.Tag.ToString() == "poi"
                ? new Uri(Symbol.GetPOILocation(typeId), UriKind.Relative)
                : new Uri(Symbol.GetImageLocation(layer.Tag.ToString()), UriKind.Relative);

            logo.EndInit();
            finalImage.Source = logo;

            var tt = new ToolTip { Content = toolTip };
            finalImage.ToolTip = tt;
            return finalImage;
        }

        #region Routing

        private async void PlanRoute_Click(object sender, RoutedEventArgs e)
        {
            // Check that we have a start and end location
            if (!this.routeLayer.CanPlanRoute)
            {
                MessageBox.Show("A start and end location are required.");
                return;
            }

            this.route = await this.routeLayer.PlanRoute();

            if (route == null)
            {
                MessageBox.Show("Error planing route.");
                return;
            }

            this.WriteInstructions(route);

            var routeLine = this.routeLayer.DrawRouteLine(this.route);

            // Show the section of the map relating to the route
            var routeCentre = new LocationRect(routeLine.Locations);
            this.Map.SetView(routeCentre);

            // Zoom out so that you can see all route pushpins
            this.Map.ZoomLevel--;
        }

        private void WriteInstructions(Route route)
        {
            var durationTimeSpan = TimeSpan.FromSeconds(route.TravelDuration);
            var travelDurationAsString = new StringBuilder(string.Empty);
            if (durationTimeSpan.Hours > 0)
            {
                travelDurationAsString.Append($"{durationTimeSpan.Hours} hours, ");
            }
            travelDurationAsString.Append($"{durationTimeSpan.Minutes} minutes");

            var sb = new StringBuilder();

            this.Distance.Text = $"Total distance: {route.TravelDistance} {route.DistanceUnit.ToLower()}";
            this.Duration.Text = $"Total duration: {travelDurationAsString}";

            sb.AppendLine(this.Distance.Text);
            sb.AppendLine(this.Duration.Text);

            RouteResults.DataContext = route;
            foreach (var leg in route.RouteLegs)
            {
                foreach (var item in leg.ItineraryItems)
                {
                    sb.AppendLine($"{item.Instruction.Text} {item.TravelDistance}kms");
                }
            }

            this.instructions = sb.ToString();
            this.InstructionsColumn.Width = new GridLength(225);
        }

        private void ClearRoute_Click(object sender, RoutedEventArgs e)
        {
            this.InstructionsColumn.Width = new GridLength(0);
            this.routeLayer.ClearRoute();
            this.Start.IsEnabled = true;
            this.End.IsEnabled = true;
            this.routeLayer.Children.Clear();
        }

        private void AddRouteStart(object sender, RoutedEventArgs e)
        {
            this.routeLayer.AddStart(this.Map.ViewportPointToLocation(this.pointClicked));
            this.Start.IsEnabled = false;
        }

        private void AddRouteEnd(object sender, RoutedEventArgs e)
        {
            this.routeLayer.AddEnd(this.Map.ViewportPointToLocation(this.pointClicked));
            this.End.IsEnabled = false;
        }

        private void AddRouteStop(object sender, RoutedEventArgs e)
        {
            this.routeLayer.AddStopLocation(this.Map.ViewportPointToLocation(this.pointClicked));

            // We can only have 10 viaWaypoints (https://msdn.microsoft.com/en-us/library/ff701717.aspx)
            if (this.routeLayer.StopLocations.Count == 10)
            {
                this.Stop.IsEnabled = false;
            }
        }

        #endregion

        private void PointSelected(object sender, MouseButtonEventArgs e)
        {
            this.pointClicked = e.GetPosition(this.Map);
        }

        private void mouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing)
            {
                return;
            }

            isDrawing = false;

            this.Map.MouseLeftButtonDown -= MouseTouchStartHandler;
            this.Map.MouseMove -= MouseTouchMoveHandler;
            this.Map.MouseLeftButtonUp -= MouseTouchEndHandler;
            this.Map.ViewChangeOnFrame -= ViewChangeOnFrame;

            this.ZoomToArea(this.currentShape);
        }

        private void ZoomToArea(MapPolygon rectangle)
        {
            var routeCentre = new LocationRect(currentShape.Locations);
            this.Map.SetView(routeCentre);
            this.drawLayer.Children.Clear();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var layer = ((CheckBox)sender).Tag.ToString();
            HideLayer(layer);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var layer = ((CheckBox)sender).Tag.ToString();
            ShowLayer(layer);
        }

        private void DrawArea_Click(object sender, RoutedEventArgs e)
        {
            // Capture the current center of the map. We will use this to lock the map view.
            center = this.Map.Center;

            this.Map.MouseLeftButtonDown += MouseTouchStartHandler;
            this.Map.MouseMove += MouseTouchMoveHandler;
            this.Map.MouseLeftButtonUp += MouseTouchEndHandler;
            this.Map.ViewChangeOnFrame += ViewChangeOnFrame;
        }

        private void MouseTouchStartHandler(object sender, MouseButtonEventArgs e)
        {
            var startLoc = GetMouseTouchLocation(e);

            this.drawLayer.Children.Clear();

            if (startLoc != null)
            {
                //Create a polygon that has four corners, all of which are the starting location.
                currentShape = new MapPolygon()
                {
                    Locations = new LocationCollection()
                    {
                        startLoc,
                        startLoc,
                        startLoc,
                        startLoc
                    },
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeThickness = 2
                };

                this.drawLayer.Children.Add(currentShape);

                isDrawing = true;
            }
        }

        private void MouseTouchMoveHandler(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                var currentLoc = GetMouseTouchLocation(e);
                if (currentLoc != null)
                {
                    var firstLoc = currentShape.Locations[0];

                    // Update locations 1 - 3 of polygon so as to create a rectangle.
                    currentShape.Locations[1] = new Location(firstLoc.Latitude, currentLoc.Longitude);
                    currentShape.Locations[2] = currentLoc;
                    currentShape.Locations[3] = new Location(currentLoc.Latitude, firstLoc.Longitude);
                }
            }
        }

        private void MouseTouchEndHandler(object sender, MouseButtonEventArgs e)
        {
            // Update drawing flag so that polygon isn't updated when mouse is moved.
            isDrawing = false;
        }

        private void ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            // If drawing keep reseting the center to the original center value when we entered drawing mode. 
            // This will disable panning of the map when we click and drag. 
            this.Map.Center = center;
        }

        private Location GetMouseTouchLocation(MouseEventArgs e)
        {
            Location loc = null;

            this.Map.TryViewportPointToLocation(e.GetPosition(this.Map), out loc);

            return loc;
        }

        private void Copy_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.instructions);
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
            var printDlg = new PrintDialog();
            var doc = this.CreateFlowDocument();
            doc.Name = "FlowDoc";

            IDocumentPaginatorSource idpSource = doc;

            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintDocument(idpSource.DocumentPaginator, "Bing Maps.");
            }
        }

        private FlowDocument CreateFlowDocument()
        {
            var doc = new FlowDocument();

            var mapSection = new Section();

            // TODO: Add image to printout
            //mapSection.Blocks.Add(new BlockUIContainer(finalImage));
            var textSection = new Section();
            var table = new Table();
            textSection.Blocks.Add(table);
            doc.Blocks.Add(mapSection);
            doc.Blocks.Add(textSection);

            var rowGroup = new TableRowGroup();
            var instructionsColumn = new TableColumn { Width = new GridLength(7, GridUnitType.Star) };
            var distanceColumn = new TableColumn { Width = new GridLength(1, GridUnitType.Star) };

            table.Columns.Add(instructionsColumn);
            table.Columns.Add(distanceColumn);
            table.RowGroups.Add(rowGroup);

            foreach (var leg in route.RouteLegs)
            {
                foreach (var item in leg.ItineraryItems)
                {
                    var row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.Instruction.Text))));
                    row.Cells.Add(
                        new TableCell(new Paragraph(new Run(item.TravelDistance.ToString(CultureInfo.InvariantCulture)))));
                    rowGroup.Rows.Add(row);
                }
            }

            return doc;
        }

        private void Zoom(object sender, MouseButtonEventArgs e)
        {
            // TODO: Fix this shit!
            this.instructionPoint = ((Services.Point)((StackPanel)sender).Tag);
        }

        private void Zoom_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            this.Map.Center = new Location(this.instructionPoint.Coordinates[0], this.instructionPoint.Coordinates[1]);
            this.Map.ZoomLevel = 16;
        }

        private void FindNearby_Click(object sender, RoutedEventArgs e)
        {
            this.poiLayer.FindNearby(this.instructionPoint);
        }
    }
}
