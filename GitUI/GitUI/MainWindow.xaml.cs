using System.Windows;
using Octokit;

namespace GitUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void getGit()
        {
            var client = new GitHubClient(new ProductHeaderValue("lsjkdghlsdkjglsdj"));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            getGit();
        }
    }
}