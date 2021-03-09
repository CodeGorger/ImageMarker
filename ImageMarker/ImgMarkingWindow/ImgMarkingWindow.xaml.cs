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
    public partial class ImgMarkingWindow : Window
    {
        ImgMarkingWindowViewModel _vm;
        public ImgMarkingWindow(ImgMarkingWindowViewModel inVM)
        {
            InitializeComponent();
            _vm = inVM;
            this.DataContext = _vm;
        }

        // OnLoad is trigged when show() was called
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _vm.SetSelectFindable(0);
            _vm.LoadNextImage();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            // CurrentImage = null;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ... Test for F5 key.
            if (e.Key == Key.F5)
            {
                this.Title = "You pressed F5";
            }
        }

        private void MouseMoveOnImageView(object sender, MouseEventArgs e)
        {
            UIElement s = e.Source as UIElement;
            Image tmpImage = ((Image)s);
            Point pos = e.GetPosition(s);

            _vm.WriteCursorPositionStatus(pos, tmpImage);

            _vm.HandleRadiusDetermining(pos);
        }


        private void MouseLeftButtonDownOnImageView(object sender, MouseEventArgs e)
        {
            //SelectionFindable
            UIElement s = e.Source as UIElement;
            Image tmpImage = ((Image)s);
            Point pos = e.GetPosition(s);

            _vm.ImageViewClick(pos);
        }
        private void Button_Click_Next(object sender, RoutedEventArgs e)
        {
            _vm.ClickNext();
        }
        public void MouseDown_Findable1(object sender, MouseButtonEventArgs e)
        {
            _vm.SetSelectFindable(0);
        }

        private void MouseDown_Findable2(object sender, MouseButtonEventArgs e)
        {
            _vm.SetSelectFindable(1);
        }

        private void MouseDown_Findable3(object sender, MouseButtonEventArgs e)
        {
            _vm.SetSelectFindable(2);
        }
    }
}
