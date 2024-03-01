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
using CADToolBox.Modules.TrackerGA.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CADToolBox.Modules.TrackerGA.Views.SubViews {
    /// <summary>
    /// SpanInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class SpanInfoView : UserControl {
        public SpanInfoView() {
            InitializeComponent();
        }

        private void SpanInfoView_OnSizeChanged(object               sender,
                                                SizeChangedEventArgs e) {
            WeakReferenceMessenger.Default.Send(new WindowSizeChangedMessage(DrawingCanvas.ActualWidth,
                                                                             DrawingCanvas.ActualHeight));
        }
    }
}