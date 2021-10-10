using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace ImageMarker
{
    public class SetMarkingsContainer
    {
        public SetMarkingsContainer()
        {
            _hasMarkingsFile = false;
            _fullPathFolderOrArchive = "";
            _isArchive = false;
            _fullPathMarkingsFile = "";
            _isLoaded = false;
        }


        private string _archiveUnzipTempDir;
        private bool _isLoaded;

        private string _fullPathFolderOrArchive;
        public void SetFolderOrArchiveName(string inFolderOrArchiveName)
        {
            _fullPathFolderOrArchive = inFolderOrArchiveName;
        }

        private string _path;
        public void SetPath(string inPath)
        {
            _path = inPath;
        }
        public string GetPath()
        {
            return _path;
        }
        

        private bool _isArchive;
        public void SetIsArchive(bool inIsArchive)
        {
            _isArchive = inIsArchive;
        }

        private string _fullPathMarkingsFile;
        public void SetFullPathMarkingsFile(string inFullPathMarkingsFile)
        {
            _fullPathMarkingsFile = inFullPathMarkingsFile;
        }

        private bool _hasMarkingsFile;
        public void SetHasMarkingsfile(bool inMarkingsFileFound)
        {
            _hasMarkingsFile = inMarkingsFileFound;
        }

        private SetMarkings _markings;
        public SetMarkings Markings
        {
            get =>_markings;
            set
            {
                _markings = value;
            }
        }

        private void _deserializeSetmarkings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SetMarkings));
            FileStream fs = new FileStream(
                _fullPathMarkingsFile, 
                FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(fs);
            _markings = (SetMarkings)serializer.Deserialize(reader);
            reader.Close();
        }

        private void _serializeSetmarkings()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SetMarkings));
            TextWriter stream = new StreamWriter(_fullPathMarkingsFile);
            formatter.Serialize(stream, _markings);
            stream.Close();

        }

        private string _getArchiveNameWithoutExtension()
        {
            string[] inPathSplit = 
                _fullPathFolderOrArchive.Split(new Char[] { '.' });
            string fileType = 
                inPathSplit[inPathSplit.Count() - 1].ToLower();
            string archiveRet = "";
            int i = 0;
            for (i = 0; i < inPathSplit.Count() - 2; i++)
            {
                archiveRet = archiveRet + inPathSplit[i] + ".";
            }
            archiveRet = archiveRet + inPathSplit[i];
            return archiveRet;
        }


        private void _unzipArchiveToTemp()
        {
            // Archives must be unpacked -> into %temp%/ArchiveNameWithoutZipEnding
            //_toBeMarkedDirItems[_nextDirIndex].UnzipToTemp();

            string tempFolder = Environment.GetEnvironmentVariable("temp");
            string archiveSubFolder = _getArchiveNameWithoutExtension();

            _archiveUnzipTempDir = tempFolder + "\\" + archiveSubFolder;
            if (Directory.Exists(_archiveUnzipTempDir))
            {
                Directory.Delete(_archiveUnzipTempDir, true);
            }

            string archiveLocation = _path + "\\" + _fullPathFolderOrArchive;

            // Try this, who know what kind of fancy special settings the archive has...
            try
            {
                ZipFile.ExtractToDirectory(archiveLocation, _archiveUnzipTempDir);
            }
            catch(Exception e)
            {
                // Hard to say all the implication of the try block not working
                // We'll be optimistic, but still inform the user...
                System.Windows.Forms.MessageBox.Show(
                    "Failed to extract "+ archiveLocation +".",
                    "Exception caught", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }


        private string _getRealImageLocation()
        {
            if (_isArchive)
            {
                return _archiveUnzipTempDir;
            }
            else
            {
                return _path + "\\" + _fullPathFolderOrArchive;
            }
        }


        private static List<ImageEntity.ImageEntity> _stringFileListToImageEntityList(string[] fileList)
        {
            List<ImageEntity.ImageEntity> ret = new List<ImageEntity.ImageEntity>();
            foreach (string f in fileList)
            {
                ret.Add(new ImageEntity.ImageEntity(f, 3));
            }
            return ret;
        }


        private static List<ImageEntity.ImageEntity> _getImageFileList(
            string inDirectory,
            bool inRecursive)
        {
            List<ImageEntity.ImageEntity> ret
                = new List<ImageEntity.ImageEntity>();
            List<ImageEntity.ImageEntity> all;
            try
            {
                if (inRecursive)
                {
                        all = _stringFileListToImageEntityList(
                            Directory.GetFiles(
                                inDirectory,
                                "*",
                                SearchOption.AllDirectories));
                }
                else
                {
                    all = _stringFileListToImageEntityList(
                        Directory.GetFiles(inDirectory));
                }
            ret = new List<ImageEntity.ImageEntity>(
                all.Where(s =>
                s.FileName.ToLower().EndsWith(".png") ||
                s.FileName.ToLower().EndsWith(".jpg") ||
                s.FileName.ToLower().EndsWith(".jpeg")));
            }
            catch (Exception e)
            {
                // Maybe the dir couldn't be opened.
                // It possibly wasn't extracted properly.
                // Maybe it was deleted, or something unexpected happened.
            }
            return ret;
        }


        private List<ImageEntity.ImageEntity> _loadFileList()
        {
            // Will return the true locations where the picture data lays
            string imgLocation = _getRealImageLocation();

            // Will get a list of all image files
            // for normal dirs, it will not go into the sub dirs
            // but for archives there can be sub dirs, they will be
            // handled recursively to be sure to get all images.
             return _getImageFileList(imgLocation, _isArchive);

        }


        private void _loadSetMarkingsContainer(bool inIsPrevLoad=false)
        {
            if (_hasMarkingsFile)
            {
                _deserializeSetmarkings();
            }
            else
            {
                Markings = new SetMarkings();
            }

            if (_isArchive)
            {
                _unzipArchiveToTemp();
            }

            Markings.setMergeImageList(_loadFileList());

            if(inIsPrevLoad)
            {
                Markings.SetNextImageIndexToLast();
            }
        }


        public bool HasNextImage()
        {
            if (!_isLoaded)
            {
                _loadSetMarkingsContainer();
                _isLoaded = true;
            }

            return Markings.HasNextImage();
        }
        public bool HasPrevImage()
        {
            if (!_isLoaded)
            {
                _loadSetMarkingsContainer(true);
                _isLoaded = true;
            }

            return Markings.HasPrevImage();
        }


        public BitmapImageSuccessDto GetCurrentFileImgMarkingsDto()
        {
            return Markings.GetCurrentFileImgMarkingsDto();
        }


        public BitmapImageSuccessDto LoadNextImage()
        {
            if (!_isLoaded)
            {
                _loadSetMarkingsContainer();
                _isLoaded = true;
            }
            return Markings.LoadNextImage();
        }

        public BitmapImageSuccessDto LoadPrevImage()
        {
            if (!_isLoaded)
            {
                _loadSetMarkingsContainer();
                _isLoaded = true;
            }
            return Markings.LoadPrevImage();
        }


        public void StoreMarkings(
            ObservableCollection<bool> inIsUsedFindable,
            List<Point> inImgFileCenters,
            List<double> inImgFileRadiuses,
            ObservableCollection<int> inAliasSelection)
        {
            Markings.StoreMarkings(
                inIsUsedFindable,
                inImgFileCenters,
                inImgFileRadiuses,
                inAliasSelection);
        }

        public void Cleanup()
        {
            // Delete temp archive folder
            if (Directory.Exists(_archiveUnzipTempDir))
            {
                Directory.Delete(_archiveUnzipTempDir, true);
            }

            // serialize markings
            _serializeSetmarkings();

            // remove markings from memory
            Markings = null;

            _isLoaded = false;
        }
    }
}
