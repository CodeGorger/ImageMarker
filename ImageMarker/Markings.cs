using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ImageMarker
{
    [Serializable()]
    [XmlRoot(ElementName = "Markings")]
    public class Markings
    {

        // The index of the image in _toBeMarkedImages that is currently handled
        private int _nextImageIndex = 0;


        // A list of all images inside of a directory to be marked
        [XmlElement(ElementName = "ImgMarkings")]
        public List<ImageEntity.ImageEntity> Images
        {
            get => _toBeMarkedImages;
            set
            {
                _toBeMarkedImages = value;
            }
        }

        private List<ImageEntity.ImageEntity> _toBeMarkedImages;

        public void setImageList(List<ImageEntity.ImageEntity> inImages)
        {
            _toBeMarkedImages = inImages;
        }

        public bool IsEndOfListReached()
        {
            return (_nextImageIndex >= _toBeMarkedImages.Count);
        }

        public ImageEntity.ImageEntity getCurrentImageEntity()
        {
            return _toBeMarkedImages[_nextImageIndex - 1];
        }
        public void SetNextImageIndex(int inIndex)
        {
            _nextImageIndex = inIndex;
        }
        public void SetNextNextImageIndex()
        {
            _nextImageIndex++;
        }

        public string CurrentProgressStatus()
        {
            return (_nextImageIndex + 1).ToString() + "/" + _toBeMarkedImages.Count.ToString();
        }

        public string CurrentFileName()
        {
            return _toBeMarkedImages[_nextImageIndex].FileName;
        }

        public string GetNextPath()
        {
            return _toBeMarkedImages[_nextImageIndex].Path;
        }
    }
}
