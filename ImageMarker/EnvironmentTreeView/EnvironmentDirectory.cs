using System;
using System.Collections.Generic;
using System.IO;
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

        private string path="";

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


        private bool isFile;
        public bool IsFile
        {
            get => isFile;
            set
            {
                isFile = value;
                OnPropertyChanged(nameof(IsFile));
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

        private void init(string inPath, string inName, bool inIsFile)
        {
            path = inPath;
            dirName = inName;
            isFile = inIsFile;
            subDirectories = new List<EnvironmentDirectory>();
            if (!isFile)
            {
                //Get each file of this directory
                string[] filesEntries = getFileList(path, dirName);

                //Check each file of this directory
                foreach (string f in filesEntries)
                {
                    string onlyPath;
                    string onlyName;
                    splitNameAndPath(f, out onlyPath, out onlyName);
                    if (isArchive(onlyName))
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

        private static bool isArchive(string fileName)
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
                string archiveNameInMarkingName = fileNameTokens[1].ToLower();
                if (dirName.Split(new Char[] { '.' })[0].ToLower() == archiveNameInMarkingName)
                {
                    MarkingsFound = true;
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
    }
}
