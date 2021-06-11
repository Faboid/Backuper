using System.Windows;
using System.Windows.Controls;

namespace BackuperUI.UserControls {
    /// <summary>
    /// Interaction logic for TopBar.xaml
    /// </summary>
    public partial class TopBar : UserControl {
        
        public TopBar() {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(UserControl), new PropertyMetadata(string.Empty));

        public string Title {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).Minimize();

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).Maximize();

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).Close();

    }
}
