using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageMarker
{
    public partial class ImgMarkingWindowViewModel
    {
        private ObservableCollection<Point> _findableCenter =
            new ObservableCollection<Point>()
                { new Point(0,0), new Point(0,0), new Point(0,0) };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 


        private ObservableCollection<double> _findableLeft =
            new ObservableCollection<double>()
                { 0, 0, 0 };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<double> FindableLeft
        {
            get => _findableLeft;
            set
            {
                _findableLeft = value;
                OnPropertyChanged(nameof(FindableLeft));
            }
        }

        private ObservableCollection<double> _findableTop =
            new ObservableCollection<double>()
                { 0, 0, 0 };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<double> FindableTop
        {
            get => _findableTop;
            set
            {
                _findableTop = value;
                OnPropertyChanged(nameof(FindableTop));
            }
        }

        private ObservableCollection<double> _findableWidthHeight
            = new ObservableCollection<double>()
                { 0, 0, 0 };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<double> FindableWidthHeight
        {
            get => _findableWidthHeight;
            set
            {
                _findableWidthHeight = value;
                OnPropertyChanged(nameof(FindableWidthHeight));
            }
        }

        private ObservableCollection<bool> _isUsedFindable =
            new ObservableCollection<bool>() { false, false, false };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 

        public ObservableCollection<bool> IsUsedFindable
        {
            get => _isUsedFindable;
            set
            {
                _isUsedFindable = value;
                OnPropertyChanged(nameof(IsUsedFindable));
            }
        }

        private string _currentDirectoryOf;
        public string CurrentDirectoryOf
        {
            get => " | Dir: " + _currentDirectoryOf;
            set
            {
                _currentDirectoryOf = value;
                OnPropertyChanged(nameof(CurrentDirectoryOf));
            }
        }

        private string _currentImageName;
        public string CurrentImageName
        {
            get => " | " + _currentImageName;
            set
            {
                _currentImageName = value;
                OnPropertyChanged(nameof(CurrentImageName));
            }
        }

        private string _currentDirectoryName;
        public string CurrentDirectoryName
        {
            get => " | " + _currentDirectoryName;
            set
            {
                _currentDirectoryName = value;
                OnPropertyChanged(nameof(CurrentDirectoryName));
            }
        }

        private string _currentDir;
        public string CurrentDir
        {
            get => " | " + _currentDir;
            set
            {
                _currentDir = value;
                OnPropertyChanged(nameof(CurrentDir));
            }
        }

        private ObservableCollection<string> _aliasEncodingSetting;
        public ObservableCollection<string> AliasEncodingSetting
        {
            get => _aliasEncodingSetting;
            set
            {
                _aliasEncodingSetting = value;
                OnPropertyChanged(nameof(AliasEncodingSetting));
            }
        }

        private ObservableCollection<int> _aliasSelection =
            new ObservableCollection<int>() { 0, 0, 0 };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<int> AliasSelection
        {
            get => _aliasSelection;
            set
            {
                _aliasSelection = value;
                OnPropertyChanged(nameof(AliasSelection));
            }
        }



        private string _currentImageOf;
        public string CurrentImageOf
        {
            get => " | Img: " + _currentImageOf;
            set
            {
                _currentImageOf = value;
                OnPropertyChanged(nameof(CurrentImageOf));
            }
        }

        private int selectionFindable = 0;
        public int SelectionFindable
        {
            get => selectionFindable;
            set
            {
                if (value < 0 || value > 2)
                {
                    return;
                }
                _findableBorderThickness[selectionFindable] = slimeBorder;
                _findableBorderThickness[value] = thickBorder;
                OnPropertyChanged(nameof(FindableBorderThickness));
                selectionFindable = value;
                OnPropertyChanged(nameof(SelectionFindable));
            }
        }

        private BitmapImage currentImage;
        public BitmapImage CurrentImage
        {
            get => currentImage;
            set
            {
                currentImage = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }

        private const int slimeBorder = 2;
        private const int thickBorder = 5;
        private ObservableCollection<int> _findableBorderThickness =
            new ObservableCollection<int>() { slimeBorder, slimeBorder, slimeBorder };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 

        public ObservableCollection<int> FindableBorderThickness
        {
            get => _findableBorderThickness;
        }

        public string NextStringButtonCaption
        {
            get => "Next Image";
        }

        private String cursorPosition = "";
        public String CursorPosition
        {
            get => cursorPosition;
            set
            {
                cursorPosition = value;
                OnPropertyChanged(nameof(CursorPosition));
            }
        }

        private double _uiImageWidth;
        public double UiImageWidth
        {
            get => _uiImageWidth;
            set
            {
                _uiImageWidth = value;
            }
        }

        private double _uiImageHeight;
        public double UiImageHeight
        {
            get => _uiImageHeight;
            set
            {
                _uiImageHeight = value;
            }
        }

        private double _fileImageWidth;
        public double FileImageWidth
        {
            get => _fileImageWidth;
            set
            {
                _fileImageWidth = value;
            }
        }

        private double _fileImageHeight;
        public double FileImageHeight
        {
            get => _fileImageHeight;
            set
            {
                _fileImageHeight = value;
            }
        }
    }
}
