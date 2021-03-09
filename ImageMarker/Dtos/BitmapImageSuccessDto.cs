using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageMarker.Dtos
{
    public class BitmapImageSuccessDto
    {
        public BitmapImageSuccessDto()
        {
            Img = new BitmapImage();
            Success = false;
            ImgNoOf = "";
            DirNoOf = "";
            ImageName = "";
            LastDirFinished = false;
            KnownMarkings = 
                new ObservableCollection<bool>()
            { false, false, false };
            FindableLeft = 
                new ObservableCollection<double>()
            { 0, 0, 0 };
            FindableTop = 
                new ObservableCollection<double>()
            { 0, 0, 0 };
            FindableWidthHeight =
                new ObservableCollection<double>()
            { 0, 0, 0 };
        }
        public BitmapImage Img;
        public bool Success;
        public string ImgNoOf;
        public string DirNoOf;
        public string ImageName;
        public bool LastDirFinished;

        public ObservableCollection<bool> KnownMarkings;
        public ObservableCollection<double> FindableLeft;
        public ObservableCollection<double> FindableTop;
        public ObservableCollection<double> FindableWidthHeight;
    }
}
