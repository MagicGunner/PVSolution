using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP2000v1;
using SapToolBox.Base.Sap2000;

namespace TestSapApi {
    internal class Program {
        static void Main(string[] args) {
            cHelper myHelper  = new Helper();
            var     sapObject = myHelper.GetObject("CSI.SAP2000.API.SapObject");
            var     sapModel  = sapObject.SapModel;

            //double Value   = 0;
            //bool   ProgDet = false;

            //var resultList = new List<OverWriteObj>();
            //for (int i = 0; i < 52; i++) {
            //    double value = 0;
            //    bool   flag  = false;
            //    var    obj   = new OverWriteObj();
            //    obj.Index = i + 1;
            //    sapModel.DesignSteel.Chinese_2010.GetOverwrite("10", obj.Index, ref value, ref flag);
            //    obj.Value     = value;
            //    obj.IsDefault = flag;
            //    resultList.Add(obj);
            //    Console.WriteLine($@"当前属性序号为：{obj.Index}, 属性值为：{obj.Value}, 程序指定为：{obj.IsDefault}");
            //}

            int      TableVersion1       = 0,    NumberRecords1 = 0;
            string[] FieldsKeysIncluded1 = null, TableData1     = null;
            sapModel.DatabaseTables.GetTableForEditingArray("Section Designer Properties 16 - Shape Polygon", null,
                                                            ref TableVersion1, ref FieldsKeysIncluded1,
                                                            ref NumberRecords1, ref TableData1);
            TableData1[0] = "ssssss";


            int      TableVersion       = 0;
            string[] FieldsKeysIncluded = null;
            sapModel.DatabaseTables.SetTableForEditingArray("Section Designer Properties 16 - Shape Polygon",
                                                            ref TableVersion, ref FieldsKeysIncluded, 0,
                                                            ref TableData1);

            //var mySapModel = new SapModelHelper(sapModel);
            //mySapModel.SetOverwrite("52", 45, 999);
            //mySapModel.DesignCode = "Chinese 2010";
            //var x = mySapModel.DesignCode;

            //int      NumberNames = 0;
            //string[] MyName      = null;
            //var      x           = sapModel.GroupDef.GetNameList(ref NumberNames, ref MyName);

            //var sapHelper = new SapModelHelper(ref sapModel);
        }
    }
}
