using ImageMarker.Dtos;
using System;
using System.Collections.Generic;
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
        private void executeSetMainPath(object parameter)
        {
            MainPathSetDto mainPathSetInformation=_markingsManagerInstance.SetMainPath();
            if (mainPathSetInformation.Initialized)
            {
                if (mainPathSetInformation.AliasEncodingFound)
                {
                    AliasEncodingFileName = mainPathSetInformation.EventualAliasDir;
                    _aliasEncoding = mainPathSetInformation.EventualAliasEncoding;
                }
                Environment_TreeView_ItemsSource = mainPathSetInformation.FileTree;
                SpawnPath = mainPathSetInformation.SpawnPath;
            }
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
