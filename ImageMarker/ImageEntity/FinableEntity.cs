using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImageMarker.ImageEntity
{
    [Serializable()]
    public class FindableEntity
    {
        public FindableEntity()
        {
            _featurePosition = new Circle();
        }

        private Circle _featurePosition;

        [XmlElement("Circle")]
        public Circle FeaturePosition
        {
            get => _featurePosition;
            set
            {
                _featurePosition = value;
            }
        }
        
        private bool _used;
        [XmlIgnore]
        public bool IsUsed
        {
            get => _used;
            set
            {
                _used = value;
            }
        }

        private int _alias;

        [XmlAttribute("Alias")]
        public int Alias
        {
            get => _alias;
            set
            {
                _alias = value;
            }
        }
    }
}
