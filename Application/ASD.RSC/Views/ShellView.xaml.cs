using System.Windows;

namespace ASD.RSC.Views {

    using ViewModels;

    public partial class ShellView : Window {

        public ShellView() {
            InitializeComponent();
            DataContext = new ShellViewModel();
        }
    }
}