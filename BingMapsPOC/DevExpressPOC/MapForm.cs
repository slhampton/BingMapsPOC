using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DevExpressPOC
{
    public partial class MapForm : Form
    {
        private readonly List<Bookmark> bookmarks = new List<Bookmark>
        {
            new Bookmark
            {
                Id = "1",
                Coordinates = new GeoPoint(55, -5),
                ZoomLevel = 4
            },
            new Bookmark
            {
                Id = "2",
                Coordinates = new GeoPoint(45, 7),
                ZoomLevel = 6
            }
        };

        VectorItemsLayer VectorLayer { get { return (VectorItemsLayer)mapControl1.Layers[1]; } }

public MapForm()
        {
            InitializeComponent();

            var itemStorage = new MapItemStorage();
            itemStorage.Items.Add(new MapPushpin { Location = new GeoPoint(51.507222, -0.1275), Text = "1" });

            VectorLayer.Data = itemStorage;

            var bookmarkButtonList = new List<Button>();

            foreach (var bookmark in bookmarks)
            {
                var button = new Button { Text = bookmark.Id };
                button.Click += this.BookmarkButtonClicked;

                bookmarkButtonList.Add(button);
            }

            flowLayoutPanel1.Controls.AddRange(bookmarkButtonList.ToArray());

            // Set different default CenterPoint depending on current region 
            mapControl1.CenterPoint = new GeoPoint(53, -5);
        }


        private void BookmarkButtonClicked(object sender, EventArgs e)
        {
            var id = ((Button)sender).Text;

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                SetBookMark(sender, id);
            }
            else
            {
                GoToBookmark(id);
            }
        }

        private void SetBookMark(object sender, string id)
        {
            var bookmark = bookmarks.FirstOrDefault(x => x.Id == id);

            bookmark.ZoomLevel = mapControl1.ZoomLevel;
            bookmark.Coordinates = mapControl1.CenterPoint as GeoPoint;
        }

        private void GoToBookmark(string id)
        {
            var bookmark = bookmarks.FirstOrDefault(x => x.Id == id);

            mapControl1.CenterPoint = bookmark.Coordinates;
            mapControl1.ZoomLevel = bookmark.ZoomLevel;
        }
    }
}
