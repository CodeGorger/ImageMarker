using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMarker
{
    // A tree element of the main directory structure.
    // It can be a node or a leaf
    public partial class EnvironmentDirectory
    {

        // The absolute path of the folder or archive.
        private string _pathName = "";
        public string PathName
        {
            get => _pathName;
        }

        // The folders name or the archives name.
        private string _lastDirOrArchiveName = "";
        public string LastDirOrArchiveName
        {
            get => _lastDirOrArchiveName;
            set
            {
                _lastDirOrArchiveName = value;
                OnPropertyChanged(nameof(LastDirOrArchiveName));
            }
        }

        private bool _isArchive = false;
        public bool IsArchive
        {
            get => _isArchive;
            set
            {
                _isArchive = value;
                OnPropertyChanged(nameof(IsArchive));
            }
        }

        // If the marking routine shall include these images
        private bool _markingRoutineWillRunOn = true;
        public bool MarkingRoutineWillRunOn
        {
            get => _markingRoutineWillRunOn;
            set
            {
                _markingRoutineWillRunOn = value;
                OnPropertyChanged(nameof(MarkingRoutineWillRunOn));
            }
        }

        // Are markings found 
        private bool _markingsFileFound = false;
        public bool MarkingsFileFound
        {
            get => _markingsFileFound;
            set
            {
                _markingsFileFound = value;
                OnPropertyChanged(nameof(MarkingsFileFound));
            }
        }
        public bool MarkingsFileNotFound
        {
            get => (!_markingsFileFound);
        }

        private string _fullPathToMarkingsFile = "";
        //public string FullPathToMarkingsFile
        //{
        //    get => _fullPathToMarkingsFile;
        //    set
        //    {
        //        _fullPathToMarkingsFile = value;
        //        OnPropertyChanged(nameof(FullPathToMarkingsFile));
        //    }
        //}

        private List<EnvironmentDirectory> _subDirectories;
        public List<EnvironmentDirectory> SubDirectories
        {
            get => _subDirectories;
            set
            {
                _subDirectories = value;
                OnPropertyChanged(nameof(SubDirectories));
            }
        }

    }
}
