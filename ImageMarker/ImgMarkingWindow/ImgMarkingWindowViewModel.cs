using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
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

        private void _applyImgDtoToUi(BitmapImageSuccessDto inImgDto)
        {
            if (inImgDto.Img != null)
            {
                CurrentImage = inImgDto.Img;
            }
            CurrentImageOf = inImgDto.ImgNoOf;
            CurrentDirectoryOf = inImgDto.DirNoOf;
            CurrentImageName = inImgDto.ImageName;

            ObservableCollection<double> tmpFindableLeft = 
                new ObservableCollection<double>();
            ObservableCollection<double> tmpFindableTop = 
                new ObservableCollection<double>();
            double ratio = UiImageWidth / FileImageWidth;
            for (int i=0;i<3;i++)
            {
                double radius = inImgDto.FileImageRadiuses[i];
                tmpFindableLeft.Add(inImgDto.FileImageCenters[i].X * ratio - radius * ratio);
                tmpFindableTop.Add(inImgDto.FileImageCenters[i].Y * ratio - radius * ratio);
                _findableCenter[i] = new Point(
                    inImgDto.FileImageCenters[i].X * ratio,
                    inImgDto.FileImageCenters[i].Y * ratio);
            }
            FindableLeft = tmpFindableLeft;
            FindableTop = tmpFindableTop;

            FindableWidthHeight =
                new ObservableCollection<double>(
                    inImgDto.FileImageRadiuses.Select(x => x * 2 * ratio)
                    .ToList());
            IsUsedFindable =
                new ObservableCollection<bool>(inImgDto.KnownMarkings);

        }

        public void LoadNextImage()
        {
            BitmapImageSuccessDto imgDto = 
                _globalMarkingsManager.LoadNextImage();
            if(imgDto.Success)
            {
                _applyImgDtoToUi(imgDto);
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

        
        private Point _getFilePointFromUIPoint(Point inPos, bool inMirrorHorizontal=false)
        {
            int xScreen = ((int)inPos.X);
            int yScreen;
            if (inMirrorHorizontal)
            {
                yScreen = ((int)UiImageHeight) - ((int)inPos.Y);
            }
            else
            {
                yScreen = (int)inPos.Y;
            }
            double xRatio = ((double)xScreen) / UiImageWidth;
            double yRatio = ((double)yScreen) / UiImageHeight;
            double imgX = xRatio * CurrentImage.Width;
            double imgY = yRatio * CurrentImage.Height;
            return new Point(imgX, imgY);
        }


        private double _getFileRadius(double inUiRadius)
        {
            return inUiRadius * CurrentImage.Width / UiImageWidth;
        }


        public void WriteCursorPositionStatus(Point inPos)
        {
            Point filePos = _getFilePointFromUIPoint(inPos);
            CursorPosition = "( x:" + ((int)filePos.X) + " / y:" + ((int)filePos.Y) + " )";
        }


        private double _getLeft(Point inPos, double inRadius)
        {
            return inPos.X - inRadius;
        }


        private double _getTop(Point inPos, double inRadius)
        {
            return inPos.Y - inRadius;
        }


        // Not write to file, just put to the memory where it belongs
        // But changed from UI frame to Pixel frame
        private void _storeMarkingstoMarkings()
        {
            List<Point> filePointCenters = new List<Point>();
            filePointCenters.Add(_getFilePointFromUIPoint(_findableCenter[0]));
            filePointCenters.Add(_getFilePointFromUIPoint(_findableCenter[1]));
            filePointCenters.Add(_getFilePointFromUIPoint(_findableCenter[2]));

            List<double> fileRadiuses = new List<double>();
            fileRadiuses.Add(_getFileRadius(FindableWidthHeight[0] / 2));
            fileRadiuses.Add(_getFileRadius(FindableWidthHeight[1] / 2));
            fileRadiuses.Add(_getFileRadius(FindableWidthHeight[2] / 2));

            _globalMarkingsManager.StoreMarkings(
                IsUsedFindable,
                filePointCenters,
                fileRadiuses,
                AliasSelection);
        }



        public void ClickNext()
        {
            //_storeMarkingstoMarkings();

            SelectionFindable = 0;
            hasStickyRadius = false;

            LoadNextImage();
        }


        public void ResizingDone()
        {
            BitmapImageSuccessDto inImgDto =
                _globalMarkingsManager.GetCurrentFileImgMarkingsDto();
            _applyImgDtoToUi(inImgDto);
        }


        public void PurgeStickRadius()
        {
            hasStickyRadius = false;
            _isUsedFindable[SelectionFindable] = false;
            _findableLeft[SelectionFindable] = 0;
            _findableTop[SelectionFindable] = 0;
            _findableWidthHeight[SelectionFindable] = 0;
            _findableCenter[SelectionFindable] = new Point(0, 0);
        }


        public void ImageViewClick(Point inPos)
        {
            if (!hasStickyRadius)
            {
                double initialRadius = 4;
                _findableCenter[SelectionFindable] = inPos;
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
                _storeMarkingstoMarkings();
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
                    _getLeft(_findableCenter[SelectionFindable], radius);
                FindableTop[SelectionFindable] = 
                    _getTop(_findableCenter[SelectionFindable], radius);
                FindableWidthHeight[SelectionFindable] = radius * 2;
            }
        }


        private double _distanceToCenter(Point inPos)
        {
            double x1 = _findableCenter[SelectionFindable].X;
            double y1 = _findableCenter[SelectionFindable].Y;
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
