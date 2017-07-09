/// <summary>
/// This class is the model for interacting with the Easy Table stored on Azure
/// </summary>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseReviewer.DataModels
{
    public class CheeseReviewerModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "Brand")]
        public string Brand { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "Comments")]
        public string Comments { get; set; }

        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "Price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName = "Rating")]
        public int Rating { get; set; }

        [JsonProperty(PropertyName = "Emotion")]
        public string Emotion{ get; set; }

        // Used for getting the Brand and Type together in the ViewReviewsPage.Xaml.
        public string BrandAndType { get { return Brand + " " + Type; } }
    }
}
