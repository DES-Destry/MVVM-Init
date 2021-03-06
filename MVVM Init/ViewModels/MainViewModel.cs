﻿using Microsoft.Win32;
using MVVM_Init.Models;
using MVVM_Init.Properties;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace MVVM_Init.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private string slnPath;
        private string projType;
        private string ProjName => Path.GetFileNameWithoutExtension(slnPath)?.Replace(" ", "_");
        private ProjectType ProjectType
        {
            get
            {
                if (projType == "System.Windows.Controls.ComboBoxItem: WPF Project")
                {
                    return ProjectType.WPF;
                }
                else if (projType == "System.Windows.Controls.ComboBoxItem: Xamarin Forms")
                {
                    return ProjectType.XamarinForms;
                }
                else
                {
                    return ProjectType.Other;
                }
            }
        }


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

                Models.Clear();
                Views.Clear();
                ViewModels.Clear();

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
        public ICommand ImplementMVVMCommand => new Command((o) => ImplementMVVM());
        public ICommand BaseViewModelCommand => new Command((o) => CreateBaseViewModelClass());
        public ICommand CreateBaseCommand => new Command((o) => CreateBaseCommandClass());

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
                InitialDirectory = GetProjCoreDirectory()
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
                InitialDirectory = GetProjCoreDirectory()
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
                InitialDirectory = GetProjCoreDirectory()
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

            MessageBox.Show("MVVM Implementated successful!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

            Models.Clear();
            ViewModels.Clear();
            Views.Clear();
        }

        private void ImplementFiles()
        {
            string core = GetProjCoreDirectory();

            if (!Directory.Exists(core + "\\Models"))
            {
                Directory.CreateDirectory(core + "\\Models");
            }
            if (!Directory.Exists(core + "\\Views"))
            {
                Directory.CreateDirectory(core + "\\Views");
            }
            if (!Directory.Exists(core + "\\ViewModels"))
            {
                Directory.CreateDirectory(core + "\\ViewModels");
            }


            if (models.Count == 0)
            {
                CreateBaseCommandClass();
            }
            else
            {
                foreach (DataGridItem model in models)
                {
                    string endPath = core + $"\\Model\\{model.FileName}";

                    string modelContent = "";
                    using (StreamReader sr = new StreamReader(model.FilePath))
                    {
                        modelContent = sr.ReadToEnd();
                    }

                    modelContent = modelContent.Replace($"namespace {ProjName}", $"namespace {ProjName}.Models");

                    using (StreamWriter sw = new StreamWriter(model.FilePath, false))
                    {
                        sw.Write(modelContent);
                    }

                    File.Move(model.FilePath, endPath);

                    if (ProjectType == ProjectType.WPF)
                    {
                        string csprojContent = "";
                        using (StreamReader sr = new StreamReader(core + $"\\{ProjName.Replace("_", " ")}.csproj"))
                        {
                            csprojContent = sr.ReadToEnd();
                        }

                        csprojContent = csprojContent.Replace($"Compile Include=\"{model.FileName}\"", $"Compile Include=\"Models\\{model.FileName}\"");

                        using (StreamWriter sw = new StreamWriter(core + $"\\{ProjName.Replace("_", " ")}.csproj", false))
                        {
                            sw.Write(csprojContent);
                        }
                    }
                }
            }

            if (views.Count == 0)
            {
                if (ProjectType == ProjectType.WPF)
                {
                    File.Create(core + "\\Views\\MainWindow.xaml").Close();
                    File.Create(core + "\\Views\\MainWindow.xaml.cs").Close();

                    string xamlContent = Resources.MainWindow;
                    string csContent = Resources.MainWindow_xaml;

                    xamlContent = xamlContent.Replace("x:Class=\"Views.MainWindow\"", $"x:Class=\"{ProjName}.Views.MainWindow\"");
                    csContent = csContent.Replace("using ViewModels;", $"using {ProjName}.ViewModels");
                    csContent = csContent.Replace("namespace Views", $"namespace {ProjName}.Views");

                    using (StreamWriter sw = new StreamWriter(core + "\\Views\\MainWindow.xaml", false))
                    {
                        sw.Write(xamlContent);
                    }
                    using (StreamWriter sw = new StreamWriter(core + "\\Views\\MainWindow.xaml.cs", false))
                    {
                        sw.Write(csContent);
                    }

                    bool isPageAdded = false;

                    XDocument xdoc = XDocument.Load(core + $"\\{ProjName.Replace("_", " ")}.csproj");
                    var itemGroups = xdoc.Root.Descendants().Where(node => node.Name.LocalName.Equals("ItemGroup"));

                    foreach (XElement itemGroup in itemGroups)
                    {
                        var compiles = itemGroup.Descendants().Where(node => node.Name.LocalName.Equals("Compile")).ToList();
                        var pages = itemGroup.Descendants().Where(node => node.Name.LocalName.Equals("Page")).ToList();

                        if (compiles.Count != 0 || pages.Count != 0)
                        {
                            XElement compile = new XElement("Compile");

                            XAttribute attribute = new XAttribute("Include", "Views\\MainWindow.xaml.cs");
                            XElement dependentUpon = new XElement("DependentUpon", "MainWindow.xaml");
                            XElement subType = new XElement("SubType", "Code");

                            compile.Add(attribute);
                            compile.Add(dependentUpon);
                            compile.Add(subType);

                            XElement page = new XElement("Page");

                            attribute = new XAttribute("Include", "Views\\MainWindow.xaml");
                            XElement generator = new XElement("Generator", "MSBuils:Compile");
                            subType = new XElement("SubType", "Designer");

                            page.Add(attribute);
                            page.Add(generator);
                            page.Add(subType);

                            itemGroup.Add(page);
                            itemGroup.Add(compile);

                            xdoc.Save(core + $"\\{ProjName.Replace("_", " ")}.csproj");

                            isPageAdded = true;

                            break;
                        }
                    }
                }
                else if (ProjectType == ProjectType.XamarinForms)
                {
                    File.Create(core + "\\Views\\MainPage.xaml");
                    File.Create(core + "\\Views\\MainPage.xaml.cs");

                    string xamlContent = Resources.MainPage;
                    string csContent = Resources.MainPage_xaml;

                    xamlContent = xamlContent.Replace("x:Class=\"Views.MainPage\"", $"x:Class=\"{ProjName}.Views.MainPage\"");
                    csContent = csContent.Replace("using ViewModels;", $"using {ProjName}.ViewModels");
                    csContent = csContent.Replace("namespace Views", $"namespace {ProjName}.Views");

                    using (StreamWriter sw = new StreamWriter(core + "\\Views\\MainPage.xaml", false))
                    {
                        sw.Write(xamlContent);
                    }
                    using (StreamWriter sw = new StreamWriter(core + "\\Views\\MainPage.xaml.cs", false))
                    {
                        sw.Write(csContent);
                    }
                }
            }
            else
            {
                foreach (DataGridItem view in views)
                {
                    string endPath = core + $"\\Views\\{view.FileName}";

                    string viewContent = "";
                    using (StreamReader sr = new StreamReader(view.FilePath))
                    {
                        viewContent = sr.ReadToEnd();
                    }

                    viewContent = viewContent.Replace($"x:Class=\"{ProjName}.{Path.GetFileNameWithoutExtension(view.FilePath)}\"", $"x:Class=\"{ProjName}.Views.{Path.GetFileNameWithoutExtension(view.FilePath)}\"");

                    using (StreamWriter sw = new StreamWriter(view.FilePath, false))
                    {
                        sw.Write(viewContent);
                    }

                    File.Move(view.FilePath, endPath);

                    if (ProjectType == ProjectType.WPF)
                    {
                        string csprojContent = "";
                        using (StreamReader sr = new StreamReader(core + $"\\{ProjName.Replace("_", " ")}.csproj"))
                        {
                            csprojContent = sr.ReadToEnd();
                        }

                        csprojContent = csprojContent.Replace($"Page Include=\"{view.FileName}\"", $"Page Include=\"Views\\{view.FileName}\"");
                        csprojContent = csprojContent.Replace($"Compile Include=\"{view.FileName}.cs\"", $"Compile Include=\"Views\\{view.FileName}.cs\"");

                        using (StreamWriter sw = new StreamWriter(core + $"\\{ProjName.Replace("_", " ")}.csproj", false))
                        {
                            sw.Write(csprojContent);
                        }
                    }


                    view.FilePath += ".cs";


                    endPath = core + $"\\Views\\{view.FileName}";

                    viewContent = "";
                    using (StreamReader sr = new StreamReader(view.FilePath))
                    {
                        viewContent = sr.ReadToEnd();
                    }

                    viewContent = viewContent.Replace($"namespace {ProjName}", $"namespace {ProjName}.Views");

                    using (StreamWriter sw = new StreamWriter(view.FilePath, false))
                    {
                        sw.WriteLine($"using {ProjName}.ViewModels;");
                        sw.Write(viewContent);
                    }

                    File.Move(view.FilePath, endPath);
                }
            }

            if (viewModels.Count == 0)
            {
                CreateBaseViewModelClass();
            }
            else
            {
                foreach (DataGridItem viewModel in viewModels)
                {
                    string endPath = core + $"\\ViewModels\\{viewModel.FileName}";

                    string viewModelContent = "";
                    using (StreamReader sr = new StreamReader(viewModel.FilePath))
                    {
                        viewModelContent = sr.ReadToEnd();
                    }

                    viewModelContent = viewModelContent.Replace($"namespace {ProjName}", $"namespace {ProjName}.ViewModels");

                    using (StreamWriter sw = new StreamWriter(viewModel.FilePath, false))
                    {
                        sw.WriteLine($"using {ProjName}.Models;");
                        sw.Write(viewModelContent);
                    }

                    File.Move(viewModel.FilePath, endPath);

                    if (ProjectType == ProjectType.WPF)
                    {
                        string csprojContent = "";
                        using (StreamReader sr = new StreamReader(core + $"\\{ProjName.Replace("_", " ")}.csproj"))
                        {
                            csprojContent = sr.ReadToEnd();
                        }

                        csprojContent = csprojContent.Replace($"Compile Include=\"{viewModel.FileName}\"", $"Compile Include=\"ViewModels\\{viewModel.FileName}\"");

                        using (StreamWriter sw = new StreamWriter(core + $"\\{ProjName.Replace("_", " ")}.csproj", false))
                        {
                            sw.Write(csprojContent);
                        }
                    }
                }
            }


            if (ProjectType == ProjectType.WPF)
            {
                string appContent = "";
                using (StreamReader sr = new StreamReader(core + "\\App.xaml"))
                {
                    appContent = sr.ReadToEnd();
                }

                string startupUri = "";
                Match match = Regex.Match(appContent, $"StartupUri=\"(.*)\" ");

                if (match.Groups.Count > 0)
                {
                    startupUri = match.Groups[0].Value;
                }

                appContent = appContent.Replace($"StartupUri=\"{startupUri}\"", $"StartupUri=\"Views/{startupUri}\"");

                using (StreamWriter sw = new StreamWriter(core + "\\App.xaml", false))
                {
                    sw.Write(appContent);
                }
            }
            else if (ProjectType == ProjectType.XamarinForms)
            {
                string appContent = "";
                using (StreamReader sr = new StreamReader(core + "\\App.xaml.cs"))
                {
                    appContent = sr.ReadToEnd();
                }

                using (StreamWriter sw = new StreamWriter(core + "\\App.xaml.cs", false))
                {
                    sw.WriteLine($"using {ProjName}.Views;");
                    sw.Write(appContent);
                }
            }
        }

        private void CreateBaseViewModelClass()
        {
            string core = GetProjCoreDirectory();

            if (File.Exists(core + "\\ViewModels\\BaseViewModel.cs"))
            {
                MessageBox.Show("This file already exist!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                File.Create(core + "\\ViewModels\\BaseViewModel.cs").Close();

                string content = Resources.BaseViewModel;
                content = content.Replace("namespace ViewModels", $"namespace {ProjName}.ViewModels");

                using (StreamWriter sw = new StreamWriter(core + "\\ViewModels\\BaseViewModel.cs", false))
                {
                    sw.Write(content);
                }


                if (ProjectType == ProjectType.WPF)
                {
                    bool isCompileAdded = false;

                    XDocument xdoc = XDocument.Load(core + $"\\{ProjName.Replace("_", " ")}.csproj");
                    var itemGroups = xdoc.Root.Descendants().Where(node => node.Name.LocalName.Equals("ItemGroup"));

                    foreach (XElement itemGroup in itemGroups)
                    {
                        var compiles = itemGroup.Descendants().Where(node => node.Name.LocalName.Equals("Compile")).ToList();
                        var pages = itemGroup.Descendants().Where(node => node.Name.LocalName.Equals("Page")).ToList();

                        if (compiles.Count != 0 || pages.Count != 0)
                        {
                            XElement compile = new XElement("Compile");
                            XAttribute attribute = new XAttribute("Include", "ViewModels\\BaseViewModel.cs");

                            compile.Add(attribute);
                            itemGroup.Add(compile);

                            xdoc.Save(core + $"\\{ProjName.Replace("_", " ")}.csproj");

                            isCompileAdded = true;

                            break;
                        }
                    }
                }
            }
        }

        private void CreateBaseCommandClass()
        {
            string core = GetProjCoreDirectory();

            if (File.Exists(core + "\\Models\\BaseCommand.cs"))
            {
                MessageBox.Show("This file already exist!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                File.Create(core + "\\Models\\BaseCommand.cs").Close();

                string content = Resources.BaseCommand;
                content = content.Replace("namespace Models", $"namespace {ProjName}.Models");

                using (StreamWriter sw = new StreamWriter(core + "\\Models\\BaseCommand.cs", false))
                {
                    sw.Write(content);
                }


                if (ProjectType == ProjectType.WPF)
                {
                    bool isCompileAdded = false;

                    XDocument xdoc = XDocument.Load(core + $"\\{ProjName.Replace("_", " ")}.csproj");
                    var itemGroups = xdoc.Root.Descendants().Where(x => x.Name.LocalName.Equals("ItemGroup"));

                    foreach (var itemGroup in itemGroups) 
                    {
                        var compiles = itemGroup.Descendants().Where(x => x.Name.LocalName.Equals("Compile")).ToList();
                        var pages = itemGroup.Descendants().Where(x => x.Name.LocalName.Equals("Page")).ToList();

                        if (compiles.Count != 0 || pages.Count != 0)
                        {
                            XElement compile = new XElement("Compile");
                            XAttribute attribute = new XAttribute("Include", "Models\\BaseCommand.cs");

                            compile.Add(attribute);
                            itemGroup.Add(compile);

                            xdoc.Save(core + $"\\{ProjName.Replace("_", " ")}.csproj");

                            isCompileAdded = true;

                            break;
                        }
                    }
                }
            }
        }

        private string GetProjCoreDirectory()
        {
            string initialDirectory = slnPath;

            if (ProjectType == ProjectType.WPF)
            {
                initialDirectory = initialDirectory.Replace(Path.GetFileName(slnPath), "");
                initialDirectory += ProjName.Replace("_", " ");

                return initialDirectory;
            }
            else if (ProjectType == ProjectType.XamarinForms)
            {
                initialDirectory = initialDirectory.Replace(Path.GetFileName(slnPath), "");
                initialDirectory += $"{ProjName.Replace("_", " ")}\\{ProjName.Replace("_", " ")}";

                return initialDirectory;
            }
            else
            {
                return "C:\\";
            }
        }
    }
}
