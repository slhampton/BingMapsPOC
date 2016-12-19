using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Maps.MapControl.WPF;
using WPFBingMapPOC;

namespace WPFBingMapPOC
{
    public partial class MapForm : Form
    {
        private readonly List<Bookmark> bookmarks = new List<Bookmark>
        {
            new Bookmark
            {
                Id = "1",
                Coordinates = new Location(55, -5),
                ZoomLevel = 4
            },
            new Bookmark
            {
                Id = "2",
                Coordinates = new Location(45, 7),
                ZoomLevel = 6
            }
        };
        
        public MapForm()
        {
            InitializeComponent();
            
            var bookmarkButtonList = new List<Button>();

            foreach (var bookmark in bookmarks)
            {
                var button = new Button { Text = bookmark.Id };
                button.Click += this.BookmarkButtonClicked;

                bookmarkButtonList.Add(button);
            }
            
            // Set different default CenterPoint depending on current region 

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

            //bookmark.ZoomLevel = mapControl1.ZoomLevel;
            //bookmark.Coordinates = mapControl1.CenterPoint as GeoPoint;
        }

        private void GoToBookmark(string id)
        {
            var bookmark = bookmarks.FirstOrDefault(x => x.Id == id);

            //mapControl1.CenterPoint = bookmark.Coordinates;
            //mapControl1.ZoomLevel = bookmark.ZoomLevel;
        }
    }
}
