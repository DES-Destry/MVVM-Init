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

        public ObservableCollection<DataGridItem> Models
        {
            get
            {
                return models;
            }
            set
            {
                models = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DataGridItem> Views
        {
            get
            {
                return views;
            }
            set
            {
                views = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DataGridItem> ViewModels
        {
            get
            {
                return viewModels;
            }
            set
            {
                viewModels = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectProjCommand { get; }
        public ICommand AddModelsCommand { get; }

        public MainViewModel()
        {
            SelectProjCommand = new Command((o) => SelectProject());
            AddModelsCommand = new Command((o) => AddModels());
        }

        private void SelectProject()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Visual studio solution file(*.sln*)|*.sln",
                Title = "Select your Visual Studio project",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                slnPath = openFileDialog.FileName;
            }

            OnPropertyChanged("CurrentProject");
        }

        private void AddModels()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "C# files(*.cs)|*.cs",
                Title = "Select your created models",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;

                foreach(string file in files)
                {
                    Models.Add(new DataGridItem(file));
                }
            }

        }

        private void AddViews()
        {

        }

        private void AddViewModels()
        {

        }

        private void ImplementMVVM()
        {
            ImplementFiles();
            CSProjFileEdit();
        }

        private void ImplementFiles()
        {

        }

        private void CSProjFileEdit()
        {

        }
    }
}
