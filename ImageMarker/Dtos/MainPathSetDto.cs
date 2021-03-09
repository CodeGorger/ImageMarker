using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMarker
{
    class MainPathSetDto
    {
        public MainPathSetDto()
        {
            Initialized = false;
            FileTree = new ObservableCollection<EnvironmentDirectory>();
            SpawnPath = "";
            EventualAliasDir = "";
            EventualAliasEncoding = new List<string>();
            AliasEncodingFound = false;
        }
        public bool Initialized;
        public ObservableCollection<EnvironmentDirectory> FileTree;
        public string SpawnPath;
        public string EventualAliasDir;
        public List<string> EventualAliasEncoding;
        public bool AliasEncodingFound;
    }
}
