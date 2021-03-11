using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                new List<bool>()
            { false, false, false };

            FileImageCenters =
                new List<Point>()
            { new Point(0, 0), new Point(0, 0), new Point(0, 0) };

            FileImageRadiuses =
                new List<double>()
            { 0, 0, 0 };

            Aliases =
                new List<int>()
            { 0, 0, 0 };
        }
        public BitmapImage Img;
        public bool Success;
        public string ImgNoOf;
        public string DirNoOf;
        public string ImageName;
        public bool LastDirFinished;

        public List<bool> KnownMarkings;
        public List<Point> FileImageCenters;
        public List<double> FileImageRadiuses;
        public List<int> Aliases;
    }
}
