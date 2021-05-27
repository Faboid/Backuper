﻿using System;
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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            var window = Window.GetWindow(this);

            window.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e) {
            var window = Window.GetWindow(this);

            if(window.WindowState == WindowState.Normal) {

                window.WindowState = WindowState.Maximized;

            } else if(window.WindowState == WindowState.Maximized) {

                window.WindowState = WindowState.Normal;
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            var window = Window.GetWindow(this);

            window.Close();
        }
    }
}