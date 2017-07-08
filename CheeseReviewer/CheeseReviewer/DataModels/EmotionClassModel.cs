using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseReviewer.DataModels
{
    public class EmotionClassModel
    {
        public FaceRectangle faceRectagle {get;set;}
        public Score score { get; set; }

    }

    public class FaceRectangle
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Score
    {
        public double anger { get; set; }
        public double contempt { get; set; }
        public double disgust { get; set; }
        public double fear { get; set; }
        public double happiness { get; set; }
        public double neutral { get; set; }
        public double sadness { get; set; }
        public double surprise { get; set; }
    }
        

}
