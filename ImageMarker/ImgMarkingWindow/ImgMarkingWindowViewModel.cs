using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageMarker
{
    public partial class ImgMarkingWindowViewModel : ViewModelBase
    {
        private GlobalSetMarkingsManager _globalMarkingsManager;
        public ImgMarkingWindowViewModel()
        {
        }
        public void SetData(GlobalSetMarkingsManager inMarkingsManager)
        {
            _globalMarkingsManager = inMarkingsManager;
        }
        public void SetAliasEncoding(List<string> inAliasEncoding)
        {
            AliasEncodingSetting = new ObservableCollection<string>(inAliasEncoding);
            AliasSelection[0] = 0;
            AliasSelection[1] = 0;
            AliasSelection[2] = 0;
            OnPropertyChanged(nameof(AliasSelection));
        }
        public void LoadNextImage()
        {
            BitmapImageSuccessDto imgDto = 
                _globalMarkingsManager.LoadNextImage();
            if(imgDto.Success)
            {
                CurrentImage = imgDto.Img;
                CurrentImageOf = imgDto.ImgNoOf;
                CurrentDirectoryOf = imgDto.DirNoOf;
                CurrentImageName = imgDto.ImageName;
                FindableLeft = imgDto.FindableLeft;
                FindableTop = imgDto.FindableTop;
                
                FindableWidthHeight = imgDto.FindableWidthHeight;
                IsUsedFindable = imgDto.KnownMarkings;
            }
            else if(imgDto.LastDirFinished)
            {
                //TODO(Simon): Don't do this, arghhhh
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Couldn't load image.");
            }
        }

        public void WriteCursorPositionStatus(Point inPos, Image inI)
        {
            int xScreen = ((int)inPos.X);
            int yScreen = ((int)inI.ActualHeight) - ((int)inPos.Y);
            double xRatio = ((double)xScreen) / inI.ActualWidth;
            double yRatio = ((double)yScreen) / inI.ActualHeight;
            double imgX = xRatio * ((ImageSource)inI.Source).Width;
            double imgY = yRatio * ((ImageSource)inI.Source).Height;
            CursorPosition = "( x:" + ((int)imgX) + " / y:" + ((int)imgY) + " )";
        }

        private double _getLeft(Point inPos, double inRadius)
        {
            return inPos.X - inRadius;
        }

        private double _getTop(Point inPos, double inRadius)
        {
            return inPos.Y - inRadius;
        }


        public void ClickNext()
        {
            _globalMarkingsManager.StoreMarkings(
                IsUsedFindable,
                FindableLeft, 
                FindableTop, 
                FindableWidthHeight,
                AliasSelection);

            SelectionFindable = 0;
            hasStickyRadius = false;

            LoadNextImage();
        }

        public void ImageViewClick(Point inPos)
        {
            if (!hasStickyRadius)
            {
                double initialRadius = 4;
                findableCenter[SelectionFindable] = inPos;
                FindableLeft[SelectionFindable] = _getLeft(inPos, initialRadius);
                FindableTop[SelectionFindable] = _getTop(inPos, initialRadius);
                FindableWidthHeight[SelectionFindable] = initialRadius * 2;
                hasStickyRadius = true;
                IsUsedFindable[SelectionFindable] = true;
                //_isUsedFindable[SelectionFindable] = true;
                //OnPropertyChanged(nameof(IsUsedFindable));
            }
            else if (hasStickyRadius)
            {
                hasStickyRadius = false;
                SelectionFindable++;
                //TODO(Simon): Magic number for findables again ...
                if (SelectionFindable >= 3)
                {
                    SelectionFindable = 0;
                }
            }
        }


        private bool hasStickyRadius = false;

        // While stickyRadius, so when the
        public void HandleRadiusDetermining(Point inPos)
        {
            if (hasStickyRadius)
            {
                double radius = _distanceToCenter(inPos);
                FindableLeft[SelectionFindable] = 
                    _getLeft(findableCenter[SelectionFindable], radius);
                FindableTop[SelectionFindable] = 
                    _getTop(findableCenter[SelectionFindable], radius);
                FindableWidthHeight[SelectionFindable] = radius * 2;
            }
        }
        private double _distanceToCenter(Point inPos)
        {
            double x1 = findableCenter[SelectionFindable].X;
            double y1 = findableCenter[SelectionFindable].Y;
            double dx = (x1 - ((int)inPos.X));
            double dy = (y1 - ((int)inPos.Y));
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        public void SetSelectFindable(int inId)
        {
            SelectionFindable = inId;
        }
    }
}



//private bool _selectionOfNextButton = false;
//public bool SelectionOfNextButton
//{
//    get => _selectionOfNextButton;
//}





//private void setMarkings(ImageEntity.ImageEntity inIE)
//{
//    //TODO(Simon): Magic 'number' for findables, 3 entries ... 
//    FindableLeft = new ObservableCollection<int>() { 0, 0, 0 };
//    FindableTop = new ObservableCollection<int>() { 0, 0, 0 };
//    FindableWidthHeight = new ObservableCollection<double>() { 0, 0, 0 };

//    int i = 0;
//    foreach (FindableEntity m in inIE.UsedMarkings)
//    {
//        IsUsedFindable[i] = false;
//        if (m.IsUsed)
//        {
//            FindableLeft[i] = ((int)(m.FeaturePosition.x));
//            FindableTop[i] = ((int)(m.FeaturePosition.y));
//            FindableWidthHeight[i] = m.FeaturePosition.Radius;
//            IsUsedFindable[i] = true;
//        }
//        i++;
//    }
//    OnPropertyChanged(nameof(FindableLeft));
//    OnPropertyChanged(nameof(FindableTop));
//    OnPropertyChanged(nameof(FindableWidthHeight));
//    OnPropertyChanged(nameof(IsUsedFindable));
//}









//private static List<ImageEntity.ImageEntity> stringFileListToImageEntityList(string[] fileList)
//{
//    List<ImageEntity.ImageEntity> ret = new List<ImageEntity.ImageEntity>();

//    foreach (string f in fileList)
//    {
//        //TODO(Simon): Incase someday another amount of markings per image...
//        // Generally, the magic number is to  be changed.
//        ret.Add(new ImageEntity.ImageEntity(f, 3));
//    }

//    return ret;
//}



//// Will call readDirectoryImageList until a
//// directory with not zero images files is read.
//private void loadNextImageList()
//{
//    bool isLastPass = readDirectoryImageList();
//    if (isLastPass)
//    {
//        return;
//    }

//    // Basically, while there are directories without images. (or in other words)
//    // While the images in the to-be-marked-list is done with it
//    while (_tmpMarkings.IsEndOfListReached())
//    {
//        isLastPass = readDirectoryImageList();
//        if (isLastPass)
//        {
//            return;
//        }
//    }
//    SelectionFindable = 0;
//    _tmpMarkings.SetNextImageIndex(0);
//    loadNextImage();
//}

