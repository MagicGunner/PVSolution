using System.Windows;
using CADToolBox.Modules.FixedGA.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CADToolBox.Modules.FixedGA.Views {
    /// <summary>
    /// TrackerMainView.xaml 的交互逻辑
    /// </summary>
    public partial class FixedMainView : Window {
        public FixedMainView() {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<WindowCloseMessage>(this,
                                                                        (s,
                                                                         e) => {
                                                                            Close();
                                                                        });


            BtnMin.Click += (s, e) => WindowState = WindowState.Minimized;
            BtnMax.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            BtnClose.Click += (s, e) => {
                                  //FixedApp.Current.FixedModel!.Status = -1;
                                  Close();
                              };
        }
    }
}