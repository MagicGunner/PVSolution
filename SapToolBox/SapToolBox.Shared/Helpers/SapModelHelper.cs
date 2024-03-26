using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using SAP2000v1;
using SapToolBox.Shared.Sap2000;

namespace SapToolBox.Shared.Helpers {
    public class SapModelHelper : BindableBase {
    #region 字段属性

        public cSapModel? SapModel { get; set; }


        private readonly int _frameNum = 0;

        public int FrameNum => _frameNum;

        private readonly string[]     _frameNameList = [];
        public           List<string> FrameNameList => [.._frameNameList];


        private readonly string[] _groupList = [];

        public List<string> GroupList => [.. _groupList];

        private readonly int _groupNum = 0;

        public int GroupNum => _groupNum;

        private string? _designCode;

        public string? DesignCode {
            get {
                SapModel?.DesignSteel.GetCode(ref _designCode);
                return _designCode;
            }
            set {
                SapModel?.DesignSteel.SetCode(value);
                SetProperty(ref _designCode, value);
            }
        }

    #endregion

    #region 构造函数

        public SapModelHelper() {
        #region 设计覆盖项部分，使用sap2000自带功能，暂时废弃

            //SapModel = sapModel;

            //SapModel.GroupDef.GetNameList(ref _groupNum, ref _groupList);

            //SapModel.FrameObj.GetNameList(ref _frameNum, ref _frameNameList);


            //InitNameDic(); // 根据设计规范初始化当前模型的字典

            //InitOverwriteList();

        #endregion
        }

    #endregion

    #region 方法

        public void SetOverwrite(string groupName,
                                 int    propIndex,
                                 double propValue) {
            switch (DesignCode) {
                case "Chinese 2018":
                    SapModel.DesignSteel.Chinese_2018.SetOverwrite(groupName, propIndex, propValue, eItemType.Group);
                    break;
                case "Chinese 2010":
                    SapModel.DesignSteel.Chinese_2010.SetOverwrite(groupName, propIndex, propValue, eItemType.Group);
                    break;
            }
        }

        public void SetOverwrite(int    propIndex,
                                 double propValue) {
            switch (DesignCode) {
                case "Chinese 2018":
                    SapModel.DesignSteel.Chinese_2018.SetOverwrite(null,
                                                                   propIndex,
                                                                   propValue,
                                                                   eItemType.SelectedObjects);
                    break;
                case "Chinese 2010":
                    SapModel.DesignSteel.Chinese_2010.SetOverwrite(null,
                                                                   propIndex,
                                                                   propValue,
                                                                   eItemType.SelectedObjects);
                    break;
            }
        }

    #endregion

    #region 设计覆盖项部分

        //private Dictionary<int, PropertyObj> _overwriteDictionary;
        //private List<OverWriteObj>           _overWriteObjs;

        //public List<OverWriteObj> OverWriteObjs {
        //    get => _overWriteObjs;
        //    set => SetProperty(ref _overWriteObjs, value);
        //}

