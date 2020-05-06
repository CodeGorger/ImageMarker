using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for FindableEntity.xaml
    /// </summary>
    public partial class FindableEntity : UserControl, INotifyPropertyChanged
    {
        private string circleLocation;
        public string CircleLocation
        {
            get => circleLocation;
            set
            {
                CircleLocation = value;
                OnPropertyChanged(nameof(CircleLocation));
            }
        }

        public FindableEntity()
        {
            InitializeComponent();
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
    }
}
