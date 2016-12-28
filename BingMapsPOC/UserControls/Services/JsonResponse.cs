using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserControls.Services
{
    public class JsonResponse
    {
        [JsonProperty(PropertyName = "d")]
        public ResultSet ResultSet { get; set; }
    }
}