        //private void InitNameDic() {
        //    switch (DesignCode) {
        //        case "Chinese 2010":
        //            _overwriteDictionary = new Dictionary<int, PropertyObj> {
        //                                                                        {
        //                                                                            1, new PropertyObj {
        //                                                                                ChineseName = "框架类型",
        //                                                                                EnglishName
        //                                                                                    = "Framing type"
        //                                                                            }
        //                                                                        }, {
        //                                                                            2, new PropertyObj {
        //                                                                                ChineseName = "构件类型",
        //                                                                                EnglishName
        //                                                                                    = "Element type"
        //                                                                            }
        //                                                                        }, {
        //                                                                            3, new PropertyObj {
        //                                                                                ChineseName = "转换构件？",
        //                                                                                EnglishName
        //                                                                                    = "Is transfer column"
        //                                                                            }
        //                                                                        }, {
        //                                                                            4, new PropertyObj {
        //                                                                                ChineseName = "地震放大系数",
        //                                                                                EnglishName
        //                                                                                    = "Seismic magnification factor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            5, new PropertyObj {
        //                                                                                ChineseName = "轧制截面？",
        //                                                                                EnglishName
        //                                                                                    = "Is rolled section"
        //                                                                            }
        //                                                                        }, {
        //                                                                            6, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "翼缘焰切割的焊接截面？",
        //                                                                                EnglishName
        //                                                                                    = "Is flange edge cut by gas"
        //                                                                            }
        //                                                                        }, {
        //                                                                            7, new PropertyObj {
        //                                                                                ChineseName = "两端铰接？",
        //                                                                                EnglishName
        //                                                                                    = "Is both end pinned"
        //                                                                            }
        //                                                                        }, {
        //                                                                            8, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "忽略宽厚比(B/T)校核？",
        //                                                                                EnglishName
        //                                                                                    = "Ignore b/t check"
        //                                                                            }
        //                                                                        }, {
        //                                                                            9, new PropertyObj {
        //                                                                                ChineseName = "梁按压弯设计？",
        //                                                                                EnglishName
        //                                                                                    = "Classify beam as flexo-compression member"
        //                                                                            }
        //                                                                        }, {
        //                                                                            10, new PropertyObj {
        //                                                                                ChineseName = "梁上翼缘加载",
        //                                                                                EnglishName
        //                                                                                    = " Is beam top loaded"
        //                                                                            }
        //                                                                        }, {
        //                                                                            11, new PropertyObj {
        //                                                                                ChineseName = "挠度校核",
        //                                                                                EnglishName
        //                                                                                    = "Consider deflection"
        //                                                                            }
        //                                                                        }, {
        //                                                                            12, new PropertyObj {
        //                                                                                ChineseName = "挠度校核类型",
        //                                                                                EnglishName
        //                                                                                    = "Deflection check type"
        //                                                                            }
        //                                                                        }, {
        //                                                                            13, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "恒荷载挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "DL deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            14, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "(附加恒载+活载)挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "SDL + LL deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            15, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "活荷载挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "LL deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            16, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "总荷载挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "Total load deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            17, new PropertyObj {
        //                                                                                ChineseName = "净挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "Total camber limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            18, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "恒载限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "DL deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            19, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "(附加恒载+活载)挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "SDL + LL deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            20, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "活荷载挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "LL deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            21, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "总荷载挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "Total load deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            22, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "净挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "Total camber limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            23, new PropertyObj {
        //                                                                                ChineseName = "指定反拱值",
        //                                                                                EnglishName
        //                                                                                    = "Specified camber"
        //                                                                            }
        //                                                                        }, {
        //                                                                            24, new PropertyObj {
        //                                                                                ChineseName = "净/毛面积比",
        //                                                                                EnglishName
        //                                                                                    = "Net area to total area ratio"
        //                                                                            }
        //                                                                        }, {
        //                                                                            25, new PropertyObj {
        //                                                                                ChineseName = "活荷载折减系数",
        //                                                                                EnglishName
        //                                                                                    = "Live load reduction factor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            26, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "无支撑长度系数(主)",
        //                                                                                EnglishName
        //                                                                                    = "Unbraced length ratio, Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            27, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "无支撑长度系数(次)",
        //                                                                                EnglishName
        //                                                                                    = "Unbraced length ratio, Minor Lateral Torsional Buckling"
        //                                                                            }
        //                                                                        }, {
        //                                                                            28, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "有限长度系数 μ(主)",
        //                                                                                EnglishName
        //                                                                                    = "Effective length factor, Mue Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            29, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "有限长度系数 μ(次)",
        //                                                                                EnglishName
        //                                                                                    = "Effective length factor, Mue Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            30, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βm Major",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_m Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            31, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βm Minor",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_m Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            32, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βt Major",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_t Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            33, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βt Minor",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_t Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            34, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "轴向稳定系数 φ Major",
        //                                                                                EnglishName
        //                                                                                    = "Axial stability coefficient, φ Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            35, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "轴向稳定系数 φ Minor",
        //                                                                                EnglishName
        //                                                                                    = "Axial stability coefficient, φ Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            36, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "梁的整体稳定性系数 φ_b Major",
        //                                                                                EnglishName
        //                                                                                    = "Flexural stability coeff, Phi_b Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            37, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "梁的整体稳定性系数 φ_b Minor",
        //                                                                                EnglishName
        //                                                                                    = "Flexural stability coeff, Phi_b Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            38, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "塑性发展系数 γ Major",
        //                                                                                EnglishName
        //                                                                                    = "Plasticity factor, Gamma Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            39, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "塑性发展系数 γ Minor",
        //                                                                                EnglishName
        //                                                                                    = "Plasticity factor, Gamma Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            40, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "截面影响系数 η",
        //                                                                                EnglishName
        //                                                                                    = "Section influence coefficient, Eta"
        //                                                                            }
        //                                                                        }, {
        //                                                                            41, new PropertyObj {
        //                                                                                ChineseName = "强柱系数 η",
        //                                                                                EnglishName
        //                                                                                    = "B/C capacity factor, Eta"
        //                                                                            }
        //                                                                        }, {
        //                                                                            42, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "欧拉弯矩系数 δ Major",
        //                                                                                EnglishName
        //                                                                                    = "Euler moment factor, Delta Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            43, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "欧拉弯矩系数 δ Minor",
        //                                                                                EnglishName
        //                                                                                    = "Euler moment factor, Delta Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            44, new PropertyObj {
        //                                                                                ChineseName = "屈服强度",
        //                                                                                EnglishName
        //                                                                                    = "Yield stress, Fy"
        //                                                                            }
        //                                                                        }, {
        //                                                                            45, new PropertyObj {
        //                                                                                ChineseName = "抗弯强度设计值",
        //                                                                                EnglishName
        //                                                                                    = "Allowable normal stress, f"
        //                                                                            }
        //                                                                        }, {
        //                                                                            46, new PropertyObj {
        //                                                                                ChineseName = "抗剪强度设计值",
        //                                                                                EnglishName
        //                                                                                    = "Allowable shear stress, fv"
        //                                                                            }
        //                                                                        }, {
        //                                                                            47, new PropertyObj {
        //                                                                                ChineseName = "考虑假想应力？",
        //                                                                                EnglishName
        //                                                                                    = "Consider fictitious shear"
        //                                                                            }
        //                                                                        }, {
        //                                                                            48, new PropertyObj {
        //                                                                                ChineseName = "应力比限值？",
        //                                                                                EnglishName
        //                                                                                    = "Demand/capacity ratio limit"
        //                                                                            }
        //                                                                        }, {
        //                                                                            49, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "框架结构剪力放大系数",
        //                                                                                EnglishName
        //                                                                                    = "Dual system magnification factor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            50, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "受压长细比限值？",
        //                                                                                EnglishName
        //                                                                                    = "Lo/r limit in compression"
        //                                                                            }
        //                                                                        }, {
        //                                                                            51, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "受拉长细比限值？",
        //                                                                                EnglishName
        //                                                                                    = "L/r limit in tension"
        //                                                                            }
        //                                                                        }
        //                                                                    };
        //            break;
        //        case "Chinese 2018":
        //            _overwriteDictionary = new Dictionary<int, PropertyObj> {
        //                                                                        {
        //                                                                            1, new PropertyObj {
        //                                                                                ChineseName = "框架类型",
        //                                                                                EnglishName
        //                                                                                    = "Framing type"
        //                                                                            }
        //                                                                        }, {
        //                                                                            2, new PropertyObj {
        //                                                                                ChineseName = "构件类型",
        //                                                                                EnglishName
        //                                                                                    = "Element type"
        //                                                                            }
        //                                                                        }, {
        //                                                                            3, new PropertyObj {
        //                                                                                ChineseName = "转换构件？",
        //                                                                                EnglishName
        //                                                                                    = "Is transfer column"
        //                                                                            }
        //                                                                        }, {
        //                                                                            4, new PropertyObj {
        //                                                                                ChineseName = "地震放大系数",
        //                                                                                EnglishName
        //                                                                                    = "Seismic magnification factor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            5, new PropertyObj {
        //                                                                                ChineseName = "轧制截面？",
        //                                                                                EnglishName
        //                                                                                    = "Is rolled section"
        //                                                                            }
        //                                                                        }, {
        //                                                                            6, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "翼缘焰切割的焊接截面？",
        //                                                                                EnglishName
        //                                                                                    = "Is flange edge cut by gas"
        //                                                                            }
        //                                                                        }, {
        //                                                                            7, new PropertyObj {
        //                                                                                ChineseName = "两端铰接？",
        //                                                                                EnglishName
        //                                                                                    = "Is both end pinned"
        //                                                                            }
        //                                                                        }, {
        //                                                                            8, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "忽略宽厚比(B/T)校核？",
        //                                                                                EnglishName
        //                                                                                    = "Ignore b/t check"
        //                                                                            }
        //                                                                        }, {
        //                                                                            9, new PropertyObj {
        //                                                                                ChineseName = "梁按压弯设计？",
        //                                                                                EnglishName
        //                                                                                    = "Classify beam as flexo-compression member"
        //                                                                            }
        //                                                                        }, {
        //                                                                            10, new PropertyObj {
        //                                                                                ChineseName = "梁上翼缘加载",
        //                                                                                EnglishName
        //                                                                                    = " Is beam top loaded"
        //                                                                            }
        //                                                                        }, {
        //                                                                            11, new PropertyObj {
        //                                                                                ChineseName = "挠度校核",
        //                                                                                EnglishName
        //                                                                                    = "Consider deflection"
        //                                                                            }
        //                                                                        }, {
        //                                                                            12, new PropertyObj {
        //                                                                                ChineseName = "挠度校核类型",
        //                                                                                EnglishName
        //                                                                                    = "Deflection check type"
        //                                                                            }
        //                                                                        }, {
        //                                                                            13, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "恒荷载挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "DL deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            14, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "(附加恒载+活载)挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "SDL + LL deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            15, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "活荷载挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "LL deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            16, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "总荷载挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "Total load deflection limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            17, new PropertyObj {
        //                                                                                ChineseName = "净挠度限值L/",
        //                                                                                EnglishName
        //                                                                                    = "Total camber limit, L/Value"
        //                                                                            }
        //                                                                        }, {
        //                                                                            18, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "恒载限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "DL deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            19, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "(附加恒载+活载)挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "SDL + LL deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            20, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "活荷载挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "LL deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            21, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "总荷载挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "Total load deflection limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            22, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "净挠度限值 abs L/",
        //                                                                                EnglishName
        //                                                                                    = "Total camber limit, absolute"
        //                                                                            }
        //                                                                        }, {
        //                                                                            23, new PropertyObj {
        //                                                                                ChineseName = "指定反拱值",
        //                                                                                EnglishName
        //                                                                                    = "Specified camber"
        //                                                                            }
        //                                                                        }, {
        //                                                                            24, new PropertyObj {
        //                                                                                ChineseName = "净/毛面积比",
        //                                                                                EnglishName
        //                                                                                    = "Net area to total area ratio"
        //                                                                            }
        //                                                                        }, {
        //                                                                            25, new PropertyObj {
        //                                                                                ChineseName = "活荷载折减系数",
        //                                                                                EnglishName
        //                                                                                    = "Live load reduction factor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            26, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "无支撑长度系数(主)",
        //                                                                                EnglishName
        //                                                                                    = "Unbraced length ratio, Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            27, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "无支撑长度系数(次)",
        //                                                                                EnglishName
        //                                                                                    = "Unbraced length ratio, Minor Lateral Torsional Buckling"
        //                                                                            }
        //                                                                        }, {
        //                                                                            28, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "有限长度系数 μ(主)",
        //                                                                                EnglishName
        //                                                                                    = "Effective length factor, Mue Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            29, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "有限长度系数 μ(次)",
        //                                                                                EnglishName
        //                                                                                    = "Effective length factor, Mue Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            30, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βm Major",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_m Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            31, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βm Minor",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_m Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            32, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βt Major",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_t Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            33, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "弯矩系数 βt Minor",
        //                                                                                EnglishName
        //                                                                                    = "Moment coefficient, Beta_t Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            34, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "轴向稳定系数 φ Major",
        //                                                                                EnglishName
        //                                                                                    = "Axial stability coefficient, φ Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            35, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "轴向稳定系数 φ Minor",
        //                                                                                EnglishName
        //                                                                                    = "Axial stability coefficient, φ Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            36, new PropertyObj {
        //                                                                                ChineseName = "???????",
        //                                                                                EnglishName = "???????,"
        //                                                                            }
        //                                                                        }, {
        //                                                                            37, new PropertyObj {
        //                                                                                ChineseName = "???????",
        //                                                                                EnglishName = "???????"
        //                                                                            }
        //                                                                        }, {
        //                                                                            38, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "塑性发展系数 γ Major",
        //                                                                                EnglishName
        //                                                                                    = "Plasticity factor, Gamma Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            39, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "塑性发展系数 γ Minor",
        //                                                                                EnglishName
        //                                                                                    = "Plasticity factor, Gamma Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            40, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "截面影响系数 η",
        //                                                                                EnglishName
        //                                                                                    = "Section influence coefficient, Eta"
        //                                                                            }
        //                                                                        }, {
        //                                                                            41, new PropertyObj {
        //                                                                                ChineseName = "强柱系数 η",
        //                                                                                EnglishName
        //                                                                                    = "B/C capacity factor, Eta"
        //                                                                            }
        //                                                                        }, {
        //                                                                            42, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "欧拉弯矩系数 δ Major",
        //                                                                                EnglishName
        //                                                                                    = "Euler moment factor, Delta Major"
        //                                                                            }
        //                                                                        }, {
        //                                                                            43, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "欧拉弯矩系数 δ Minor",
        //                                                                                EnglishName
        //                                                                                    = "Euler moment factor, Delta Minor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            44, new PropertyObj {
        //                                                                                ChineseName = "屈服强度",
        //                                                                                EnglishName
        //                                                                                    = "Yield stress, Fy"
        //                                                                            }
        //                                                                        }, {
        //                                                                            45, new PropertyObj {
        //                                                                                ChineseName = "抗弯强度设计值",
        //                                                                                EnglishName
        //                                                                                    = "Allowable normal stress, f"
        //                                                                            }
        //                                                                        }, {
        //                                                                            46, new PropertyObj {
        //                                                                                ChineseName = "抗剪强度设计值",
        //                                                                                EnglishName
        //                                                                                    = "Allowable shear stress, fv"
        //                                                                            }
        //                                                                        }, {
        //                                                                            47, new PropertyObj {
        //                                                                                ChineseName = "考虑假想应力？",
        //                                                                                EnglishName
        //                                                                                    = "Consider fictitious shear"
        //                                                                            }
        //                                                                        }, {
        //                                                                            48, new PropertyObj {
        //                                                                                ChineseName = "应力比限值？",
        //                                                                                EnglishName
        //                                                                                    = "Demand/capacity ratio limit"
        //                                                                            }
        //                                                                        }, {
        //                                                                            49, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "框架结构剪力放大系数？",
        //                                                                                EnglishName
        //                                                                                    = "Dual system magnification factor"
        //                                                                            }
        //                                                                        }, {
        //                                                                            50, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "受压长细比限值？",
        //                                                                                EnglishName
        //                                                                                    = "Lo/r limit in compression"
        //                                                                            }
        //                                                                        }, {
        //                                                                            51, new PropertyObj {
        //                                                                                ChineseName
        //                                                                                    = "受拉长细比限值？",
        //                                                                                EnglishName
        //                                                                                    = "L/r limit in tension"
        //                                                                            }
        //                                                                        }, {
        //                                                                            52, new PropertyObj {
        //                                                                                ChineseName = "???????",
        //                                                                                EnglishName = "???????"
        //                                                                            }
        //                                                                        }
        //                                                                    };
        //            break;
        //    }
        //}

