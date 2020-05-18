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

namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for ImgMarkingWindow.xaml
    /// </summary>
    public partial class ImgMarkingWindow : Window, INotifyPropertyChanged
    {
        // A list of all directories/archives with images to be marked
        private List<string> _toBeMarkedDirItems;

        // A list of all images inside of a directory to be marked
        private List<string> _toBeMarkedImages;

        // The index of directory in _toBeMarkedDirItems that is currently handled
        private int _nextDirIndex = 0;

        // The index of the image in _toBeMarkedImages that is currently handled
        private int _nextImageIndex = 0;

        public ImgMarkingWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void loadImg(string imagePathAndFilename)
        {
            try
            {
                Uri imageFilename = new Uri(imagePathAndFilename);
                CurrentImage = new BitmapImage(imageFilename);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private int findableEntityCount;
        public int FindableEntityCount
        {
            get => findableEntityCount;
            set
            {
                findableEntityCount = value;
                OnPropertyChanged(nameof(FindableEntityCount));
                OnPropertyChanged(nameof(HasOneOrMoreFindables));
                OnPropertyChanged(nameof(HasTwoOrMoreFindables));
                OnPropertyChanged(nameof(HasThreeOrMoreFindables));
            }
        }

        private int selectionFindable = 0;
        public int SelectionFindable
        {
            get => selectionFindable;
            set
            {
                selectionFindable = value;
                OnPropertyChanged(nameof(SelectionFindable));
                OnPropertyChanged(nameof(FirstFindableBorderThickness));
                OnPropertyChanged(nameof(SecondFindableBorderThickness));
                OnPropertyChanged(nameof(ThirdFindableBorderThickness));
            }
        }

        public int FirstFindableBorderThickness
        {
            get => (selectionFindable == 0) ? 5 : 2;
        }
        public int SecondFindableBorderThickness
        {
            get => (selectionFindable == 1) ? 5 : 2;
        }
        public int ThirdFindableBorderThickness
        {
            get => (selectionFindable == 2) ? 5 : 2;
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

        private String cursorPosition="";
        public String CursorPosition
        {
            get => cursorPosition;
            set
            {
                cursorPosition = value;
                OnPropertyChanged(nameof(CursorPosition));
            }
        }

        private int findableOneCenterLeft;
        public int FindableOneLeft
        {
            get => findableOneCenterLeft - findableOneRadius;
        }

        private int findableOneCenterTop;
        public int FindableOneTop
        {
            get => findableOneCenterTop - findableOneRadius;
        }

        private int findableOneRadius;
        public int FindableOneRadius
        {
            get => findableOneRadius;
            set
            {
                findableOneRadius = value;
                OnPropertyChanged(nameof(FindableOneRadius));
                OnPropertyChanged(nameof(FindableOneTop));
                OnPropertyChanged(nameof(FindableOneLeft));
                OnPropertyChanged(nameof(FindableOneWidthHeight));
            }
        }

        public int FindableOneWidthHeight
        {
            get => 2*findableOneRadius;
        }



        public bool HasOneOrMoreFindables
        {
            get => (FindableEntityCount >= 1);
        }
        public bool HasTwoOrMoreFindables
        {
            get => (FindableEntityCount >= 2);
        }

        public bool HasThreeOrMoreFindables
        {
            get => (FindableEntityCount >= 3);
        }

        private void MouseMoveOnImageView(object sender, MouseEventArgs e)
        {
            UIElement s = e.Source as UIElement;
            Image i = ((Image)s);
            ImageSource isrc = ((ImageSource)i.Source);

            int xScreen = ((int)e.GetPosition(s).X);
            int yScreen = ((int)i.ActualHeight) - ((int)e.GetPosition(s).Y);
            double xRatio = ((double)xScreen) / ((Image)s).ActualWidth;
            double yRatio = ((double)yScreen) / ((Image)s).ActualHeight;
            double imgX = xRatio * isrc.Width;
            double imgY = yRatio * isrc.Height;
            CursorPosition = "( " + ((int)imgX) + " / " + ((int)imgY) + " )";
            if (0 == SelectionFindable && 0 == hasStickyRadius)
            {
                double a = findableOneCenterLeft - ((int)e.GetPosition(s).X);
                double b = findableOneCenterTop - ((int)e.GetPosition(s).Y);
                FindableOneRadius = (int)Math.Sqrt(a * a + b * b);
            }
        } 
        
        private int hasStickyRadius = -1;

        private void MouseLeftButtonDownOnImageView(object sender, MouseEventArgs e)
        {
            //SelectionFindable
            UIElement s = e.Source as UIElement;
            Image i = ((Image)s);
            //MessageBox.Show(((int)e.GetPosition(s).X) + " / " + ((int)e.GetPosition(s).Y));
            int xScreen = ((int)e.GetPosition(s).X);
            int yScreen = ((int)i.ActualHeight) - ((int)e.GetPosition(s).Y);

            if (0 == SelectionFindable && -1 == hasStickyRadius)
            {
                findableOneCenterLeft = ((int)e.GetPosition(s).X);
                findableOneCenterTop = ((int)e.GetPosition(s).Y);
                FindableOneRadius = 10;
                hasStickyRadius = 0;
            }
            else if (0 == SelectionFindable && 0 == hasStickyRadius)
            {
                hasStickyRadius = -1;
            }

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

        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
        }

        public void SetData(List<string> inTreeViewItems)
        {
            _toBeMarkedDirItems = inTreeViewItems;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _nextDirIndex = 0;
            _nextImageIndex = 0;
            bool isLastPass = readImageList();
            if(isLastPass)
            {
                return;
            }

            loadNextImageList();
        }

        // return true:  last pass
        //        false: not last pass
        private bool readImageList()
        {
            _nextImageIndex = 0;

            if (0 != _nextDirIndex)
            {
                //TODO(Simon): Write a marking file.
            }

            if (_toBeMarkedDirItems.Count() <= _nextDirIndex)
            {
                MessageBox.Show("All items of the directories are marked.");
                this.Close();
                return true;
            }

            if (_toBeMarkedDirItems[_nextDirIndex].EndsWith(".zip"))
            { 
                // Archives must be unpacked -> into %temp%/ArchiveNameWithoutZipEnding
                // 
                string outputTempFolder = Environment.GetEnvironmentVariable("temp");
                ZipFile.ExtractToDirectory(_toBeMarkedDirItems[_nextDirIndex], outputTempFolder);
                string outputFinalFolder = _toBeMarkedDirItems[_nextDirIndex].Split(new string[] { ".zip" }, StringSplitOptions.None)[0];
                outputFinalFolder = outputFinalFolder.Split(new char[] { '\\' }).Last();

                string _archiveTempDir = outputTempFolder + "\\" + outputFinalFolder;
                List<string> tmpAllFiles = new List<string>(Directory.GetFiles(_archiveTempDir,"*", SearchOption.AllDirectories));
                _nextDirIndex++;
                _toBeMarkedImages = new List<string>(
                    tmpAllFiles.Where(s =>
                    s.ToLower().EndsWith(".png") ||
                    s.ToLower().EndsWith(".jpg") ||
                    s.ToLower().EndsWith(".jpeg")));
            }
            else
            {
                // Easy, all images are already unpacked
                List<string> tmpAllFiles = new List<string>(Directory.GetFiles(_toBeMarkedDirItems[_nextDirIndex]));
                _nextDirIndex++;
                _toBeMarkedImages = new List<string>(
                    tmpAllFiles.Where(s =>
                    s.ToLower().EndsWith(".png") ||
                    s.ToLower().EndsWith(".jpg") ||
                    s.ToLower().EndsWith(".jpeg")));
            }
            return false;
        }
        private void loadNextImageList()
        {
            while(_toBeMarkedImages.Count()<= _nextImageIndex)
            {
                bool isLastPass = readImageList();
                if (isLastPass)
                {
                    return;
                }
            }
            String pathToImage = System.IO.Path.GetFullPath(_toBeMarkedImages[_nextImageIndex]);
            CurrentImage = new BitmapImage(new Uri(pathToImage));
            SelectionFindable = 0;
        }
    }
}
