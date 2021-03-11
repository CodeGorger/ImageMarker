using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace ImageMarker
{
    // Called SetMarkings because it will gather all markings
    // of an image set. That is all markings to all images of a folder (or archive)
    [Serializable()]
    [XmlRoot(ElementName = "SetMarkings")]
    public class SetMarkings
    {
        public SetMarkings()
        {
            _toBeMarkedImages = new List<ImageEntity.ImageEntity>();
            _nextImageIndex = 0;
        }

        // The index of the image in _toBeMarkedImages that is currently handled
        private int _nextImageIndex;


        // A list of all images inside of a directory to be marked
        private List<ImageEntity.ImageEntity> _toBeMarkedImages;

        [XmlElement(ElementName = "ImgMarkings")]
        public List<ImageEntity.ImageEntity> Images
        {
            get => _toBeMarkedImages;
            set
            {
                _toBeMarkedImages = value;
            }
        }

        // _toBeMarkedImages is either empty or
        // has loaded markings. If the archive changed, the 
        // list should be merged.
        public void setMergeImageList(List<ImageEntity.ImageEntity> inImages)
        {      
            // for each new incoming image
            for(int i=0; i< inImages.Count; i++)
            {
                int j = 0;
                // check if there is already a file named this way
                for(j=0;
                    _toBeMarkedImages.Count>j &&
                    _toBeMarkedImages[j].FileName!=inImages[i].FileName; 
                    j++){}

                //went through completely, nothing found...
                bool nothingFound = (j >= _toBeMarkedImages.Count);

                if(nothingFound)
                {
                    _toBeMarkedImages.Add(inImages[i]);
                }
                else
                {
                    _toBeMarkedImages[j].SetPath( inImages[i] );
                }
            }
        }

        public bool HasNextImage()
        {
            // Problem is: _toBeMarkedImages not filled yet.
            // Keeping this in mind is job of SetMarkingsContainer.
            return ((_nextImageIndex) < _toBeMarkedImages.Count);
        }

        private BitmapImage _loadTheImage()
        {
            BitmapImage ret = new BitmapImage();

            String pathToImage = System.IO.Path.GetFullPath(
                _toBeMarkedImages[_nextImageIndex].Path);

            ret.BeginInit();
            ret.UriSource = new Uri(pathToImage);
            ret.CacheOption = BitmapCacheOption.OnLoad;
            ret.EndInit();

            return ret;
        }

        private void _copyMarkings(
            ref BitmapImageSuccessDto inDestDto,
            int inId)
        {
            inDestDto.KnownMarkings[inId] =
                _toBeMarkedImages[_nextImageIndex].UsedMarkings[inId].IsUsed;
            if (_toBeMarkedImages[_nextImageIndex].UsedMarkings[inId].IsUsed)
            {
                inDestDto.FileImageCenters[inId] = new Point(
                    _toBeMarkedImages[_nextImageIndex].
                        UsedMarkings[inId].FeaturePosition.x,
                    _toBeMarkedImages[_nextImageIndex].
                        UsedMarkings[inId].FeaturePosition.y
                    );
                    
                inDestDto.FileImageRadiuses[inId] =
                    _toBeMarkedImages[_nextImageIndex].
                    UsedMarkings[inId].FeaturePosition.Radius;
            }
        }

        public BitmapImageSuccessDto LoadNextImage()
        {
            BitmapImageSuccessDto ret = new BitmapImageSuccessDto();
            if (!HasNextImage())
            {
                return ret;
            }

            _copyMarkings(ref ret, 0);
            _copyMarkings(ref ret, 1);
            _copyMarkings(ref ret, 2);
            
            ret.Img = _loadTheImage();
            ret.ImgNoOf =   (_nextImageIndex + 1) + 
                            " / " + 
                            _toBeMarkedImages.Count;
            ret.ImageName = _toBeMarkedImages[_nextImageIndex].FileName;

            ret.Success = true;
            _nextImageIndex++;

            return ret;
        }


        public BitmapImageSuccessDto GetCurrentFileImgMarkingsDto()
        {
            _nextImageIndex--;
            BitmapImageSuccessDto ret = new BitmapImageSuccessDto();
            if (!HasNextImage())
            {
                return ret;
            }

            _copyMarkings(ref ret, 0);
            _copyMarkings(ref ret, 1);
            _copyMarkings(ref ret, 2);

            ret.Img = null;
            ret.ImgNoOf = (_nextImageIndex + 1) +
                            " / " +
                            _toBeMarkedImages.Count;
            ret.ImageName = _toBeMarkedImages[_nextImageIndex].FileName;

            ret.Success = true;
            _nextImageIndex++;

            return ret;
        }


        public void StoreMarkings(
            ObservableCollection<bool> inIsUsedFindable,
            List<Point> inImgFileCenters,
            List<double> inImgFileRadiuses,
            ObservableCollection<int> inAliasSelection)
        {
            for (int i = 0; i < 3; i++)
            {
                _toBeMarkedImages[_nextImageIndex - 1].UsedMarkings[i].IsUsed =
                    inIsUsedFindable[i];
                if (inIsUsedFindable[i])
                {
                    _toBeMarkedImages[_nextImageIndex - 1].UsedMarkings[i].Alias =
                       inAliasSelection[i];
                    _toBeMarkedImages[_nextImageIndex - 1].UsedMarkings[i].FeaturePosition.x =
                       inImgFileCenters[i].X;
                    _toBeMarkedImages[_nextImageIndex - 1].UsedMarkings[i].FeaturePosition.y =
                       inImgFileCenters[i].Y;
                    _toBeMarkedImages[_nextImageIndex - 1].UsedMarkings[i].FeaturePosition.Radius =
                       inImgFileRadiuses[i];
                }
            }
        }

    }
}
