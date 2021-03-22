using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageMarker
{
    public partial class MainWindowViewModel
    {
        private string _winTitle = "Img Marker - V 1.1";
        public string WinTitle
        {
            get => _winTitle;
            set
            {
                _winTitle = value;
                OnPropertyChanged(nameof(WinTitle));
            }
        }

        private int _loadingStatusInt=1;

        private string _statusSettingSpawn = "";
        public string StatusSettingSpawn
        {
            get => _statusSettingSpawn;
            set
            {
                _statusSettingSpawn = value;
                OnPropertyChanged(nameof(StatusSettingSpawn));
            }
        }

        private ObservableCollection<EnvironmentDirectory> _environment_TreeView_ItemsSource;
        public ObservableCollection<EnvironmentDirectory> Environment_TreeView_ItemsSource
        {
            get => _environment_TreeView_ItemsSource;
            set
            {
                _environment_TreeView_ItemsSource = value;
                OnPropertyChanged(nameof(Environment_TreeView_ItemsSource));
            }
        }

        private List<string> _aliasEncoding;
        //public List<string> AliasEncoding
        //{
        //    get => _aliasEncoding;
        //    set
        //    {
        //        _aliasEncoding = value;
        //        OnPropertyChanged(nameof(AliasEncoding));
        //    }
        //}

        public string AliasEncodingFileName
        {
            set
            {
                SetAliasEncodingButtonText =
                    "Set Alias Encoding\n" + "(" + value + ")";
            }
        }

        private string _setAliasEncodingButtonText= "Set Alias Encoding\n(Not Set)";
        public string SetAliasEncodingButtonText
        {
            get
            {
                return _setAliasEncodingButtonText;
            }
            set
            {
                _setAliasEncodingButtonText =value;
                OnPropertyChanged(nameof(SetAliasEncodingButtonText));
            }
        }

        private string _spawnPath = "Not Set";
        public string SpawnPath
        {
            get => _spawnPath;
            set
            {
                _spawnPath = value;
                SetSpawnButtonText =
                    "Set Spawn\n" + "(" + value + ")";
                OnPropertyChanged(nameof(SpawnPath));
            }
        }

        private string _setSpawnButtonText = "Set Spawn\n(Not Set)";
        public string SetSpawnButtonText
        {
            get
            {
                return _setSpawnButtonText;
            }
            set
            {
                _setSpawnButtonText = value;
                OnPropertyChanged(nameof(SetSpawnButtonText));
            }
        }
    }
}
