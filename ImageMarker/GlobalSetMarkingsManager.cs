using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageMarker
{
    public class GlobalSetMarkingsManager
    {
        // A list of all directories/archives with images to be marked
        private List<SetMarkingsContainer> _allSetMarkingsContainers;

        // The index of he next setmarkings in _allSetMarkingsContainers
        private int _currentSetIndex = 0;

        public GlobalSetMarkingsManager(
            List<SetMarkingsContainer> inSetMarkingsContainerList)
        {
            _allSetMarkingsContainers = inSetMarkingsContainerList;
        }


        public BitmapImageSuccessDto GetCurrentFileImgMarkingsDto()
        {
            BitmapImageSuccessDto ret =
                _allSetMarkingsContainers[_currentSetIndex].GetCurrentFileImgMarkingsDto();
            ret.DirNoOf = (_currentSetIndex + 1).ToString() +
                "/" + _allSetMarkingsContainers.Count.ToString();
            return ret;
        }

        public BitmapImageSuccessDto LoadNextImage()
        {
            // This entire algo may only be execute if not at the end
            if (_currentSetIndex >= _allSetMarkingsContainers.Count)
            {
                BitmapImageSuccessDto retLast = new BitmapImageSuccessDto();
                retLast.LastDirFinished = true;
                return retLast;
            }

            // Cycle through the sets without any images until one is found.
                while (!_allSetMarkingsContainers[_currentSetIndex].HasNextImage())
            {
                // Delete temp archive folder, serialize markings
                // and remove markings from memory
                _allSetMarkingsContainers[_currentSetIndex].Cleanup();

                _currentSetIndex++;
                // If the new current does exist because 
                // the prev was the last one
                if (_currentSetIndex >= _allSetMarkingsContainers.Count)
                {
                    BitmapImageSuccessDto retLast = new BitmapImageSuccessDto();
                    retLast.LastDirFinished = true;
                    return retLast;
                }

            }
            BitmapImageSuccessDto ret =
                _allSetMarkingsContainers[_currentSetIndex].LoadNextImage();
            ret.DirNoOf = (_currentSetIndex+1).ToString() + 
                "/" + _allSetMarkingsContainers.Count.ToString();
            return ret;
        }


        public BitmapImageSuccessDto LoadPrevImage()
        {
            // This entire algo may only be execute if not at the end
            if (_currentSetIndex < 0)
            {
                BitmapImageSuccessDto retLast = new BitmapImageSuccessDto();
                retLast.LastDirFinished = true;
                return retLast;
            }

            // Cycle through the sets without any images until one is found.
            while (!_allSetMarkingsContainers[_currentSetIndex].HasPrevImage())
            {
                if (_currentSetIndex > 0)
                {
                    // Delete temp archive folder, serialize markings
                    // and remove markings from memory
                    _allSetMarkingsContainers[_currentSetIndex].Cleanup();
                }

                _currentSetIndex--;
                // If the new current does exist because 
                // the prev was the last one
                if (_currentSetIndex < 0)
                {
                    _currentSetIndex = 0;
                    BitmapImageSuccessDto retLast = new BitmapImageSuccessDto();
                    retLast.LastDirFinished = true;
                    return retLast;
                }

            }
            BitmapImageSuccessDto ret =
                _allSetMarkingsContainers[_currentSetIndex].LoadPrevImage();
            ret.DirNoOf = (_currentSetIndex + 1).ToString() +
                "/" + _allSetMarkingsContainers.Count.ToString();
            return ret;

        }


        public string GetCurrentDir()
        {
            return _allSetMarkingsContainers[_currentSetIndex].GetPath();
        }


        public void StoreMarkings(
            ObservableCollection<bool> inIsUsedFindable,
            List<Point> inImgFileCenters,
            List<double> inImgFileRadiuses,
            ObservableCollection<int> inAliasSelection)
        {
            if(_currentSetIndex>=_allSetMarkingsContainers.Count)
            {
                return;
            }
            _allSetMarkingsContainers[_currentSetIndex].StoreMarkings(
                inIsUsedFindable,
                inImgFileCenters,
                inImgFileRadiuses,
                inAliasSelection);
        }

    }
}
