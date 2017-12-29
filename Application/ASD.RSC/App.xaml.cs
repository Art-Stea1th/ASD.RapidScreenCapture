using System.Windows;

namespace ASD.RSC {

    using Views;

    public partial class App : Application {
        public App() => (MainWindow = new ShellView()).Show();
    }
}