using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MVVM_Init.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM_Init.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private string slnPath;
        private string projType;

        private ObservableCollection<DataGridItem> models = new ObservableCollection<DataGridItem>();
        private ObservableCollection<DataGridItem> views = new ObservableCollection<DataGridItem>();
        private ObservableCollection<DataGridItem> viewModels = new ObservableCollection<DataGridItem>();

        public string CurrentProject => $"MVVM Init: {Path.GetFileName(slnPath)}";

        public ICommand SelectProjCommand { get; }

        public MainViewModel()
        {
            SelectProjCommand = new Command((o) => SelectProject());
        }

        private void SelectProject()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Visual studio solution file(*.sln*)|*.sln";

            if(openFileDialog.ShowDialog() == true)
            {
                slnPath = openFileDialog.FileName;
            }

            OnPropertyChanged("CurrentProject");
        }

        private void AddModels()
        {

        }

        private void AddViews()
        {

        }

        private void AddViewModels()
        {

        }

        private void ImplementMVVM()
        {
            ImplementFolders();
            CSProjFileEdit();
        }

        private void ImplementFolders()
        {

        }

        private void CSProjFileEdit()
        {

        }
    }
}