        //private void InitOverwriteList() {
        //    Dictionary<int, string> canModifyDic;
        //    switch (DesignCode) {
        //        case "Chinese 2018":
        //            canModifyDic = new Dictionary<int, string> {
        //                                                           { 16, "总荷载挠度限值" },
        //                                                           { 24, "净/毛面积比" },
        //                                                           { 26, "无支撑长度系数(主)" },
        //                                                           { 27, "无支撑长度系数(次)" },
        //                                                           { 28, "有效长度系数(主)" },
        //                                                           { 29, "有效长度系数(次)" },
        //                                                           { 36, "梁的整体稳定性系数(主)" },
        //                                                           { 37, "梁的整体稳定性系数(次)" },
        //                                                           { 38, "塑性发展系数(主)" },
        //                                                           { 39, "塑性发展系数(次)" },
        //                                                           { 40, "截面影响系数" },
        //                                                           { 41, "强柱系数" },
        //                                                           { 44, "屈服应力" },
        //                                                           { 45, "抗弯强度设计值" },
        //                                                           { 46, "抗剪强度设计值" },
        //                                                           { 48, "应力比限值" },
        //                                                           { 50, "受压长细比限值" },
        //                                                           { 51, "受拉长细比限值" }
        //                                                       };
        //            OverWriteObjs = [];
        //            for (var i = 0; i < 52; i++) {
        //                double value = 0;
        //                var    flag  = false;
        //                var obj = new OverWriteObj {
        //                                               Index = i + 1
        //                                           };
        //                if (canModifyDic.ContainsKey(obj.Index)) {
        //                    SapModel.DesignSteel.Chinese_2018.GetOverwrite(FrameNameList.First(),
        //                                                                   obj.Index,
        //                                                                   ref value,
        //                                                                   ref flag);
        //                    obj.Value       = value;
        //                    obj.DisplayName = canModifyDic[obj.Index];
        //                    obj.IsDefault   = flag;
        //                    obj.Property    = _overwriteDictionary[obj.Index];
        //                    obj.NeedModify  = false;
        //                    OverWriteObjs.Add(obj);
        //                }
        //            }

