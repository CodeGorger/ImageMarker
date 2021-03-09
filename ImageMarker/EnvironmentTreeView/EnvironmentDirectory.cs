using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMarker
{
    // A tree element of the main directory structure.
    // It can be a node or a leaf
    public partial class EnvironmentDirectory : ViewModelBase
    {
        // example: inPath == "C:\prog\home\settings"
        // inPathSplit = {"C:","prog","home","settings"}
        // init("C:\prog\home","settings",false)
        // settings
        public EnvironmentDirectory(string inPath)
        {
            string onlyPath;
            string onlyName;
            StaticHelper.SplitNameAndPath(inPath, out onlyPath, out onlyName);
            init(onlyPath, onlyName, false);
        }

        private EnvironmentDirectory(string inPath, string inName, bool inIsFile)
        {
            init(inPath, inName, inIsFile);
        }


        public string GetFullPathToMarkingsFile()
        {
            return _fullPathToMarkingsFile;
        }

        private void init(string inPath, string inName, bool inIsArchive)
        {
            _pathName = inPath;
            _lastDirOrArchiveName  = inName;
            _isArchive = inIsArchive;
            _subDirectories = new List<EnvironmentDirectory>();
            if (_isArchive)
            {
                _fullPathToMarkingsFile = 
                    _pathName +
                    "\\markings_" + 
                    inName.Substring(0, inName.Length-4) +
                    ".xml";
            }
            else
            {
                _fullPathToMarkingsFile = _pathName + "\\" + _lastDirOrArchiveName + "\\markings.xml";
            }
            // If it is an archive, there is no reason to search
            // subdirectories.
            // For sub directories, they must be searched for sub sub dirs
            // and archives...
            if (!_isArchive)
            {
                //Get each file of this directory
                string[] filesEntries = getFileList(
                    _pathName, _lastDirOrArchiveName );

                //Check each file of this directory
                foreach (string f in filesEntries)
                {
                    string onlyPath;
                    string onlyName;
                    StaticHelper.SplitNameAndPath(f, out onlyPath, out onlyName);
                    
                    if (StaticHelper.CheckIsArchive(onlyName))
                    {//its a archive file, so this must be added to the tree.
                        EnvironmentDirectory tmpArchive;
                        tmpArchive = new EnvironmentDirectory(
                            Path.Combine(_pathName, _lastDirOrArchiveName ), 
                            onlyName, true);
                        tmpArchive.searchForOwnMarkings(filesEntries);
                        _subDirectories.Add(tmpArchive);
                        
                    }
                    if (StaticHelper.IsFolderMarking(onlyName))
                    {
                        // its a folder marking information.
                        // These kind of markings will only exactly 
                        // for the images in this folder
                        _markingsFileFound = true;
                        _markingRoutineWillRunOn = false;
                    }
                }



                //Get each sub directory of this directory
                string[] subdirectoryEntries = getDirList(_pathName, _lastDirOrArchiveName );

                //Recursive adding entry for each sub dir
                foreach (string d in subdirectoryEntries)
                {
                    string onlyPath;
                    string onlyName;
                    StaticHelper.SplitNameAndPath(d, out onlyPath, out onlyName);
                    _subDirectories.Add(
                        new EnvironmentDirectory(
                            Path.Combine(_pathName, _lastDirOrArchiveName ), 
                            onlyName, 
                            false));
                }
            }
        }



        // If a archive was found during the treebuilding process,
        // somehow an according markings file must be linked
        // for archives its not that easy, because the file is not located in it
        // but next to it.
        // if its archive marking, the filename is markings_archivename.xml
        private void searchForOwnMarkings(string[] inFilesEntries)
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
                StaticHelper.SplitNameAndPath(f, out onlyPath, out onlyFile);
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

                if (_lastDirOrArchiveName .Split(new Char[] { '.' })[0] == archiveNameInMarkingName)
                {
                    _markingsFileFound = true;
                    _markingRoutineWillRunOn = false;
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

        //public string DirAndFileName()
        //{
        //    return PathName + "\\" + LastDirOrArchiveName;
        //}


        //public string GetArchiveNameWithoutExtension()
        //{
        //    string[] inPathSplit = _lastDirOrArchiveName.Split(new Char[] { '.' });
        //    string fileType = inPathSplit[inPathSplit.Count() - 1].ToLower();
        //    string archiveRet = "";
        //    int i = 0;
        //    for (i = 0; i < inPathSplit.Count() - 2; i++)
        //    {
        //        archiveRet = archiveRet + inPathSplit[i] + ".";
        //    }
        //    archiveRet = archiveRet + inPathSplit[i];
        //    return archiveRet;
        //}


        //public string GetUnzipDir()
        //{
        //    return unzipTempDir;
        //}
    }
}
