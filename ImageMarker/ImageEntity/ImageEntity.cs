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
            _markings = new List<FindableEntity>();
        }
        public ImageEntity(string inFullPath, int inMarkingsCount)
        {
            fileName = inFullPath.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
            directory = inFullPath.Replace("\\"+fileName,"");
            _markings = new List<FindableEntity>();
            for (int i = 0; i < inMarkingsCount; i++)
            {
                _markings.Add(new FindableEntity());
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
            get => _markings;
            set
            {
                _markings = value;
            }
        }

        [XmlIgnore]
        private List<FindableEntity> _markings;


        [XmlAttribute("EntityCount")]
        public int GetEntityCount
        {
            get
            {
                //int ret = 0;
                //for(int i=0; i< _markings.Count; i++)
                //{
                //    ret += (_markings[i].IsUsed ? 1 : 0);
                //}
                //return ret;
                return 3;
            }
            set
            {

            }
        }

        public Point GetCenter(int inMarkingId)
        {
            return _markings[inMarkingId].FeaturePosition.Center;
        }

        public void SetCenter(int inMarkingId, Point inCenter)
        {
            _markings[inMarkingId].FeaturePosition.Center=inCenter;
        }


        public void SetIsUsed(int inMarkingId, bool inIsUsed)
        {
            _markings[inMarkingId].IsUsed = inIsUsed;
        }

        public void SetAlias(int inMarkingId, int inAlias)
        {
            _markings[inMarkingId].Alias = inAlias;
        }

        public void SetPath(ImageEntity inOtherPath)
        {
            directory = inOtherPath.directory;
            fileName = inOtherPath.fileName;
        }

        // TODO(Simon): Probably removable
        //public int GetLeft(int inMarkingId)
        //{
        //    int x_pos = ((int)_markings[inMarkingId].FeaturePosition.Center.X);
        //    int radius = ((int)_markings[inMarkingId].FeaturePosition.Radius);
        //    return x_pos - radius;
        //}

        // TODO(Simon): Probably removable
        //public int GetTop(int inMarkingId)
        //{
        //    int y_pos = ((int)_markings[inMarkingId].FeaturePosition.Center.Y);
        //    int radius = ((int)_markings[inMarkingId].FeaturePosition.Radius);
        //    return y_pos - radius;
        //}

        // TODO(Simon): Probably removable
        //public int DistanceToCenter(int inMarkingId, Point inPos)
        //{
        //    double dx = (((int)_markings[inMarkingId].FeaturePosition.Center.X) - ((int)inPos.X));
        //    double dy = (((int)_markings[inMarkingId].FeaturePosition.Center.Y) - ((int)inPos.Y));
        //    return (int)Math.Sqrt(dx * dx + dy * dy);
        //}

        // TODO(Simon): Probably removable
        //public void SetRadius(int inMarkingId, double inRadius)
        //{
        //    _markings[inMarkingId].FeaturePosition.Radius = inRadius;
        //}
    }
}
