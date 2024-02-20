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
using System.Windows.Shapes;

namespace SapToolBox.Plugin.Views {
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window {
        public MainView() {
            InitializeComponent();
            BtnMin.Click += (s, e) => WindowState = WindowState.Minimized;
            BtnMax.Click
                += (s, e) => WindowState = WindowState == WindowState.Maximized
                                               ? WindowState.Normal
                                               : WindowState.Maximized;

            BtnClose.Click += (s, e) => { Close(); };
        }
    }
}