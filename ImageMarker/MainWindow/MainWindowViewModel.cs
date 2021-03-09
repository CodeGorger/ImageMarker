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
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace ImageMarker
{
    public partial class MainWindowViewModel : ViewModelBase, ICloseWindow
    {
        private MarkingsManager _markingsManagerInstance;
        public MainWindowViewModel()
        {
            _markingsManagerInstance = new MarkingsManager();

            StartMarkingCommand = new Command(
                executeStartMarking,
                canExecuteStartMarking);

            SetMainPathCommand = new Command(
                executeSetMainPath,
                canExecuteSetMainPath);

            SetAliasesCommand = new Command(
                executeSetAliases,
                canExecuteSetAliases);
        }

        public Action Close { get; set; }
    }
    interface ICloseWindow
    {
        Action Close { get; set; }
    }
}
