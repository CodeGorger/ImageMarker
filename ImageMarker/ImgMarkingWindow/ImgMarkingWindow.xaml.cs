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
using System.Timers;

namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for ImgMarkingWindow.xaml
    /// </summary>
    public partial class ImgMarkingWindow : Window
    {
        Timer _resizeTimer = new Timer(100) { Enabled = false };

        ImgMarkingWindowViewModel _vm;
        public ImgMarkingWindow(ImgMarkingWindowViewModel inVM)
        {
            InitializeComponent();
            _vm = inVM;
            this.DataContext = _vm;
            _resizeTimer.Elapsed += new ElapsedEventHandler(ResizingDone);
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            if (!_finishedLoading)
            {
                return;
            }
            _vm.FileImageWidth = _vm.CurrentImage.PixelWidth;
            _vm.FileImageHeight = _vm.CurrentImage.PixelHeight;
            _vm.PurgeStickRadius();
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }
        void ResizingDone(object sender, ElapsedEventArgs e)
        {
            _resizeTimer.Stop();
            _vm.ResizingDone();
        }

        private bool _finishedLoading=false;
        // OnLoad is trigged when show() was called
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _vm.SetSelectFindable(0);
            _vm.LoadNextImage();
            _finishedLoading = true;
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
            if (e.Key == Key.Escape)
            {
                _vm.PurgeStickRadius();
            }
            if (e.Key == Key.Space)
            {
                _vm.ClickNext();
            }
            if (e.Key == Key.D1)
            {
                _vm.SetSelectFindable(0);
            }
            if (e.Key == Key.D2)
            {
                _vm.SetSelectFindable(1);
            }
            if (e.Key == Key.D3)
            {
                _vm.SetSelectFindable(2);
            }
            if (e.Key == Key.Left)
            {
                _vm.ClickBack();
            }
            if (e.Key == Key.Right)
            {
                _vm.ClickNext();
            }
        }

        private void MouseMoveOnImageView(object sender, MouseEventArgs e)
        {
            UIElement s = e.Source as UIElement;
            Point pos = e.GetPosition(s);

            _vm.WriteCursorPositionStatus(pos);
            _vm.HandleRadiusDetermining(pos);
        }


        private void MouseLeftButtonDownOnImageView(object sender, MouseEventArgs e)
        {
            //SelectionFindable
            UIElement s = e.Source as UIElement;
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
