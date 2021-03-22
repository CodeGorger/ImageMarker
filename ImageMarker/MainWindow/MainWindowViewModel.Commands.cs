using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageMarker
{
    public partial class MainWindowViewModel
    {
        public ICommand StartMarkingCommand { get; set; }

        private bool canExecuteStartMarking(object parameter)
        {
            return true;
        }

        private void executeStartMarking(object parameter)
        {
            _markingsManagerInstance.StartMarking(
                Environment_TreeView_ItemsSource,
                _aliasEncoding,
                _spawnPath
                );
            Close?.Invoke();
        }

        public ICommand SetMainPathCommand { get; set; }
        private bool canExecuteSetMainPath(object parameter)
        {
            return true;
        }

        BackgroundWorker _loadTreeWorker;

        private void executeSetMainPath(object parameter)
        {
            _loadTreeWorker = new BackgroundWorker();
            _loadTreeWorker.WorkerReportsProgress = true;
            _loadTreeWorker.DoWork += loadingTreeSetMainPath_DoWork;
            _loadTreeWorker.ProgressChanged += loadingTreeSetMainPath_ProgressChanged;
            _loadTreeWorker.RunWorkerCompleted += loadingTreeSetMainPath_RunWorkerCompleted;


            string tmpMaybePath = _markingsManagerInstance.AskForSpawningPoint();

            if (tmpMaybePath != "")
            {
                _loadTreeWorker.RunWorkerAsync(tmpMaybePath);
            }
        }

        private void loadingTreeSetMainPath_DoWork(
            object sender, DoWorkEventArgs e)
        {
            StatusSettingSpawn = "Loading";
            MainPathSetDto mainPathSetInformation = _markingsManagerInstance.SetMainPath((string)e.Argument, _loadTreeWorker);
            if (mainPathSetInformation.Initialized)
            {
                StatusSettingSpawn = "Loading Done";
                if (mainPathSetInformation.AliasEncodingFound)
                {
                    AliasEncodingFileName = mainPathSetInformation.EventualAliasDir;
                    _aliasEncoding = mainPathSetInformation.EventualAliasEncoding;
                }
                Environment_TreeView_ItemsSource = mainPathSetInformation.FileTree;
                SpawnPath = mainPathSetInformation.SpawnPath;
            }
            else
            {
                StatusSettingSpawn = "Loading Failed";
            }
        }

        private void loadingTreeSetMainPath_ProgressChanged(
            object sender, ProgressChangedEventArgs e)
        {
            _loadingStatusInt++;
            if (_loadingStatusInt>=4)
            {
                _loadingStatusInt = 0;
            }
            switch (_loadingStatusInt)
            {
                case 0:
                    StatusSettingSpawn = "Loading";
                    break;
                case 1:
                    StatusSettingSpawn = "Loading.";
                    break;
                case 2:
                    StatusSettingSpawn = "Loading..";
                    break;
                case 3:
                    StatusSettingSpawn = "Loading...";
                    break;
                default:
                    break;
            }
        }

        private void loadingTreeSetMainPath_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {

        }

        public ICommand SetAliasesCommand { get; set; }
        private bool canExecuteSetAliases(object parameter)
        {
            return true;
        }
        private void executeSetAliases(object parameter)
        {
            AliasInformationDto tmpAliasInfo=_markingsManagerInstance.SetAliases();

            if (tmpAliasInfo.Initialized)
            {
                AliasEncodingFileName = tmpAliasInfo.AliasesDirAndFile;
                _aliasEncoding = tmpAliasInfo.AliasesList;
            }
        }
    }
}
