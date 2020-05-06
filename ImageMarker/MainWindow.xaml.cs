using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string spawningDir = "";

        public MainWindow()
        {
            spawningDir = askForSpawningPoint();
            WinTitle = "Img Marker - " + spawningDir;
            initTree();
            InitializeComponent();
        }

        private void initTree()
        {
            EnvironmentDirectory root = new EnvironmentDirectory(spawningDir);
            ObservableCollection<EnvironmentDirectory> tmpTree;
            tmpTree = new ObservableCollection<EnvironmentDirectory>();
            tmpTree.Add(root);
            Environment_TreeView_ItemsSource = tmpTree;
        }

        private string winTitle="Img Marker";
        public string WinTitle
        {
            get => winTitle;
            set
            {
                winTitle = value;
            }
        }

        private ObservableCollection<EnvironmentDirectory> environment_TreeView_ItemsSource;
        public ObservableCollection<EnvironmentDirectory> Environment_TreeView_ItemsSource
        {
            get => environment_TreeView_ItemsSource;
            set
            {
                environment_TreeView_ItemsSource = value;
                OnPropertyChanged(nameof(Environment_TreeView_ItemsSource));
            }
        }

        private string askForSpawningPoint()
        {
            var openFolderDialog = new CommonOpenFileDialog();

            openFolderDialog.Title = "Select Root";
            openFolderDialog.IsFolderPicker = true;

            CommonFileDialogResult result = openFolderDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                return openFolderDialog.FileName;
            }
            return "";
        }

        private void Button_Click_SetSpawn(object sender, RoutedEventArgs e)
        {
            spawningDir = askForSpawningPoint();
            WinTitle = "Img Marker - " + spawningDir;
            initTree();
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        { 
            //ImgMarkingWindowModel m = new ImgMarkingWindowModel();
            ImgMarkingWindow instanceMarkingWindow = new ImgMarkingWindow();
            instanceMarkingWindow.Show();
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
