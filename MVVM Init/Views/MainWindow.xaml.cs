using MVVM_Init.ViewModels;
using System.Windows;

namespace MVVM_Init.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
