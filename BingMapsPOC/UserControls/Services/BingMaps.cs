using System;
using System.IO;
using System.Net;
using System.Drawing;
using Microsoft.Maps.MapControl.WPF;

namespace UserControls.Services
{
    public class BingMaps
    {
        public Image AddIcon(Location location)
        {
            var uri =
                new Uri(
                    "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/47.610,-122.107/6?mapSize=100,600&pushpin=47.610,-122.107;40;P10&key=AsJ2NC7HdNc4k8T03GPuBT5IwzYUJATkSTWfaog7uEJ0lsx_XLS3st1ZStjitTm2");


            //using (var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders
            //        .Accept
            //        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            //    var response = client.SendAsync(request).Result;
            //    response.EnsureSuccessStatusCode();

            Image img = null;
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream str = response.GetResponseStream())
                {
                    img = Image.FromStream(str);
                }
            }

            return img;
         }
    }
}

