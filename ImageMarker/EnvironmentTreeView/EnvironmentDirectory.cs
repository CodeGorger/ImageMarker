using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMarker
{
    public class EnvironmentDirectory : ViewModelBase
    {
        private bool markingsFound = false;
        public bool MarkingsFound
        {
            get => markingsFound;
            set
            {
                markingsFound = value;
                OnPropertyChanged(nameof(MarkingsFound));
            }
        }

        private string markingsAt = "";
        public string MarkingsAt
        {
            get => markingsAt;
            set
            {
                markingsAt = value;
                OnPropertyChanged(nameof(MarkingsAt));
            }
        }

        private string path="";
        public string PathName
        {
            get => path;
        }

        private string dirName="";
        public string DirName
        {
            get => dirName;
            set
            {
                dirName = value;
                OnPropertyChanged(nameof(DirName));
            }
        }

        private bool isArchive=false;
        public bool IsArchive
        {
            get => isArchive;
            set
            {
                isArchive = value;
                OnPropertyChanged(nameof(IsArchive));
            }
        }
        
        private List<EnvironmentDirectory> subDirectories;
        public List<EnvironmentDirectory> SubDirectories
        {
            get => subDirectories;
            set
            {
                subDirectories = value;
                OnPropertyChanged(nameof(SubDirectories));
            }
        }

        public string ArchiveNameWithoutExtension
        {
            get
            {
                string[] inPathSplit = dirName.Split(new Char[] { '.' });
                string fileType = inPathSplit[inPathSplit.Count() - 1].ToLower();
                string archiveRet = "";
                int i = 0;
                for (i = 0; i<inPathSplit.Count()-2; i++)
                {
                    archiveRet = archiveRet + inPathSplit[i] + ".";
                }
                archiveRet = archiveRet + inPathSplit[i];
                return archiveRet;
            }
        }

        // example: inPath == "C:\prog\home\settings"
        // inPathSplit = {"C:","prog","home","settings"}
        // init("C:\prog\home","settings",false)
        // settings
        public EnvironmentDirectory(string inPath)
        {
            string onlyPath;
            string onlyName;
            splitNameAndPath(inPath, out onlyPath, out onlyName);
            init(onlyPath, onlyName, false);
        }

        private EnvironmentDirectory(string inPath, string inName, bool inIsFile)
        {
            init(inPath, inName, inIsFile);
        }

        private void init(string inPath, string inName, bool inIsArchive)
        {
            path = inPath;
            dirName = inName;
            isArchive = inIsArchive;
            subDirectories = new List<EnvironmentDirectory>();
            if (!isArchive)
            {
                //Get each file of this directory
                string[] filesEntries = getFileList(path, dirName);

                //Check each file of this directory
                foreach (string f in filesEntries)
                {
                    string onlyPath;
                    string onlyName;
                    splitNameAndPath(f, out onlyPath, out onlyName);
                    
                    if (checkIsArchive(onlyName))
                    {//its a archive file, so this must be added to the tree.
                        EnvironmentDirectory tmpArchive;
                        tmpArchive = new EnvironmentDirectory(
                            Path.Combine(path, dirName), onlyName, true);
                        tmpArchive.SearchForOwnMarkings(filesEntries);
                        subDirectories.Add(tmpArchive);
                        
                    }
                    if (isFolderMarking(onlyName))
                    {
                        // its a folder marking information.
                        // These kind of markings will only exactly 
                        // for the images in this folder
                        MarkingsFound = true;
                        MarkingsAt = f;
                    }
                }

                //Get each sub directory of this directory
                string[] subdirectoryEntries = getDirList(path, dirName);

                //Recursive adding entry for each sub dir
                foreach (string d in subdirectoryEntries)
                {
                    string onlyPath;
                    string onlyName;
                    splitNameAndPath(d, out onlyPath, out onlyName);
                    subDirectories.Add(
                        new EnvironmentDirectory(
                            Path.Combine(path, dirName), 
                            onlyName, 
                            false));
                }
            }
        }

        private static void splitNameAndPath(string inPathAndName, out string outPath, out string outName)
        {
            if(inPathAndName=="")
            {
                outPath = "";
                outName = "";
                return;
            }
            string tmpPath = "";
            string[] inPathSplit = inPathAndName.Split(new Char[] { '\\' });
            for (int i = 0; i <= inPathSplit.Count() - 3; i++)
            {
                tmpPath += inPathSplit[i];
                tmpPath += "\\";
            }
            tmpPath += inPathSplit[inPathSplit.Count() - 2];
            outPath = tmpPath;
            outName = inPathSplit[inPathSplit.Count() - 1];
        }

        private static bool checkIsArchive(string fileName)
        {
            string[] inPathSplit = fileName.Split(new Char[] { '.' });
            string fileType = inPathSplit[inPathSplit.Count() - 1].ToLower();
            if (fileType == "zip")
            {
                return true;
            }
            if (fileType == "rar")
            {
                return true;
            }
            if (fileType == "7z")
            {
                return true;
            }
            return false;
        }

        // if its folder marking, the filename is markings.xml
        private static bool isFolderMarking(string fileName)
        {
            return (fileName == "markings.xml");
        }

        // If a archive was found during the treebuilding process,
        // somehow an according markings file must be linked
        // for archives its not that easy, because the file is not located in it
        // but next to it.
        // if its archive marking, the filename is markings_archivename.xml
        private void SearchForOwnMarkings(string[] inFilesEntries)
        {
            // MarkingsFound=true or false?
            // Check all entries in inFilesEntries
            // and see if there is one in the format "markings_"+ thisfilenamewithoutextenstion +".xml"
            // if so, MarkingsFound=true
            // else MarkingsFound=false
            foreach (string f in inFilesEntries)
            {
                string onlyFile;
                string onlyPath;
                splitNameAndPath(f, out onlyPath, out onlyFile);
                string[] fileNameTokens = onlyFile.Split(new Char[] { '.', '_' });
                string fileType = fileNameTokens[fileNameTokens.Count() - 1].ToLower();
                if (fileType != "xml")
                {
                    continue;
                }
                string filePrefix = fileNameTokens[0].ToLower();
                if (filePrefix != "markings")
                {
                    continue;
                }

                if (onlyFile.Length < 4)
                {   continue;   }
                string archiveNameInMarkingName2 = onlyFile.Substring(0, onlyFile.Length - 4);

                if(archiveNameInMarkingName2.Length < 9)
                {   continue;   }
                string archiveNameInMarkingName = archiveNameInMarkingName2.Substring(9);

                if (dirName.Split(new Char[] { '.' })[0] == archiveNameInMarkingName)
                {
                    MarkingsFound = true;
                    MarkingsAt = f;
                    return;
                }
            }
        }

        private static string[] getFileList(string path, string dirName)
        {
            string p = Path.Combine(path, dirName);
            string[] filesEntries = { };
            try
            {
                filesEntries = Directory.GetFiles(p);
            }
            catch (System.UnauthorizedAccessException e)
            {
                // Ok, just skip it ... 
                //TODO(Simon): Log...
            }
            return filesEntries;
        }

        private static string[] getDirList(string path, string dirName)
        {
            string p = Path.Combine(path, dirName);
            string[] subdirectoryEntries = { };
            try
            {
                subdirectoryEntries = Directory.GetDirectories(p);
            }
            catch (System.UnauthorizedAccessException e)
            {
                // Ok, just skip it ... 
                //TODO(Simon): Log...
            }
            return subdirectoryEntries;
        }
        public string DirAndFileName()
        {
            return PathName + "\\" + DirName;
        }

        private string unzipTempDir="";
        public void UnzipToTemp()
        {
            string tempFolder = Environment.GetEnvironmentVariable("temp");
            string archiveSubFolder = ArchiveNameWithoutExtension;

            unzipTempDir = tempFolder + "\\" + archiveSubFolder;
            if (Directory.Exists(unzipTempDir))
            {
                Directory.Delete(unzipTempDir, true);
            }

            string archiveLocation = DirAndFileName();
            ZipFile.ExtractToDirectory(archiveLocation, unzipTempDir);
            
        }

        public string GetUnzipDir()
        {
            return unzipTempDir;
        }
    }
}
