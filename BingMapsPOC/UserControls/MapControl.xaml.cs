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
        private Location endLocation;
        private Location startLocation;
        private readonly MapLayer baseLayer;
        private readonly MapLayer drawLayer;
        private readonly MapLayer routeLayer;
        private readonly MapLayer poiLayer;
        private Point pointClicked;
        private readonly List<Location> stopLocations = new List<Location>();
        private List<MapLayer> InitialLayers = new List<MapLayer>();
        private Random random = new Random();
        private bool isDrawing = false;
        private Location center;
        private MapPolygon currentShape;
        private MapPolyline routeLine;
        private string instructions;
        private Route route;
        private Services.Point instructionPoint;
        

        public MapControl()
        {
            InitializeComponent();

            this.drawLayer = new MapLayer {Tag = "draw"};
            this.baseLayer = new MapLayer {Tag = "base"};
            this.routeLayer = new MapLayer {Tag = "route"};
            this.poiLayer = new MapLayer { Tag = "poi" };
            this.Map.Children.Add(this.baseLayer);
            this.Map.Children.Add(this.drawLayer);
            this.Map.Children.Add(this.routeLayer);
            this.Map.Children.Add(this.poiLayer);

            this.AddLayers();

            this.Map.Center = new Location(53, -5);
            this.Map.ZoomLevel = 7;
            this.InstructionsColumn.Width = new GridLength(0);
        }

        private void AddLayers()
        {
            InitialLayers.AddRange(new List<MapLayer>
            {
                new MapLayer {Tag = "car"},
                new MapLayer {Tag = "pcc"},
                new MapLayer {Tag = "home"}
            });

            foreach (var layer in InitialLayers)
            {
                this.AddRandomPins(layer);
                this.Map.Children.Add(layer);
            }
        }

        private void HideLayer(string layerToRemove)
        {
            foreach (var layer in InitialLayers)
            {
                if (layer.Tag.ToString() == layerToRemove)
                {
                    layer.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ShowLayer(string layerToRemove)
        {
            foreach (var layer in InitialLayers)
            {
                if (layer.Tag.ToString() == layerToRemove)
                {
                    layer.Visibility = Visibility.Visible;
                }
            }
        }

        private void AddRandomPins(MapLayer layer)
        {
            for (var i = 0; i < 3; i++)
            {
                var lat = random.Next(52, 56);
                var lon = random.Next(-7, 0);
                DrawCustomPins(new Location(lat, lon), null, null, layer);
            }
        }


        private void ChangeMapView_Click(object sender, RoutedEventArgs e)
        {
            // Parse the information of the button's Tag property
            var tagInfo = ((Button) sender).Tag.ToString().Split(' ');
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
                Width = radius*2,
                Height = radius*2
            };

            var logo = new BitmapImage();
            logo.BeginInit();

            logo.UriSource = layer.Tag.ToString() == "poi" ? new Uri(Symbol.GetPOILocation(typeId), UriKind.Relative) : new Uri(Symbol.GetImageLocation(layer.Tag.ToString()), UriKind.Relative);

            logo.EndInit();
            finalImage.Source = logo;

            var tt = new ToolTip {Content = toolTip};
            finalImage.ToolTip = tt;
            return finalImage;
        }

        private async void PlanRoute_Click(object sender, RoutedEventArgs e)
        {
            // Check that we have a start and end location
            if (this.startLocation == null)
            {
                MessageBox.Show("A start location is required.");
                return;
            }

            if (this.endLocation == null)
            {
                MessageBox.Show("A end location is required.");
                return;
            }

            // Calculate the route
            this.route = await BingMapsService.PlanRoute(this.startLocation, this.endLocation, this.stopLocations);

            if (route == null)
            {
                MessageBox.Show("Error planing route.");
                return;
            }

            // Remove any existing route, calculate the new route and add it to the route layer
            this.routeLayer.Children.Remove(this.routeLine);
            this.CalculateRouteLine(route);
            this.routeLayer.Children.Add(this.routeLine);

            WriteInstructions(route);

            // Show the section of the map relating to the route
            var routeCentre = new LocationRect(this.routeLine.Locations);
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

        private void CalculateRouteLine(Route route)
        {
            var routePath = route.RoutePath.Line.Coordinates;
            var locations = new LocationCollection();

            foreach (var coordinates in routePath)
            {
                if (coordinates.Length >= 2)
                {
                    locations.Add(new Location(coordinates[0], coordinates[1]));
                }
            }

            // Create a MapPolyline of the route and add it to the map
            this.routeLine = new MapPolyline
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 5,
                Opacity = 0.7,
                Locations = locations
            };
        }

        private void ClearRoute_Click(object sender, RoutedEventArgs e)
        {
            this.InstructionsColumn.Width = new GridLength(0);
            this.startLocation = null;
            this.endLocation = null;
            this.Start.IsEnabled = true;
            this.End.IsEnabled = true;
            this.routeLayer.Children.Clear();
        }

        private void AddStartPoint(object sender, RoutedEventArgs e)
        {
            this.startLocation = this.Map.ViewportPointToLocation(this.pointClicked);

            // The pushpin to add to the map.
            var startPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Green),
                Location = this.startLocation
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(startPin);

            this.Start.IsEnabled = false;
        }

        private void AddEndPoint(object sender, RoutedEventArgs e)
        {
            this.endLocation = this.Map.ViewportPointToLocation(this.pointClicked);

            // The pushpin to add to the map.
            var endPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Red),
                Location = this.endLocation
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(endPin);

            this.End.IsEnabled = false;
        }

        private void AddStop(object sender, RoutedEventArgs e)
        {
            var stopLocation = this.Map.ViewportPointToLocation(this.pointClicked);
            this.stopLocations.Add(stopLocation);

            // The pushpin to add to the map.
            var stopPin = new Pushpin
            {
                Background = new SolidColorBrush(Colors.Gray),
                Location = stopLocation
            };

            // Adds the pushpin to the map.
            this.routeLayer.Children.Add(stopPin);

            // We can only have 10 viaWaypoints (https://msdn.microsoft.com/en-us/library/ff701717.aspx)
            if (stopLocations.Count == 10)
            {
                this.Stop.IsEnabled = false;
            }
        }

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
            var layer = ((CheckBox) sender).Tag.ToString();
            HideLayer(layer);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var layer = ((CheckBox) sender).Tag.ToString();
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
            FlowDocument doc = new FlowDocument();

            var mapSection = new Section();

            // TODO: Add image to printout
            //mapSection.Blocks.Add(new BlockUIContainer(finalImage));
            var textSection = new Section();
            var table = new Table();
            textSection.Blocks.Add(table);
            doc.Blocks.Add(mapSection);
            doc.Blocks.Add(textSection);
            
            var rowGroup = new TableRowGroup();
            var instructionsColumn = new TableColumn();
            instructionsColumn.Width = new GridLength(7, GridUnitType.Star);
            var distanceColumn = new TableColumn();
            distanceColumn.Width = new GridLength(1, GridUnitType.Star);
            
            table.Columns.Add(instructionsColumn);
            table.Columns.Add(distanceColumn);
            table.RowGroups.Add(rowGroup);

            foreach (var leg in route.RouteLegs)
            {
                foreach (var item in leg.ItineraryItems)
                {
                    var row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.Instruction.Text))));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(item.TravelDistance.ToString(CultureInfo.InvariantCulture)))));
                    rowGroup.Rows.Add(row);
                }
            }

            return doc;
        }
        private void Zoom(object sender, MouseButtonEventArgs e)
        {
            // TODO: Fix this shit!
            this.instructionPoint = ((Services.Point) ((StackPanel) sender).Tag);
        }

        private void Zoom_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            this.Map.Center = new Location(this.instructionPoint.Coordinates[0], this.instructionPoint.Coordinates[1]);
            this.Map.ZoomLevel = 16;
        }

        private async void FindNearby_Click(object sender, RoutedEventArgs e)
        {
            this.poiLayer.Children.Clear();
            var currentCentre = new Location(this.instructionPoint.Coordinates[0], this.instructionPoint.Coordinates[1]);
            var entityTypes = new List<string> { "8211" , "3578", "8060", "9565" };

            var places = await BingSpatialDataService.FindNearby(currentCentre, entityTypes);

            foreach (var place in places)
            {
                var location = new Location(Convert.ToDouble(place.Latitude), Convert.ToDouble(place.Longitude));
                DrawCustomPins(location, place.DisplayName, place.EntityTypeID, this.poiLayer);
            }
        }
    }
}