        //            // 暂时支持更改的为抗弯强度设计值和抗剪强度设计值
        //            //double value = 0;
        //            //var    flag  = false;
        //            //var    obj   = new OverWriteObj { Index = 45 };
        //            //_sapModel.DesignSteel.Chinese_2018.GetOverwrite("All", obj.Index, ref value, ref flag);
        //            //obj.Value     = value;
        //            //obj.IsDefault = flag;
        //            //obj.Property  = _overwriteDictionary[obj.Index];
        //            //OverWriteObjs.Add(obj);
        //            //obj = new OverWriteObj { Index = 46 };
        //            //_sapModel.DesignSteel.Chinese_2018.GetOverwrite("All", obj.Index, ref value, ref flag);
        //            //obj.Value     = value;
        //            //obj.IsDefault = flag;
        //            //obj.Property  = _overwriteDictionary[obj.Index];
        //            //OverWriteObjs.Add(obj);
        //            break;
        //        case "Chinese 2010":
        //            canModifyDic = new Dictionary<int, string> {
        //                                                           { 16, "总荷载挠度限值" },
        //                                                           { 24, "净/毛面积比" },
        //                                                           { 26, "无支撑长度系数(主)" },
        //                                                           { 27, "无支撑长度系数(次)" },
        //                                                           { 28, "有效长度系数(主)" },
        //                                                           { 29, "有效长度系数(次)" },
        //                                                           { 36, "梁的整体稳定性系数(主)" },
        //                                                           { 37, "梁的整体稳定性系数(次)" },
        //                                                           { 38, "塑性发展系数(主)" },
        //                                                           { 39, "塑性发展系数(次)" },
        //                                                           { 40, "截面影响系数" },
        //                                                           { 41, "强柱系数" },
        //                                                           { 44, "屈服应力" },
        //                                                           { 45, "抗弯强度设计值" },
        //                                                           { 46, "抗剪强度设计值" },
        //                                                           { 48, "应力比限值" },
        //                                                           { 50, "受压长细比限值" },
        //                                                           { 51, "受拉长细比限值" }
        //                                                       };
        //            OverWriteObjs = [];
        //            for (var i = 0; i < 51; i++) {
        //                double value = 0;
        //                var    flag  = false;
        //                var obj = new OverWriteObj {
        //                                               Index = i + 1
        //                                           };
        //                if (canModifyDic.ContainsKey(obj.Index)) {
        //                    SapModel.DesignSteel.Chinese_2010.GetOverwrite(FrameNameList.First(),
        //                                                                   obj.Index,
        //                                                                   ref value,
        //                                                                   ref flag);
        //                    obj.Value       = value;
        //                    obj.DisplayName = canModifyDic[obj.Index];
        //                    obj.IsDefault   = flag;
        //                    obj.Property    = _overwriteDictionary[obj.Index];
        //                    obj.NeedModify  = false;
        //                    OverWriteObjs.Add(obj);
        //                }
        //            }

        //            break;
        //    }
        //}

    #endregion
    }
}