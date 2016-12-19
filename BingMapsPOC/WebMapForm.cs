using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using DevExpress.XtraMap;

namespace BingMapsPOC
{
    public partial class WebMapForm : Form
    {
        public ChromiumWebBrowser browser;

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
        

        public WebMapForm()
        {
            InitializeComponent();

            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("https://www.bing.com/maps/");
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            var bookmarkButtonList = new List<Button>();

            foreach (var bookmark in bookmarks)
            {
                var button = new Button { Text = bookmark.Id };
                button.Click += this.BookmarkButtonClicked;

                bookmarkButtonList.Add(button);
            }

            flowLayoutPanel1.Controls.AddRange(bookmarkButtonList.ToArray());
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
