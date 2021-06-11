using BackuperUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackuperUI.CustomControls {
    public class FolderSearchTextBox : TextBoxWithPreview {

        private const string ButtonName = "button";

        static FolderSearchTextBox() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderSearchTextBox), new FrameworkPropertyMetadata(typeof(FolderSearchTextBox)));
        }

        public override void OnApplyTemplate() {
            var btn = GetTemplateChild(ButtonName) as ButtonBase;
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, RoutedEventArgs e) {
            string result = OpenFolderDialog.Show();
            if(result is not null) {
                Text = result;
            }
        }
    }
}
