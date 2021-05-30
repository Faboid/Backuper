using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackuperUI.UserControls {
    /// <summary>
    /// Interaction logic for DynamicMargin.xaml
    /// </summary>
    public partial class DynamicMargin : UserControl {
        public DynamicMargin() {
            InitializeComponent();
        }

        private static GridLength defLenght = new(0, GridUnitType.Pixel);

        public static readonly DependencyProperty RightProperty =
            DependencyProperty.Register("Right", typeof(GridLength), typeof(Grid), new PropertyMetadata(defLenght));

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(GridLength), typeof(Grid), new PropertyMetadata(defLenght));

        public GridLength Right {
            get => (GridLength)GetValue(RightProperty);
            set => SetValue(RightProperty, value);
        }

        public GridLength Left {
            get => (GridLength)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }



    }

}
