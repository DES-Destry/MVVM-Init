﻿using Microsoft.Win32;
using MVVM_Init.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace MVVM_Init.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private string slnPath;
        private string projType;
        private string ProjName => Path.GetFileNameWithoutExtension(slnPath);


        private ObservableCollection<DataGridItem> models = new ObservableCollection<DataGridItem>();
        private ObservableCollection<DataGridItem> views = new ObservableCollection<DataGridItem>();
        private ObservableCollection<DataGridItem> viewModels = new ObservableCollection<DataGridItem>();

        public string CurrentProject => $"MVVM Init: {ProjName}";

        public string ProjType
        {
            get
            {
                return projType;
            }
            set
            {
                projType = value;
                OnPropertyChanged();
                OnPropertyChanged("IsSelectProjEnabled");
            }
        }

        public bool IsSelectProjEnabled => !string.IsNullOrWhiteSpace(projType);

        public bool IsAddNewFilesEnabled => !string.IsNullOrEmpty(slnPath);

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

        public ICommand SelectProjCommand => new Command((o) => SelectProject());
        public ICommand AddModelsCommand => new Command((o) => AddModels());
        public ICommand AddViewsCommand => new Command((o) => AddViews());
        public ICommand AddViewModelsCommand => new Command((o) => AddViewModels());

        public MainViewModel()
        {

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

            Models.Clear();
            Views.Clear();
            ViewModels.Clear();

            OnPropertyChanged("IsAddNewFilesEnabled");
            OnPropertyChanged("CurrentProject");
        }

        private void AddModels()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "C# files(*.cs)|*.cs",
                Title = "Select your created models",
                Multiselect = true,
                InitialDirectory = GetInitialDirectory()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;

                foreach (string file in files)
                {
                    Models.Add(new DataGridItem(file));
                }
            }
        }

        private void AddViews()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XAML files(*.xaml)|*.xaml",
                Title = "Select your created views",
                Multiselect = true,
                InitialDirectory = GetInitialDirectory()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;

                foreach (string file in files)
                {
                    Views.Add(new DataGridItem(file));
                }
            }
        }

        private void AddViewModels()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "C# Files(*.cs)|*.cs",
                Title = "Select your view-model files",
                Multiselect = true,
                InitialDirectory = GetInitialDirectory()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;
                foreach (string file in files)
                {
                    ViewModels.Add(new DataGridItem(file));
                }
            }
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

        private string GetInitialDirectory()
        {
            string initialDirectory = slnPath;

            if (projType == "System.Windows.Controls.ComboBoxItem: WPF Project")
            {
                initialDirectory = initialDirectory.Replace(Path.GetFileName(slnPath), "");
                initialDirectory += ProjName;

                return initialDirectory;
            }
            else if (projType == "System.Windows.Controls.ComboBoxItem: Xamarin Forms")
            {
                initialDirectory = initialDirectory.Replace(Path.GetFileName(slnPath), "");
                initialDirectory += $"{ProjName}\\{ProjName}";

                return initialDirectory;
            }
            else
            {
                return "C:\\";
            }
        }
    }
}
