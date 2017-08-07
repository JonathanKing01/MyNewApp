using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace MyNewApp.DataModels
{
    public class NotMountainModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "Result")]
        public bool Result { get; set; }

        [JsonProperty(PropertyName = "Prediction")]
        public string Prediction { get; set; }
    }
}
