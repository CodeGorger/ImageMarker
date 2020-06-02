using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace ImageMarker
{
    [Serializable()]
    [XmlRoot(ElementName = "Circle")]
    public class Circle
    {
        private Point _center;
        [XmlIgnore]
        public Point Center
        {
            get => _center;
            set
            {
                _center = value;
            }
        }

        [XmlAttribute("x")]
        public double x
        {
            get => Math.Round(_center.X,2);
            set
            {
                _center.X=value;
            }
        }

        [XmlAttribute("y")]
        public double y
        {
            get => Math.Round(_center.Y, 2);
            set
            {
                _center.Y = value;
            }
        }

        private double _radius;
        [XmlAttribute("radius")]
        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
            }
        }
    }
}