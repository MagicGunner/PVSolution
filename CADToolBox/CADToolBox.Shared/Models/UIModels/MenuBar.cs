using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.UIModels {
    public partial class MenuBar : ObservableObject {
        [ObservableProperty]
        private string? _icon;

        [ObservableProperty]
        private string? _title;
    }
}
