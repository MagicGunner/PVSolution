using System.Windows;
using CADToolBox.Modules.TrackerGA.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CADToolBox.Modules.TrackerGA.Views {
    /// <summary>
    /// TrackerMainView.xaml 的交互逻辑
    /// </summary>
    public partial class TrackerMainView : Window {
        public TrackerMainView() {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<WindowCloseMessage>(this, (s,
                                                                               e) => {
                                                                                  Close();
                                                                              });


            BtnMin.Click += (s, e) => WindowState = WindowState.Minimized;
            BtnMax.Click
                += (s, e) => WindowState = WindowState == WindowState.Maximized
                                               ? WindowState.Normal
                                               : WindowState.Maximized;

            BtnClose.Click += (s,
                               e) => {
                                  TrackerApp.Current.TrackerModel!.Status = -1;
                                  Close();
                              };
        }
    }
}