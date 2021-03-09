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
            ret.DirNoOf= (_currentSetIndex+1).ToString() + 
                "/" + _allSetMarkingsContainers.Count.ToString();
            return ret;
        }


        public void StoreMarkings(
            ObservableCollection<bool> inIsUsedFindable,
            ObservableCollection<double> inFindableLeft,
            ObservableCollection<double> inFindableTop,
            ObservableCollection<double> inFindableWidthHeight,
            ObservableCollection<int> inAliasSelection)
        {
            if(_currentSetIndex>=_allSetMarkingsContainers.Count)
            {
                return;
            }
            _allSetMarkingsContainers[_currentSetIndex].StoreMarkings(
                inIsUsedFindable,
                inFindableLeft,
                inFindableTop,
                inFindableWidthHeight,
                inAliasSelection);
        }

    }
}
