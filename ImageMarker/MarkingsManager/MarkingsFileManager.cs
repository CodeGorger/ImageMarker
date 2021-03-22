using ImageMarker.Dtos;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace ImageMarker
{
    class MarkingsManager
    {
        public MarkingsManager()
        {

        }

        private AliasInformationDto checkForAliasesInSpawnDir(string inSpawningDir)
        {
            string[] filesEntries = Directory.GetFiles(inSpawningDir);
            bool foundEntry = false;
            foreach (string f in filesEntries)
            {
                if (f == (inSpawningDir + "\\aliases.txt"))
                {
                    foundEntry = true;
                    break;
                }
            }
            AliasInformationDto ret = new AliasInformationDto();
            if (foundEntry)
            {
                MessageBoxResult r;
                r = MessageBox.Show(
                    "Found aliases file at:\n" + inSpawningDir + "\\aliases.txt" + "\nUse it?",
                    "Use this alias file?",
                    (MessageBoxButton)MessageBoxButtons.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    ret.AliasesDirAndFile = inSpawningDir + "\\aliases.txt";
                    ret.AliasesList = loadAliasFile(inSpawningDir + "\\aliases.txt");
                    ret.Initialized = true;
                }
            }
            return ret;
        }

        private ObservableCollection<EnvironmentDirectory> initTree(
            string inSpawningDir,
            BackgroundWorker inSender = null)
        {
            EnvironmentDirectory.StaticBackgroundWorker = inSender;
            EnvironmentDirectory root = new EnvironmentDirectory(inSpawningDir);
            ObservableCollection<EnvironmentDirectory> tmpTree;
            tmpTree = new ObservableCollection<EnvironmentDirectory>();
            tmpTree.Add(root);
            return tmpTree;
        }



        public string AskForSpawningPoint()
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





        private List<string> loadAliasFile(string tmpMaybePath)
        {
            return new List<string>(File.ReadAllLines(tmpMaybePath));
        }


        // Will return a list of string of all directories showed
        private List<SetMarkingsContainer> dirTreeToDirList(
            EnvironmentDirectory inDir)
        {
            List<SetMarkingsContainer> ret = new List<SetMarkingsContainer>();
            if (inDir.MarkingRoutineWillRunOn)
            {
                ret.Add(convertToSetMarkings(inDir));
            }
            foreach (EnvironmentDirectory d in inDir.SubDirectories)
            {
                ret.AddRange(dirTreeToDirList(d));
            }

            return ret;
        }

        private SetMarkingsContainer convertToSetMarkings(
            EnvironmentDirectory inDir)
        {
            SetMarkingsContainer ret = new SetMarkingsContainer();
            
            ret.SetIsArchive(inDir.IsArchive);
            ret.SetFullPathMarkingsFile(inDir.GetFullPathToMarkingsFile());
            ret.SetPath(inDir.PathName);
            ret.SetFolderOrArchiveName(inDir.LastDirOrArchiveName);
            ret.SetHasMarkingsfile(inDir.MarkingsFileFound);
            return ret;
        }

        private const bool ShuffleEntries=true;
        private static Random rng = new Random();
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public void StartMarking(
            ObservableCollection<EnvironmentDirectory> inEnvironmentTree,
            List<string> inAliasEncoding,
            string inSpawnDir)
        {
            if (inSpawnDir == "Not Set")
            {
                MessageBox.Show(
                    "The spawning directory must first be set.", 
                    "Set Spawn First.");
                return;
            }

            List<SetMarkingsContainer> setMarkingsContainerList =
                dirTreeToDirList(inEnvironmentTree[0]);
            if(ShuffleEntries)
            {
                Shuffle(setMarkingsContainerList);
            }

            GlobalSetMarkingsManager gMM =
                new GlobalSetMarkingsManager(setMarkingsContainerList);
            ImgMarkingWindowViewModel tmpIMWVM =
                new ImgMarkingWindowViewModel();
            tmpIMWVM.SetData(gMM);
            if (null != inAliasEncoding)
            {
                tmpIMWVM.SetAliasEncoding(inAliasEncoding);
            }

            ImgMarkingWindow instanceMarkingWindow = 
                new ImgMarkingWindow(tmpIMWVM);
            instanceMarkingWindow.Show();
        }

        public MainPathSetDto SetMainPath(string inPath, BackgroundWorker inSender=null )
        {
            MainPathSetDto ret = new MainPathSetDto();
            ret.FileTree = initTree(inPath, inSender);
            ret.SpawnPath = inPath;
            AliasInformationDto tmpEventualAliasI = 
                checkForAliasesInSpawnDir(inPath);

            ret.AliasEncodingFound = tmpEventualAliasI.Initialized;
            if (tmpEventualAliasI.Initialized)
            {
                ret.EventualAliasDir = tmpEventualAliasI.AliasesDirAndFile;
                ret.EventualAliasEncoding = tmpEventualAliasI.AliasesList;
            }
            ret.Initialized = true;
            return ret;
        }

        /* Will return */
        public AliasInformationDto SetAliases()
        {
            string tmpMaybePath = askForAliasFilePoint();

            if (tmpMaybePath == "")
            {
                return new AliasInformationDto();
            }
            AliasInformationDto ret = new AliasInformationDto();
            ret.Initialized = true;
            ret.AliasesDirAndFile = tmpMaybePath;
            ret.AliasesList = loadAliasFile(tmpMaybePath);
            return ret;
        }
    }
}
