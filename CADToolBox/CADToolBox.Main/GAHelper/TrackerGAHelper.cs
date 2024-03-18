using System.Net.Mime;
using CADToolBox.Shared.Models.CADModels.Implement;
using IFoxCAD.Cad;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace CADToolBox.Main.GAHelper;

// 此处增加绘图辅助函数
public class TrackerGAHelper(
    TrackerModel trackerModel,
    DBTrans      trans,
    Point3d      insertPoint
) {
    private TrackerModel GAModel { get; } = trackerModel;

    private DBTrans CurrentTrans { get; } = trans;

    private string CompanyName { get; set; } = "Linsum"; // 公司名称

    private string TrackerType {
        get {
            if (GAModel.ModuleRowCounter > 1) return GAModel.HasSlew ? "D2" : "D1"; // 1P系统
            if (GAModel.HasSlew) return GAModel.DriveNum > 1 ? "S3" : "S2";
            return "S1";
        }
    } // 支架类型S1，S2、、、


    private double DimHeight { get; set; } = 6.0; // 全局标注高度,未缩放

    private double TextSize { get; set; } = 3.0; // 全局字体高度,未缩放

    private double FrameScale { get; set; } // 全局缩放比例

    private double MaxPileEmbedment { get; set; } // 最大埋深

    // 导入模板文件块
    public void ImportBlockFromDwg(string dwgName) {
        var tempDB = new Database(false, true); // 创建临时数据库
        try {
            tempDB.ReadDwgFile(dwgName, System.IO.FileShare.Read, true, null);
            var objectIds = new ObjectIdCollection();
            foreach (var objectId in CurrentTrans.BlockTable) {
                var btr = CurrentTrans.GetObject(objectId) as BlockTableRecord;
                if (btr != null && btr is { IsAnonymous: false, IsLayout: false }) {
                    if (btr.Name.Length > 3 && btr.Name.Substring(0, 3) == "00-") {
                        objectIds.Add(objectId);
                    }
                }
            }

            tempDB.WblockCloneObjects(objectIds, CurrentTrans.Database.BlockTableId, new IdMapping(),
                                      DuplicateRecordCloning.Replace, false);
        } catch (AcException ex) {
            Acaop.ShowAlertDialog("出现错误：" + ex.Message);
        }

        tempDB.Dispose();
    }

    public void InitStyles() {
        Acaop.SetSystemVariable("DIMBLK", "_ARCHTICK");
        // 初始化字体
        if (!CurrentTrans.TextStyleTable.Has(CompanyName + "Font")) {
            try {
                CurrentTrans.TextStyleTable.Add(CompanyName + "Font", newTextStyle => {
                                                                          newTextStyle.FileName = "gbeitc.shx";
                                                                          newTextStyle.TextSize = TextSize;
                                                                          newTextStyle.XScale   = 0.8; // 字体宽度系数
                                                                      });
            } catch {
                MessageBox.Show("缺少GB字体，使用标准字体代替");
                CurrentTrans.TextStyleTable.Add(CompanyName, FontTTF.仿宋GB2312, 0.8);
            }
        }

        // 初始化标注样式
        if (!CurrentTrans.DimStyleTable.Has(CompanyName + "Dim")) {
            CurrentTrans.DimStyleTable.Add(CompanyName + "Dim", newDimStyle => {
                                                                    newDimStyle.Dimclrd
                                                                        = Color.FromColorIndex(ColorMethod.ByAci, 4);
                                                                    newDimStyle.Dimclre
                                                                        = Color.FromColorIndex(ColorMethod.ByAci, 4);
                                                                    newDimStyle.Dimclrt
                                                                        = Color.FromColorIndex(ColorMethod.ByAci, 3);
                                                                    newDimStyle.Dimtxt = 3.0; // 文字高度
                                                                    newDimStyle.Dimdec = 0;   // 标注取整
                                                                    newDimStyle.Dimasz = 1;   // 表示尺寸线箭头，引线箭头和勾线的大小
                                                                    newDimStyle.Dimtxsty
                                                                        = CurrentTrans.TextStyleTable
                                                                            [CompanyName + "Font"]; // 设置标注字体
                                                                    newDimStyle.Dimblk
                                                                        = CurrentTrans.BlockTable["DIMBLK"]; // 设置箭头样式
                                                                    newDimStyle.Dimtad = 1;                  // 字体在标注线上方
                                                                });
        }

        // 自定义图层
        if (!CurrentTrans.LayerTable.Has("01-Dimension")) { // 标注图层
            CurrentTrans.LayerTable.Add("01-Dimension",
                                        newLayer => { newLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 4); });
        }

        // 旋转部图层
        if (!CurrentTrans.LayerTable.Has("02-Rotation")) { // 旋转部图层
            CurrentTrans.LayerTable.Add("02-Rotation", newLayer => {
                                                           newLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 6);
                                                           //if (!CurrentTrans.LinetypeTable.Has("test")) {
                                                           //    CurrentTrans.LinetypeTable.Add("test",
                                                           //        newLineType => {
                                                           //            newLineType.Database
                                                           //               .LoadLineTypeFile("HIDDEN", "acad.lin");
                                                           //        });
                                                           //}
                                                           newLayer.LinetypeObjectId
                                                               = CurrentTrans.LinetypeTable["HIDDEN"];
                                                       });
        }

        // 文字图层
        if (!CurrentTrans.LayerTable.Has("03-Text")) {
            CurrentTrans.LayerTable.Add("03-Text",
                                        newLayer => { newLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 3); });
        }
    }

    public void GetGA() {
        // 计算缩放比例
        // 主梁数据和立柱数据初始化完成来设置图形中相关的缩放比例
        MaxPileEmbedment = GAModel.PostList!.Max(post => post.PileDownGround);      // 最大埋深
        var frameScaleX = (GAModel.SystemLength * 1.2 + GAModel.Chord * 1.5) / 420; // 水平缩放比例
        var frameScaleY = GAModel.ModuleHeight + GAModel.PurlinHeight + GAModel.BeamCenterToGround +
                          MaxPileEmbedment; // 竖直缩放比例,考虑主视图系统总高度
        frameScaleY += GAModel.ModuleLength * 2 + GAModel.Chord +
                       GAModel.ModuleLength * 2; // 考虑主视图上下各留一个组件高度的距离，俯视图上下各留一个组件高度的距离
        frameScaleY += GAModel.ModuleLength * 0.5 + GAModel.Chord * Math.Sin(GAModel.MaxAngle) +
                       GAModel.BeamCenterToGround; // 考虑侧视图上下各留0.5个组件高度的距离
        frameScaleY *= 2;
        frameScaleY /= 297;

        FrameScale = frameScaleX > frameScaleY ? frameScaleX : frameScaleY;
        var frameHeight = FrameScale * 297; // 图框实际高度
        var frameWidth  = FrameScale * 420; // 图框实际宽度


        // 插入图框
        // 输入图框的属性
        CurrentTrans.CurrentSpace.InsertBlock(insertPoint, CurrentTrans.BlockTable["00-" + CompanyName + "-中文图框"],
                                              new Scale3d(FrameScale, FrameScale, FrameScale));

        // 原图框上下左右内边距分别为8,5,5,5
        double[] frameMargin  = [8  * FrameScale, 5  * FrameScale, 5  * FrameScale, 5  * FrameScale]; // 绘图区域外边距
        double[] framePadding = [16 * FrameScale, 62 * FrameScale, 16 * FrameScale, 16 * FrameScale]; // 绘图区域内边距

        var leftUp = new Point3d(insertPoint.X + frameMargin[2] + framePadding[2],
                                 insertPoint.Y - frameMargin[0] - framePadding[0], 0);
        var rightDown = new Point3d(insertPoint.X + frameWidth  - frameMargin[3] - framePadding[3],
                                    insertPoint.Y - frameHeight + frameMargin[1] + framePadding[1], 0);

        CurrentTrans.CurrentSpace.AddEntity(new Line(leftUp, rightDown));
        // 主视图绘制
        DrawFrontView(new Point3d(leftUp.X, leftUp.Y - DimHeight * 3, 0));
    }

    /// <summary>
    /// 绘制主视图
    /// </summary>
    /// <param name="FrontViewInsertPoint">主梁中心线最左侧的点</param>
    public void DrawFrontView(Point3d FrontViewInsertPoint) {
        // 绘制主梁，使用多段线
        if (GAModel.BeamList != null) {
            for (var i = 0; i < GAModel.BeamList!.Count; i++) {
                var pts = new List<Point3d> {
                                                new(GAModel.BeamList[i].StartX + FrontViewInsertPoint.X,
                                                    FrontViewInsertPoint.Y -
                                                    GAModel.BeamHeight / (1 + GAModel.BeamRadio), 0), // 左下
                                                new(GAModel.BeamList[i].StartX + FrontViewInsertPoint.X,
                                                    FrontViewInsertPoint.Y + GAModel.BeamHeight * GAModel.BeamRadio /
                                                    (1 + GAModel.BeamRadio), 0), // 左上
                                                new(GAModel.BeamList[i].EndX + FrontViewInsertPoint.X,
                                                    FrontViewInsertPoint.Y + GAModel.BeamHeight * GAModel.BeamRadio /
                                                    (1 + GAModel.BeamRadio), 0), // 右上
                                                new(GAModel.BeamList[i].EndX + FrontViewInsertPoint.X,
                                                    FrontViewInsertPoint.Y -
                                                    GAModel.BeamHeight / (1 + GAModel.BeamRadio), 0) // 右下
                                            };
                // 切换默认图层绘制主梁
                if (CurrentTrans.LayerTable.Has("0")) CurrentTrans.Database.Clayer = CurrentTrans.LayerTable["0"];
                CurrentTrans.CurrentSpace.AddEntity(pts.CreatePolyline(p => p.Closed = true));
                // 切换文字图层添加主梁规格
                if (CurrentTrans.LayerTable.Has("03-Text"))
                    CurrentTrans.Database.Clayer = CurrentTrans.LayerTable["03-Text"];
                //var beamSection = new DBText();
                //beamSection.Justify = AttachmentPoint.BaseCenter;
                //beamSection.AlignmentPoint
                //    = new Point3d(FrontViewInsertPoint.X + (GAModel.BeamList[i].StartX + GAModel.BeamList[i].EndX) / 2,
                //                  FrontViewInsertPoint.Y +
                //                  GAModel.BeamHeight * GAModel.BeamRadio / (1 + GAModel.BeamRadio) +
                //                  GAModel.PurlinHeight + GAModel.ModuleHeight + TextSize * FrameScale * 0.1, 0);
                //beamSection.Height      = TextSize * FrameScale * 0.5;
                //beamSection.TextString  = GAModel.BeamList[i].Section;
                //beamSection.TextStyleId = CurrentTrans.TextStyleTable[CompanyName + "Font"];
                //CurrentTrans.CurrentSpace.AddEntity(beamSection);
                CurrentTrans.CurrentSpace.AddEntity(new DBText {
                                                                   Justify = AttachmentPoint.BaseCenter,
                                                                   AlignmentPoint
                                                                       = new
                                                                           Point3d(FrontViewInsertPoint.X + (GAModel.BeamList[i].StartX + GAModel.BeamList[i].EndX) / 2,
                                                                               FrontViewInsertPoint.Y +
                                                                               GAModel.BeamHeight * GAModel.BeamRadio /
                                                                               (1 + GAModel.BeamRadio) +
                                                                               GAModel.PurlinHeight    +
                                                                               GAModel.ModuleHeight    +
                                                                               TextSize * FrameScale * 0.1, 0),
                                                                   Height     = TextSize * FrameScale * 0.5,
                                                                   TextString = GAModel.BeamList[i].Section,
                                                                   TextStyleId
                                                                       = CurrentTrans.TextStyleTable
                                                                           [CompanyName + "Font"]
                                                               });
            }
        }

        // 绘制檩条
        if (GAModel.PurlinList != null) {
            // 切换默认图层绘制檩条
            if (CurrentTrans.LayerTable.Has("0")) CurrentTrans.Database.Clayer = CurrentTrans.LayerTable["0"];
            var purlinStartPoint = new Point3d(FrontViewInsertPoint.X, FrontViewInsertPoint.Y, 0);
            var purlinNamePrefix = "00-" + CompanyName + "-正视图-" + GAModel.ModuleRowCounter + "P";
            var leftPurlinObj
                = CurrentTrans.GetObject(CurrentTrans.CurrentSpace.InsertBlock(purlinStartPoint,
                                                                               CurrentTrans.BlockTable
                                                                                   [purlinNamePrefix + "左檩条"])) as
                      BlockReference;
            var midPurlinObj
                = CurrentTrans.GetObject(CurrentTrans.CurrentSpace.InsertBlock(purlinStartPoint,
                                                                               CurrentTrans.BlockTable
                                                                                   [purlinNamePrefix + "中檩条"])) as
                      BlockReference;
            var rightPurlinObj
                = CurrentTrans.GetObject(CurrentTrans.CurrentSpace.InsertBlock(purlinStartPoint,
                                                                               CurrentTrans.BlockTable
                                                                                   [purlinNamePrefix + "右檩条"])) as
                      BlockReference;
            var purlinDic = new Dictionary<string, double> {
                                                               { "檩条高度", GAModel.PurlinHeight }, {
                                                                   "主梁上半高度",
                                                                   GAModel.BeamHeight * GAModel.BeamRadio /
                                                                   (1 + GAModel.BeamRadio)
                                                               }, {
                                                                   "主梁下半高度",
                                                                   GAModel.BeamHeight / (1 + GAModel.BeamRadio)
                                                               }
                                                           };
            SetDynamicBlock(leftPurlinObj, purlinDic);
            SetDynamicBlock(midPurlinObj, purlinDic);
            SetDynamicBlock(rightPurlinObj, purlinDic);
            foreach (var purlinModel in GAModel.PurlinList) {
                var purlinVector
                    = purlinStartPoint.GetVectorTo(new Point3d(FrontViewInsertPoint.X + purlinModel.X,
                                                               FrontViewInsertPoint.Y, 0));
                BlockReference tempPurlinObj;
                switch (purlinModel.Type) {
                    case -1: // 左檩条
                        tempPurlinObj = leftPurlinObj.Clone() as BlockReference;
                        tempPurlinObj?.TransformBy(Matrix3d.Displacement(purlinVector));
                        if (tempPurlinObj != null) CurrentTrans.CurrentSpace.AddEntity(tempPurlinObj);
                        break;
                    case 1: // 右檩条
                        tempPurlinObj = rightPurlinObj.Clone() as BlockReference;
                        tempPurlinObj?.TransformBy(Matrix3d.Displacement(purlinVector));
                        if (tempPurlinObj != null) CurrentTrans.CurrentSpace.AddEntity(tempPurlinObj);
                        break;
                    case 0: // 中檩条
                        tempPurlinObj = midPurlinObj.Clone() as BlockReference;
                        tempPurlinObj?.TransformBy(Matrix3d.Displacement(purlinVector));
                        if (tempPurlinObj != null) CurrentTrans.CurrentSpace.AddEntity(tempPurlinObj);
                        break;
                }
            }

            leftPurlinObj?.Erase();
            midPurlinObj?.Erase();
            rightPurlinObj?.Erase();
        }

        // 绘制立柱
        if (GAModel.PostList != null) {
            // 切换默认图层绘制立柱
            if (CurrentTrans.LayerTable.Has("0")) CurrentTrans.Database.Clayer = CurrentTrans.LayerTable["0"];
            var postDic                                                        = new Dictionary<string, double>();
            foreach (var postModel in GAModel.PostList) {
                postDic["主梁中心与基础顶面距离"] = GAModel.BeamCenterToGround - postModel.PileUpGround;
                postDic["法兰板厚度"]       = postModel.FlangeThickness;
                postDic["基础露头"]        = postModel.PileUpGround;
                postDic["基础埋深"]        = postModel.PileDownGround;
                string postBlockName;
                if (postModel.PileUpGround > 0) { // 立柱露头大于0默认为混凝土桩
                    postBlockName = postModel.IsDrive
                                        ? "00-" + CompanyName + "-正视图-" + TrackerType + "-驱动立柱-PHC"
                                        : "00-" + CompanyName + "-正视图-" + TrackerType + "-普通立柱-PHC";
                } else { // 立柱露头为0表示锤入桩
                    postBlockName = postModel.IsDrive
                                        ? "00-" + CompanyName + "-正视图-" + TrackerType + "-驱动立柱"
                                        : "00-" + CompanyName + "-正视图-" + TrackerType + "-普通立柱";
                }

                var postId
                    = CurrentTrans.CurrentSpace
                                  .InsertBlock(new Point3d(FrontViewInsertPoint.X + postModel.X, FrontViewInsertPoint.Y, 0),
                                               CurrentTrans.BlockTable[postBlockName]); // 插入立柱块
                var postObj = CurrentTrans.GetObject(postId) as BlockReference;
                SetDynamicBlock(postObj, postDic); // 设置立柱动态块属性

                // 添加立柱规格
                if (CurrentTrans.LayerTable.Has("03-Text"))
                    CurrentTrans.Database.Clayer = CurrentTrans.LayerTable["03-Text"];

                CurrentTrans.CurrentSpace.AddEntity(new DBText {
                                                                   Justify = AttachmentPoint.TopCenter,
                                                                   AlignmentPoint
                                                                       = new
                                                                           Point3d(FrontViewInsertPoint.X + postModel.X,
                                                                               FrontViewInsertPoint.Y     -
                                                                               GAModel.BeamCenterToGround -
                                                                               MaxPileEmbedment           -
                                                                               TextSize * FrameScale * 0.1, 0),
                                                                   Height     = TextSize * FrameScale * 0.5,
                                                                   TextString = postModel.Section,
                                                                   TextStyleId
                                                                       = CurrentTrans.TextStyleTable
                                                                           [CompanyName + "Font"]
                                                               });
                CurrentTrans.CurrentSpace.AddEntity(new DBText {
                                                                   Justify = AttachmentPoint.TopCenter,
                                                                   AlignmentPoint
                                                                       = new
                                                                           Point3d(FrontViewInsertPoint.X + postModel.X,
                                                                               FrontViewInsertPoint.Y      -
                                                                               GAModel.BeamCenterToGround  -
                                                                               MaxPileEmbedment            -
                                                                               TextSize * FrameScale * 0.5 -
                                                                               TextSize * FrameScale * 0.1, 0),
                                                                   Height = TextSize * FrameScale * 0.5,
                                                                   TextString
                                                                       = postModel.IsDrive
                                                                             ? "DrivePost"
                                                                             : "GeneralPost",
                                                                   TextStyleId
                                                                       = CurrentTrans.TextStyleTable
                                                                           [CompanyName + "Font"]
                                                               });
            }
        }
    }


    // 修改动态块的属性
    public static void SetDynamicBlock(BlockReference?            brf,
                                       Dictionary<string, double> dicData) {
        var props = brf?.DynamicBlockReferencePropertyCollection;
        if (props == null) return;
        foreach (DynamicBlockReferenceProperty prop in props) {
            if (dicData.TryGetValue(prop.PropertyName, out var value)) {
                prop.Value = value;
            }
        }
    }
}