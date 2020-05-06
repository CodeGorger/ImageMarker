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
                string p=Path.Combine(path, dirName);
                string[] filesEntries = { };
                try
                {
                    filesEntries = Directory.GetFiles(p);
                }
                catch(System.UnauthorizedAccessException e)
                {
                    // Ok, just skip it ... 
                    //TODO(Simon): Log...
                }

                foreach (string f in filesEntries)
                {
                    string onlyPath;
                    string onlyName;
                    splitNameAndPath(f, out onlyPath, out onlyName);
                    if (isArchive(onlyName))
                    {
                        subDirectories.Add(new EnvironmentDirectory(p, onlyName, true));
                    }
                }

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
                foreach (string d in subdirectoryEntries)
                {
                    string onlyPath;
                    string onlyName;
                    splitNameAndPath(d, out onlyPath, out onlyName);
                    subDirectories.Add(new EnvironmentDirectory(p, onlyName, false));
                }
            }
        }

        private static void splitNameAndPath(string inPathAndName, out string outPath, out string outName)
        {
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
    }
}
