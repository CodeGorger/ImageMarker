using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Compression;
using ImageMarker.ImageEntity;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for ImgMarkingWindow.xaml
    /// </summary>
    public partial class ImgMarkingWindow : Window, INotifyPropertyChanged
    {
        public string NextStringButtonCaption
        {
            get => "Next Image";
        }

        // A list of all directories/archives with images to be marked
        private List<EnvironmentDirectory> _toBeMarkedDirItems;

        private Markings _tmpMarkings;


        // The index of directory in _toBeMarkedDirItems that is currently handled
        private int _nextDirIndex = 0;

        private bool _selectionOfNextButton = false;
        public bool SelectionOfNextButton
        {
            get => _selectionOfNextButton;
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

        private ObservableCollection<string> _aliasEncodingSetting;
        public  ObservableCollection<string> AliasEncodingSetting
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


        public ImgMarkingWindow()
        {
            DataContext = this;
            _tmpMarkings = new Markings();
            InitializeComponent();
        }


        private int selectionFindable = 0;
        public int SelectionFindable
        {
            get => selectionFindable;
            set
            {
                if(value<0 || value>2)
                {
                    return;
                }
                _findableBorderThickness[selectionFindable] = slimeBorder;
                _findableBorderThickness[value] = thickBorder;
                selectionFindable = value;
                OnPropertyChanged(nameof(SelectionFindable));
                OnPropertyChanged(nameof(FindableBorderThickness));
            }
        }

        private const int slimeBorder = 2;
        private const int thickBorder = 5;
        private ObservableCollection<int> _findableBorderThickness =
            new ObservableCollection<int>(){ slimeBorder, slimeBorder, slimeBorder };
        //TODO(Simon): Magic 'number' for findables, 3 entries ... 

        public ObservableCollection<int> FindableBorderThickness
        {
            get => _findableBorderThickness;
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

        private ObservableCollection<int> findableLeft= new ObservableCollection<int>()
        { 0, 0, 0 };//TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<int> FindableLeft
        {
            get => findableLeft;
            set
            {
                findableLeft = value;
                OnPropertyChanged(nameof(FindableLeft));
            }
        }

        private ObservableCollection<int> findableTop = new ObservableCollection<int>()
        { 0, 0, 0 };//TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<int> FindableTop
        {
            get => findableTop;
            set
            {
                findableTop = value;
                OnPropertyChanged(nameof(FindableTop));
            }
        }

        private ObservableCollection<double> findableWidthHeight = new ObservableCollection<double>()
        { 0, 0, 0 };//TODO(Simon): Magic 'number' for findables, 3 entries ... 
        public ObservableCollection<double> FindableWidthHeight
        {
            get => findableWidthHeight;
            set
            {
                findableWidthHeight = value;
                OnPropertyChanged(nameof(FindableWidthHeight));
            }
        }

        private void MouseMoveOnImageView(object sender, MouseEventArgs e)
        {
            UIElement s = e.Source as UIElement;
            Image tmpImage = ((Image)s);
            Point pos = e.GetPosition(s);

            writeCursorPositionStatus(pos, tmpImage);

            handleRadiusDetermining(pos);
        }

        private bool hasStickyRadius = false;

        // While stickyRadius, so when the
        private void handleRadiusDetermining(Point inPos)
        {
            if (hasStickyRadius)
            {
                double radius = _tmpMarkings.getCurrentImageEntity().DistanceToCenter(SelectionFindable, inPos);
                _tmpMarkings.getCurrentImageEntity().SetRadius(SelectionFindable, radius);
                FindableLeft[SelectionFindable] = _tmpMarkings.getCurrentImageEntity().GetLeft(SelectionFindable);
                FindableTop[SelectionFindable] = _tmpMarkings.getCurrentImageEntity().GetTop(SelectionFindable);
                FindableWidthHeight[SelectionFindable] = radius * 2;
            }
        }

        private void setMarkingsNull()
        {
            //TODO(Simon): Magic 'number' for findables, 3 entries ... 
            FindableLeft = new ObservableCollection<int>(){ 0, 0, 0 };
            FindableTop = new ObservableCollection<int>() { 0, 0, 0 };
            FindableWidthHeight = new ObservableCollection<double>() { 0, 0, 0 };
        }

        private void SetFindableGuiCenter(int inIdFindable, Point inCenter)
        {

        }

        private void MouseLeftButtonDownOnImageView(object sender, MouseEventArgs e)
        {
            //SelectionFindable
            UIElement s = e.Source as UIElement;
            Image i = ((Image)s);

            if (!hasStickyRadius)
            {
                double initialRadius = 4;
                Point c = e.GetPosition(s);
                _tmpMarkings.getCurrentImageEntity().SetCenter(SelectionFindable, c);
                _tmpMarkings.getCurrentImageEntity().SetRadius(SelectionFindable, initialRadius);
                FindableLeft[SelectionFindable] = _tmpMarkings.getCurrentImageEntity().GetLeft(SelectionFindable);
                FindableTop[SelectionFindable] =  _tmpMarkings.getCurrentImageEntity().GetTop(SelectionFindable);
                FindableWidthHeight[SelectionFindable] = initialRadius * 2;
                hasStickyRadius = true;
                SetFindableGuiCenter(SelectionFindable, c);

                _isUsedFindable[SelectionFindable]=true;
                OnPropertyChanged(nameof(IsUsedFindable));
            }
            else if (hasStickyRadius)
            {
                hasStickyRadius = false;
                SelectionFindable++;
                //TODO(Simon): Magic number for findables again ...
                if (SelectionFindable>=3) 
                {
                    SelectionFindable = 0;
                    _selectionOfNextButton = true;
                }
            }
        }



        private void writeCursorPositionStatus(Point inPos, Image inI)
        {
            int xScreen = ((int)inPos.X);
            int yScreen = ((int)inI.ActualHeight) - ((int)inPos.Y);
            double xRatio = ((double)xScreen) / inI.ActualWidth;
            double yRatio = ((double)yScreen) / inI.ActualHeight;
            double imgX = xRatio * ((ImageSource)inI.Source).Width;
            double imgY = yRatio * ((ImageSource)inI.Source).Height;
            CursorPosition = "( x:" + ((int)imgX) + " / y:" + ((int)imgY) + " )";
        }


        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            //TODO(Simon): Magic number of finables
            for(int i=0;i<3;i++)
            {
                _tmpMarkings.getCurrentImageEntity().SetIsUsed(i, _isUsedFindable[i]);
                _tmpMarkings.getCurrentImageEntity().SetAlias(i, _aliasSelection[i]);
                if (!_isUsedFindable[i])
                {
                    _tmpMarkings.getCurrentImageEntity().SetCenter(i, new Point(0, 0));
                    _tmpMarkings.getCurrentImageEntity().SetRadius(i, 0);
                }
            }
            
            if (_tmpMarkings.IsEndOfListReached())
            {
                // End of the list was reached, loading a 
                // new image not possible. 
                loadNextImageList();
            }
            else
            {
                // There are still images left, just load the next one
                loadNextImage();
            }

            //TODO(Simon): Magic number of finables
            for (int i = 0; i < 3; i++)
            {
                _isUsedFindable[i] = false;
            }
            OnPropertyChanged(nameof(IsUsedFindable));
        }

        public void SetData(List<EnvironmentDirectory> inTreeViewItems)
        {
            _toBeMarkedDirItems = inTreeViewItems;
        }

        public void SetAliasEncoding(List<string> inAliasEncoding)
        {
            AliasEncodingSetting = new ObservableCollection<string>(inAliasEncoding);
            AliasSelection[0] = 0;
            AliasSelection[1] = 0;
            AliasSelection[2] = 0;
            OnPropertyChanged(nameof(AliasSelection));
        }

        // OnLoad is trigged when show() was called
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _nextDirIndex = 0;
            _tmpMarkings.SetNextImageIndex(0);
            loadNextImageList();
        }

        private static List<ImageEntity.ImageEntity> stringFileListToImageEntityList(string[] fileList)
        {
            List<ImageEntity.ImageEntity> ret = new List<ImageEntity.ImageEntity>();

            foreach (string f in fileList)
            {
                //TODO(Simon): Incase someday another amount of markings per image...
                // Generally, the magic number is to  be changed.
                ret.Add(new ImageEntity.ImageEntity(f, 3));
            }

            return ret;
        }

        private static List<ImageEntity.ImageEntity> getImageFileList(
            string inDirectory,
            bool inRecursive)
        {
            List<ImageEntity.ImageEntity> ret;
            List<ImageEntity.ImageEntity> all;
            if (inRecursive)
            {
                all = stringFileListToImageEntityList(
                    Directory.GetFiles(inDirectory, "*", SearchOption.AllDirectories));
            }
            else
            {
                all = stringFileListToImageEntityList(
                    Directory.GetFiles(inDirectory));
            }
            ret = new List<ImageEntity.ImageEntity>(
                all.Where(s =>
                s.FileName.ToLower().EndsWith(".png") ||
                s.FileName.ToLower().EndsWith(".jpg") ||
                s.FileName.ToLower().EndsWith(".jpeg")));
            return ret;
        }

        // Will call readDirectoryImageList until a
        // directory with not zero images files is read.
        private void loadNextImageList()
        {
            bool isLastPass = readDirectoryImageList();
            if (isLastPass)
            {
                return;
            }

            // Basically, while there are directories without images. (or in other words)
            // While the images in the to-be-marked-list is done with it
            while (_tmpMarkings.IsEndOfListReached()) 
            {
                isLastPass = readDirectoryImageList();
                if (isLastPass)
                {
                    return;
                }
            }
            SelectionFindable = 0;
            _tmpMarkings.SetNextImageIndex(0);
            loadNextImage();
        }

        private void SerializeCurrentMarkings()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Markings));
            string writeMarkingTo;
            if (_toBeMarkedDirItems[_nextDirIndex - 1].IsArchive)
            {
                writeMarkingTo = 
                    _toBeMarkedDirItems[_nextDirIndex - 1].PathName + 
                    "\\markings_"+
                    _toBeMarkedDirItems[_nextDirIndex - 1].ArchiveNameWithoutExtension+
                    ".xml";
            }
            else
            {
                writeMarkingTo =
                    _toBeMarkedDirItems[_nextDirIndex - 1].PathName +
                    "\\"+
                    _toBeMarkedDirItems[_nextDirIndex - 1].DirName+
                    "\\markings.xml";
            }
            TextWriter stream = new StreamWriter(writeMarkingTo);
            formatter.Serialize(stream, _tmpMarkings);
            stream.Close();
        }


        // Will take the next directory entry,
        // read all images files and write it in _toBeMarkedImages
        // return true:  last pass
        //        false: not last pass
        private bool readDirectoryImageList()
        {
            _tmpMarkings.SetNextImageIndex(0);

            // During the first pass, no directory/images have been marked
            // no marking file shall be written, but if this func
            // is called any other time, one full directory is read/marked
            // the markings can be serialized/write to a file.
            if (0 != _nextDirIndex)
            {
                // Write the markings file
                SerializeCurrentMarkings();
            }

            // Are there no more directories left? (or)
            // Is the end of the directories`list reached? 
            if (_toBeMarkedDirItems.Count() <= _nextDirIndex)
            {
                MessageBox.Show("All items of the directories are marked.");
                this.Close();
                return true;
            }

            if (_toBeMarkedDirItems[_nextDirIndex].IsArchive)
            {
                // Archives must be unpacked -> into %temp%/ArchiveNameWithoutZipEnding
                // 
                _toBeMarkedDirItems[_nextDirIndex].UnzipToTemp();
                string unzipdir = _toBeMarkedDirItems[_nextDirIndex].GetUnzipDir();

                _tmpMarkings.setImageList(getImageFileList(unzipdir, true));
            }
            else
            {
                // Easy, all images are already unpacked
                _tmpMarkings.setImageList(getImageFileList(_toBeMarkedDirItems[_nextDirIndex].DirAndFileName(), false));
            }
            CurrentDirectoryOf = (_nextDirIndex + 1).ToString() + "/" + _toBeMarkedDirItems.Count.ToString();
            _nextDirIndex++;
            return false;
        }


        private void loadNextImage()
        {
            setMarkingsNull();
            SelectionFindable = 0;
            _selectionOfNextButton = false;
            String pathToImage = System.IO.Path.GetFullPath(_tmpMarkings.GetNextPath());

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(pathToImage);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            CurrentImage=src;

            CurrentImageOf = _tmpMarkings.CurrentProgressStatus();
            if (_toBeMarkedDirItems[_nextDirIndex - 1].IsArchive)
            {

                CurrentImageName = 
                    _toBeMarkedDirItems[_nextDirIndex - 1].PathName + 
                    "\\" +
                    _toBeMarkedDirItems[_nextDirIndex - 1].DirName +
                    "\\" + _tmpMarkings.CurrentFileName();

            }
            else
            {
                CurrentImageName = pathToImage;
            }
            _tmpMarkings.SetNextNextImageIndex();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ... Test for F5 key.
            if (e.Key == Key.F5)
            {
                this.Title = "You pressed F5";
            }
        }

        public void MouseDown_Findable1(object sender, MouseButtonEventArgs e)
        {
            SelectionFindable = 0;
        }

        private void MouseDown_Findable2(object sender, MouseButtonEventArgs e)
        {
            SelectionFindable = 1;
        }

        private void MouseDown_Findable3(object sender, MouseButtonEventArgs e)
        {
            SelectionFindable = 2;
        }


        private void OnClosing(object sender, CancelEventArgs e)
        {
            CurrentImage = null;
        }
    }
}
