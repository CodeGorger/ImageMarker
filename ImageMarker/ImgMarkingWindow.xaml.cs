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

namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for ImgMarkingWindow.xaml
    /// </summary>
    public partial class ImgMarkingWindow : Window, INotifyPropertyChanged
    {
        private List<EnvironmentDirectory> _toBeMarkedDirItems;

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
            DirectoryInfo directory = null;
            try
            {
                string currentDir = Environment.CurrentDirectory;
                directory = new DirectoryInfo(currentDir);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            loadImg(directory.FullName + "/redCircles_256_1024.jpg");
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
            String pathToImage = System.IO.Path.GetFullPath("redCircles_256_1024.jpg");

            CurrentImage = new BitmapImage(new Uri(pathToImage));

            SelectionFindable = 0;
        }

        public void SetData(List<EnvironmentDirectory> inTreeViewItems)
        {
            _toBeMarkedDirItems = inTreeViewItems;
            MessageBox.Show(_toBeMarkedDirItems[0].DirName);
        }
    }
}
