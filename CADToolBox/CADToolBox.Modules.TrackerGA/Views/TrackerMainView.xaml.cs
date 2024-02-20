using System.Windows;

namespace CADToolBox.Modules.TrackerGA.Views {
    /// <summary>
    /// TrackerMainView.xaml 的交互逻辑
    /// </summary>
    public partial class TrackerMainView : Window {
        public TrackerMainView() {
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