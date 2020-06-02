using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace ImageMarker.ImageEntity
{
    [Serializable()]
    public class ImageEntity
    {
        public ImageEntity()
        {
            fileName = "";
            directory = "";
        }
        public ImageEntity(string inFullPath, int inMarkingsCount)
        {
            fileName = inFullPath.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
            directory = inFullPath.Replace("\\"+fileName,"");
            Markings=new List<FindableEntity>();
            for (int i = 0; i < inMarkingsCount; i++)
            {
                Markings.Add(new FindableEntity());
            }
        }

        // Absolute directory
        private string directory;
        //public string Directory;
        // File name with ending
        private string fileName;

        [XmlAttribute("ImageName")]
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
            }
        }

        //Full path
        [XmlIgnore]
        public string Path
        {
            get => directory + "\\" + fileName;
            set
            {
            }
        }


        [XmlElement(ElementName = "Entity")]
        public List<FindableEntity> UsedMarkings
        {
            get
            {
                List<FindableEntity> tmp;
                tmp=(List<FindableEntity>)Markings.Where(s => s.IsUsed).ToList();
                return tmp;
            }
            set
            { }
        }

        [XmlIgnore]
        public List<FindableEntity> Markings;


        [XmlAttribute("EntityCount")]
        public int GetEntityCount
        {
            get
            {
                int ret = 0;
                for(int i=0; i< Markings.Count; i++)
                {
                    ret += (Markings[i].IsUsed ? 1 : 0);
                }
                return ret;
            }
            set
            {

            }
        }

        public Point GetCenter(int inMarkingId)
        {
            return Markings[inMarkingId].FeaturePosition.Center;
        }

        public void SetCenter(int inMarkingId, Point inCenter)
        {
            Markings[inMarkingId].FeaturePosition.Center=inCenter;
        }


        public void SetIsUsed(int inMarkingId, bool inIsUsed)
        {
            Markings[inMarkingId].IsUsed = inIsUsed;
        }

        public void SetAlias(int inMarkingId, int inAlias)
        {
            Markings[inMarkingId].Alias = inAlias;
        }


        public int GetLeft(int inMarkingId)
        {
            int x_pos = ((int)Markings[inMarkingId].FeaturePosition.Center.X);
            int radius = ((int)Markings[inMarkingId].FeaturePosition.Radius);
            return x_pos - radius;
        }

        public int GetTop(int inMarkingId)
        {
            int y_pos = ((int)Markings[inMarkingId].FeaturePosition.Center.Y);
            int radius = ((int)Markings[inMarkingId].FeaturePosition.Radius);
            return y_pos - radius;
        }

        public int DistanceToCenter(int inMarkingId, Point inPos)
        {
            double dx = (((int)Markings[inMarkingId].FeaturePosition.Center.X) - ((int)inPos.X));
            double dy = (((int)Markings[inMarkingId].FeaturePosition.Center.Y) - ((int)inPos.Y));
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }
        public void SetRadius(int inMarkingId, double inRadius)
        {
            Markings[inMarkingId].FeaturePosition.Radius = inRadius;
        }
    }
}
