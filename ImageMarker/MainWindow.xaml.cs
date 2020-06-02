using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace ImageMarker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string spawningDir = "Not Set";
        private string aliasDirAndFile = "Not Set";

        List<string> aliasEncoding;

        public string SetAliasEncodingButtonText
        {
            get => "Set Alias Encoding\n" + "("+ aliasDirAndFile + ")";
        }
        public string SetSpawnButtonText
        {
            get => "Set Spawn\n" + "(" + spawningDir + ")";
        }

        public MainWindow()
        {
            InitializeComponent();
        }


        private void checkForAliasesInSpawnDir()
        {
            string[] filesEntries = Directory.GetFiles(spawningDir);
            bool foundEntry = false;
            foreach(string f in filesEntries)
            {
                if(f == (spawningDir + "\\aliases.txt"))
                {
                    foundEntry = true;
                    break;
                }
            }

            if(foundEntry)
            {
                MessageBoxResult r;
                r= MessageBox.Show(
                    "Found aliases file at:\n" + spawningDir + "\\aliases.txt" + "\nUse it?",
                    "Use this alias file?",
                    (MessageBoxButton)MessageBoxButtons.YesNo);
                if (r== MessageBoxResult.Yes)
                {
                    aliasDirAndFile = spawningDir + "\\aliases.txt";
                    aliasEncoding = loadAliasFile(aliasDirAndFile);
                    OnPropertyChanged(nameof(SetAliasEncodingButtonText));
                }
            }
        }

        private void initTree()
        {
            EnvironmentDirectory root = new EnvironmentDirectory(spawningDir);
            ObservableCollection<EnvironmentDirectory> tmpTree;
            tmpTree = new ObservableCollection<EnvironmentDirectory>();
            tmpTree.Add(root);
            Environment_TreeView_ItemsSource = tmpTree;
        }

        private string winTitle= "Img Marker - V 1.0";
        public string WinTitle
        {
            get => winTitle;
            set
            {
                winTitle = value;
                OnPropertyChanged(nameof(WinTitle));
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


        private string askForAliasFilePoint()
        {
            var openFolderDialog = new CommonOpenFileDialog();

            openFolderDialog.Title = "Select Alias File";
            openFolderDialog.IsFolderPicker = false;

            CommonFileDialogResult result = openFolderDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                return openFolderDialog.FileName;
            }
            return "";
        }


        private void Button_Click_SetSpawn(object sender, RoutedEventArgs e)
        {
            string tmpMaybePath = askForSpawningPoint();

            if (tmpMaybePath == "")
            {
                return;
            }
            spawningDir = tmpMaybePath;
            OnPropertyChanged(nameof(SetSpawnButtonText));

            initTree();

            checkForAliasesInSpawnDir();
        }

        private void Button_Click_SetAliasEncoding(object sender, RoutedEventArgs e)
        {
            string tmpMaybePath = askForAliasFilePoint();

            if (tmpMaybePath == "")
            {
                return;
            }
            aliasDirAndFile = tmpMaybePath;
            aliasEncoding = loadAliasFile(tmpMaybePath);
            OnPropertyChanged(nameof(SetAliasEncodingButtonText));
        }



        private List<string> loadAliasFile(string tmpMaybePath)
        {
           return new List<string>(File.ReadAllLines(tmpMaybePath));
        }


        // Will return a list of string of all directories showed
        private List<EnvironmentDirectory> dirTreeToDirList(EnvironmentDirectory inDir)
        {
            List<EnvironmentDirectory> ret = new List<EnvironmentDirectory>();
            if (!inDir.MarkingsFound)
            {
                ret.Add(inDir);
            }
            foreach (EnvironmentDirectory d in inDir.SubDirectories)
            {
                ret.AddRange(dirTreeToDirList(d));
            }
            return ret;
        }


        private void Button_Click_Start(object sender, RoutedEventArgs e)
        { 
            if(spawningDir=="Not Set")
            {
                MessageBox.Show("The spawning directory must first be set.", "Set Spawn First." );
                return;
            }
            //ImgMarkingWindowModel m = new ImgMarkingWindowModel();
            ImgMarkingWindow instanceMarkingWindow = new ImgMarkingWindow();
            instanceMarkingWindow.SetData(dirTreeToDirList(Environment_TreeView_ItemsSource[0]));
            if(null!=aliasEncoding)
            {
                instanceMarkingWindow.SetAliasEncoding(aliasEncoding);
            }
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

