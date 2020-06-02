using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class FindableEntityGui : UserControl
    {
        public static readonly DependencyProperty AliasItemProperty =
        DependencyProperty.Register(
        "AliasItem", typeof(int),
        typeof(FindableEntityGui)
        );

        public int AliasItem
        {
            get
            {
                return (int)GetValue(AliasItemProperty);
            }
            set
            {
                SetValue(AliasItemProperty, value);
            }
        }

        public static readonly DependencyProperty AliasesEncodingProperty =
        DependencyProperty.Register(
        "AliasesEncoding", typeof(ObservableCollection<string>),
        typeof(FindableEntityGui)
        );

        public ObservableCollection<string> AliasesEncoding
        {
            get
            {
                return (ObservableCollection<string>)GetValue(AliasesEncodingProperty);
            }
            set
            {
                SetValue(AliasesEncodingProperty, value);
            }
        }

        public static readonly DependencyProperty CircleLocationProperty =
        DependencyProperty.Register(
        "CircleLocation", typeof(string),
        typeof(FindableEntityGui)
        );

        public string CircleLocation
        {
            get
            {
                return (string)GetValue(CircleLocationProperty);
            }
            set
            {
                SetValue(CircleLocationProperty, value);
            }
        }

        public static readonly DependencyProperty IsUsedProperty =
        DependencyProperty.Register(
        "IsUsed", typeof(bool),
        typeof(FindableEntityGui)
        );

        public bool IsUsed
        {
            get
            {
                return (bool)GetValue(IsUsedProperty);
            }
            set
            {
                SetValue(IsUsedProperty, value);
            }
        }


        public FindableEntityGui()
        {
            InitializeComponent();
        }
    }
}
