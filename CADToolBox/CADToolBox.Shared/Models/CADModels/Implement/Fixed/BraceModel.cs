using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADToolBox.Shared.Models.CADModels.Interface;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CADToolBox.Shared.Models.CADModels.Implement.Fixed {
    public partial class BraceModel : ObservableObject {
        #region 通用属性

        [ObservableProperty]
        private string? _sectionType;

        [ObservableProperty]
        private string? _section;

        [ObservableProperty]
        private string? _material;

        [ObservableProperty]
        private double _length;

        [ObservableProperty]
        private double _thickness;

        #endregion

        #region 特有属性

        [ObservableProperty]
        private bool _isFont; // 是否正面朝上，正面朝上翻边为直线，否则翻边为虚线

        [ObservableProperty]
        private double _angle; // 当前斜撑与水平线的夹角

        #endregion
    }
}
