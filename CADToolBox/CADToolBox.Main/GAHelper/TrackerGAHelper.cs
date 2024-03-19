using System.Net.Mime;
using System.Windows;
using CADToolBox.Shared.Models.CADModels.Implement;
using IFoxCAD.Cad;
using MessageBox = System.Windows.MessageBox;
using Point3d = Autodesk.AutoCAD.Geometry.Point3d;

namespace CADToolBox.Main.GAHelper;

// 此处增加绘图辅助函数
public class TrackerGAHelper(
    TrackerModel trackerModel,
    DBTrans      trans,
    Point3d      insertPoint
) {
    private TrackerModel GAModel { get; } = trackerModel;

    private DBTrans trans { get; } = trans;

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
            foreach (var objectId in trans.BlockTable) {
                var btr = trans.GetObject(objectId) as BlockTableRecord;
                if (btr != null && btr is { IsAnonymous: false, IsLayout: false }) {
                    if (btr.Name.Length > 3 && btr.Name.Substring(0, 3) == "00-") {
                        objectIds.Add(objectId);
                    }
                }
            }

            tempDB.WblockCloneObjects(objectIds, trans.Database.BlockTableId, new IdMapping(), DuplicateRecordCloning.Replace, false);
        } catch (AcException ex) {
            Acaop.ShowAlertDialog("出现错误：" + ex.Message);
        }

        tempDB.Dispose();
    }

    // 批量更新块名称
    public void UpdateBlockName(string oldCompanyName, string newCompanyName) {
        var btrs = trans.BlockTable.GetRecords().Where(btr => btr != null && btr is { IsAnonymous: false, IsLayout: false } && btr.Name.Length > 3 && btr.Name[..3] == "00-");
        foreach (var btr in btrs) {
            trans.BlockTable.Change(btr.Name, btr => { btr.Name = btr.Name.Replace(oldCompanyName, newCompanyName); });
        }
    }

    public void InitStyles() {
        Acaop.SetSystemVariable("DIMBLK", "_ARCHTICK");
        // 初始化字体
        if (!trans.TextStyleTable.Has(CompanyName + "Font")) {
            try {
                trans.TextStyleTable.Add(CompanyName + "Font", newTextStyle => {
                                                                   newTextStyle.FileName = "gbeitc.shx";
                                                                   newTextStyle.TextSize = TextSize;
                                                                   newTextStyle.XScale = 0.8; // 字体宽度系数
                                                               });
            } catch {
                MessageBox.Show("缺少GB字体，使用标准字体代替");
                trans.TextStyleTable.Add(CompanyName, FontTTF.仿宋GB2312, 0.8);
            }
        }

        // 初始化标注样式
        if (!trans.DimStyleTable.Has(CompanyName + "Dim")) {
            trans.DimStyleTable.Add(CompanyName + "Dim", newDimStyle => {
                                                             newDimStyle.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 4);
                                                             newDimStyle.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 4);
                                                             newDimStyle.Dimclrt = Color.FromColorIndex(ColorMethod.ByAci, 3);
                                                             newDimStyle.Dimtxt = 3.0;                                          // 文字高度
                                                             newDimStyle.Dimdec = 0;                                            // 标注取整
                                                             newDimStyle.Dimasz = 1;                                            // 表示尺寸线箭头，引线箭头和勾线的大小
                                                             newDimStyle.Dimtxsty = trans.TextStyleTable[CompanyName + "Font"]; // 设置标注字体
                                                             newDimStyle.Dimblk = trans.BlockTable["DIMBLK"];                   // 设置箭头样式
                                                             newDimStyle.Dimtad = 1;                                            // 字体在标注线上方
                                                         });
        }

        // 自定义图层
        if (!trans.LayerTable.Has("01-Dimension")) { // 标注图层
            trans.LayerTable.Add("01-Dimension", newLayer => { newLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 4); });
        }

        // 旋转部图层
        if (!trans.LayerTable.Has("02-Rotation")) { // 旋转部图层
            trans.LayerTable.Add("02-Rotation", newLayer => {
                                                    newLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 6);
                                                    //if (!trans.LinetypeTable.Has("test")) {
                                                    //    trans.LinetypeTable.Add("test",
                                                    //        newLineType => {
                                                    //            newLineType.Database
                                                    //               .LoadLineTypeFile("HIDDEN", "acad.lin");
                                                    //        });
                                                    //}
                                                    newLayer.LinetypeObjectId = trans.LinetypeTable["HIDDEN"];
                                                });
        }

        // 文字图层
        if (!trans.LayerTable.Has("03-Text")) {
            trans.LayerTable.Add("03-Text", newLayer => { newLayer.Color = Color.FromColorIndex(ColorMethod.ByAci, 3); });
        }
    }

    public void GetGA() {
        // 计算缩放比例
        // 主梁数据和立柱数据初始化完成来设置图形中相关的缩放比例
        MaxPileEmbedment = GAModel.PostList!.Max(post => post.PileDownGround);                                               // 最大埋深
        var frameScaleX = (GAModel.SystemLength * 1.2 + GAModel.Chord * 1.5) / 420;                                          // 水平缩放比例
        var frameScaleY = GAModel.ModuleHeight + GAModel.PurlinHeight + GAModel.BeamCenterToGround + MaxPileEmbedment;       // 竖直缩放比例,考虑主视图系统总高度
        frameScaleY += GAModel.ModuleLength * 2 + GAModel.Chord + GAModel.ModuleLength * 2;                                  // 考虑主视图上下各留一个组件高度的距离，俯视图上下各留一个组件高度的距离
        frameScaleY += GAModel.ModuleLength * 0.5 + GAModel.Chord * Math.Sin(GAModel.MaxAngle) + GAModel.BeamCenterToGround; // 考虑侧视图上下各留0.5个组件高度的距离
        frameScaleY *= 2;
        frameScaleY /= 297;

        FrameScale = frameScaleX > frameScaleY ? frameScaleX : frameScaleY;
        var frameHeight = FrameScale * 297; // 图框实际高度
        var frameWidth = FrameScale * 420;  // 图框实际宽度


        // 插入图框
        // 输入图框的属性
        trans.CurrentSpace.InsertBlock(insertPoint, trans.BlockTable["00-" + CompanyName + "-中文图框"], new Scale3d(FrameScale, FrameScale, FrameScale));

        // 原图框上下左右内边距分别为8,5,5,5
        double[] frameMargin = [8 * FrameScale, 5 * FrameScale, 5 * FrameScale, 5 * FrameScale];      // 绘图区域外边距
        double[] framePadding = [16 * FrameScale, 62 * FrameScale, 16 * FrameScale, 16 * FrameScale]; // 绘图区域内边距

        var leftUp = new Point3d(insertPoint.X + frameMargin[2] + framePadding[2], insertPoint.Y - frameMargin[0] - framePadding[0], 0);
        var rightDown = new Point3d(insertPoint.X + frameWidth - frameMargin[3] - framePadding[3], insertPoint.Y - frameHeight + frameMargin[1] + framePadding[1], 0);

        trans.CurrentSpace.AddEntity(new Line(leftUp, rightDown));
        // 主视图绘制
        var deltaY = DimHeight * FrameScale * 3;
        DrawFrontView(new Point3d(leftUp.X, leftUp.Y - deltaY, 0));
        // 俯视图绘制
        if (GAModel.PostList != null) deltaY += GAModel.BeamCenterToGround + MaxPileEmbedment + DimHeight * FrameScale * GAModel.PostList.Count / 2;
        deltaY += GAModel.Chord;
        DrawPlanFormView(new Point3d(leftUp.X, leftUp.Y - deltaY, 0));
        // 判断侧视图放在右方还是下方
        var drawHeight = leftUp.Y - rightDown.Y;
        var drawWidth = rightDown.X - leftUp.X;
        var width1 = drawWidth - GAModel.SystemLength; // 右边侧视图绘图区域的宽度
        var height1 = deltaY + GAModel.Chord / 2;      // 右边侧视图绘图高度
        var width2 = GAModel.SystemLength;
        var height2 = drawHeight - height1; // 下边侧视图绘图高度
        const double sidePaddingRatio = 0.8;
        var rightScale = Math.Min(sidePaddingRatio * width1 / (GAModel.Chord * 2), sidePaddingRatio * height1 / (0.5 * GAModel.Chord * Math.Sin(GAModel.MaxAngle) + GAModel.BeamCenterToGround + MaxPileEmbedment + 0.3 * DimHeight * FrameScale));
        var downScale = Math.Min(sidePaddingRatio * width2 / (GAModel.Chord * 2), sidePaddingRatio * height2 / (0.5 * GAModel.Chord * Math.Sin(GAModel.MaxAngle) + GAModel.BeamCenterToGround + MaxPileEmbedment + 0.3 * DimHeight * FrameScale));
        Point3d sideViewInsertPoint;
        double sideScale;
        if (rightScale >= downScale) {
            sideScale = rightScale;
            sideViewInsertPoint = new Point3d(rightDown.X - width1 / 2, leftUp.Y - (1 - sidePaddingRatio) / 2 * height1 - 0.5 * GAModel.Chord * Math.Sin(GAModel.MaxAngle) * sideScale, 0);
        } else {
            sideScale = downScale;
            sideViewInsertPoint = new Point3d(leftUp.X + GAModel.Chord * sideScale, leftUp.Y - height1 - (1 - sidePaddingRatio) / 2 * height2 - 0.5 * GAModel.Chord * Math.Sin(GAModel.MaxAngle) * sideScale, 0);
        }

        DrawSideView(sideViewInsertPoint, sideScale);
    }

    /// <summary>
    /// 绘制主视图
    /// </summary>
    /// <param name="frontViewInsertPoint">主梁中心线最左侧的点</param>
    public void DrawFrontView(Point3d frontViewInsertPoint) {
        #region 添加支架主要构件

        // 切换默认图层绘制构件
        if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
        // 绘制主梁
        if (GAModel.BeamList != null) {
            var beamStartX = frontViewInsertPoint.X;
            var beamStartY = frontViewInsertPoint.Y - GAModel.BeamHeight / (1 + GAModel.BeamRadio);
            foreach (var beamModel in GAModel.BeamList) {
                var beamPts = new List<Point3d> {
                                                    new(beamModel.StartX + beamStartX, beamStartY, 0),                      // 左下
                                                    new(beamModel.StartX + beamStartX, beamStartY + GAModel.BeamHeight, 0), // 左上
                                                    new(beamModel.EndX + beamStartX, beamStartY + GAModel.BeamHeight, 0),   // 右上
                                                    new(beamModel.EndX + beamStartX, beamStartY, 0)                         // 右下
                                                };

                trans.CurrentSpace.AddEntity(beamPts.CreatePolyline(p => p.Closed = true));
            }
        }

        // 绘制檩条
        if (GAModel.PurlinList != null) {
            // 切换默认图层绘制檩条
            if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
            var purlinStartPoint = new Point3d(frontViewInsertPoint.X, frontViewInsertPoint.Y, 0);
            var purlinNamePrefix = "00-" + CompanyName + "-正视图-" + GAModel.ModuleRowCounter + "P";
            var leftPurlinObj = trans.GetObject(trans.CurrentSpace.InsertBlock(purlinStartPoint, trans.BlockTable[purlinNamePrefix + "左檩条"])) as BlockReference;
            var midPurlinObj = trans.GetObject(trans.CurrentSpace.InsertBlock(purlinStartPoint, trans.BlockTable[purlinNamePrefix + "中檩条"])) as BlockReference;
            var rightPurlinObj = trans.GetObject(trans.CurrentSpace.InsertBlock(purlinStartPoint, trans.BlockTable[purlinNamePrefix + "右檩条"])) as BlockReference;
            var purlinDic = new Dictionary<string, double> {
                                                               { "檩条高度", GAModel.PurlinHeight },
                                                               { "主梁上半高度", GAModel.BeamHeight * GAModel.BeamRadio / (1 + GAModel.BeamRadio) },
                                                               { "主梁下半高度", GAModel.BeamHeight / (1 + GAModel.BeamRadio) }
                                                           };
            SetDynamicBlock(leftPurlinObj, purlinDic);
            SetDynamicBlock(midPurlinObj, purlinDic);
            SetDynamicBlock(rightPurlinObj, purlinDic);
            foreach (var purlinModel in GAModel.PurlinList) {
                var purlinVector = purlinStartPoint.GetVectorTo(new Point3d(frontViewInsertPoint.X + purlinModel.X, frontViewInsertPoint.Y, 0));
                BlockReference tempPurlinObj;
                switch (purlinModel.Type) {
                    case -1: // 左檩条
                        tempPurlinObj = leftPurlinObj.Clone() as BlockReference;
                        tempPurlinObj?.TransformBy(Matrix3d.Displacement(purlinVector));
                        if (tempPurlinObj != null) trans.CurrentSpace.AddEntity(tempPurlinObj);
                        break;
                    case 1: // 右檩条
                        tempPurlinObj = rightPurlinObj.Clone() as BlockReference;
                        tempPurlinObj?.TransformBy(Matrix3d.Displacement(purlinVector));
                        if (tempPurlinObj != null) trans.CurrentSpace.AddEntity(tempPurlinObj);
                        break;
                    case 0: // 中檩条
                        tempPurlinObj = midPurlinObj.Clone() as BlockReference;
                        tempPurlinObj?.TransformBy(Matrix3d.Displacement(purlinVector));
                        if (tempPurlinObj != null) trans.CurrentSpace.AddEntity(tempPurlinObj);
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
            if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
            var postDic = new Dictionary<string, double>();
            foreach (var postModel in GAModel.PostList) {
                postDic["主梁中心与基础顶面距离"] = GAModel.BeamCenterToGround - postModel.PileUpGround;
                postDic["法兰板厚度"] = postModel.FlangeThickness;
                postDic["基础露头"] = postModel.PileUpGround;
                postDic["基础埋深"] = postModel.PileDownGround;
                string postBlockName;
                if (postModel.PileUpGround > 0) { // 立柱露头大于0默认为混凝土桩
                    postBlockName = postModel.IsDrive ? "00-" + CompanyName + "-正视图-" + TrackerType + "-驱动立柱-PHC" : "00-" + CompanyName + "-正视图-" + TrackerType + "-普通立柱-PHC";
                } else { // 立柱露头为0表示锤入桩
                    postBlockName = postModel.IsDrive ? "00-" + CompanyName + "-正视图-" + TrackerType + "-驱动立柱" : "00-" + CompanyName + "-正视图-" + TrackerType + "-普通立柱";
                }

                var postId = trans.CurrentSpace.InsertBlock(new Point3d(frontViewInsertPoint.X + postModel.X, frontViewInsertPoint.Y, 0), trans.BlockTable[postBlockName]); // 插入立柱块
                var postObj = trans.GetObject(postId) as BlockReference;
                SetDynamicBlock(postObj, postDic); // 设置立柱动态块属性
            }
        }

        // 绘制组件
        if (GAModel.PurlinList != null) {
            // 切换默认图层绘制主梁
            if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
            var moduleIndex = 0;
            var purlinStartX = frontViewInsertPoint.X;
            var purlinStartY = frontViewInsertPoint.Y + GAModel.BeamHeight * GAModel.BeamRadio / (1 + GAModel.BeamRadio) + GAModel.PurlinHeight;
            while (moduleIndex < GAModel.PurlinList.Count) {
                while (GAModel.PurlinList[moduleIndex].Type != 1) {
                    var startX = GAModel.PurlinList[moduleIndex].X + GAModel.ModuleGapAxis / 2;
                    var endX = GAModel.PurlinList[moduleIndex + 1].X - GAModel.ModuleGapAxis / 2;
                    var purlinPts = new List<Point3d> {
                                                          new(purlinStartX + startX, purlinStartY, 0),
                                                          new(purlinStartX + startX, purlinStartY + GAModel.ModuleHeight, 0),
                                                          new(purlinStartX + endX, purlinStartY + GAModel.ModuleHeight, 0),
                                                          new(purlinStartX + endX, purlinStartY, 0)
                                                      };
                    trans.CurrentSpace.AddEntity(purlinPts.CreatePolyline(p => p.Closed = true));
                    moduleIndex++;
                }

                moduleIndex++;
            }
        }

        // 地平线示意图
        // 地表面示意线
        if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
        trans.CurrentSpace.AddEntity(new Line(new Point3d(frontViewInsertPoint.X - GAModel.LeftRemind, frontViewInsertPoint.Y - GAModel.BeamCenterToGround, 0), new Point3d(frontViewInsertPoint.X + GAModel.SystemLength - GAModel.LeftRemind, frontViewInsertPoint.Y - GAModel.BeamCenterToGround, 0)));

        #endregion

        #region 标注

        if (trans.LayerTable.Has("01-Dimension")) trans.Database.Clayer = trans.LayerTable["01-Dimension"]; // 切换标注图层
        // 添加主视图中的标注
        // 主梁标注
        if (GAModel.BeamList != null) {
            var beamStartX = frontViewInsertPoint.X;
            var beamStartY = frontViewInsertPoint.Y - GAModel.BeamHeight / (1 + GAModel.BeamRadio);
            // 主梁分段标注
            foreach (var beamModel in GAModel.BeamList) {
                trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                      XLine1Point = new Point3d(beamStartX + beamModel.StartX, beamStartY, 0),
                                                                      XLine2Point = new Point3d(beamStartX + beamModel.EndX, beamStartY, 0),
                                                                      DimLinePoint = new Point3d(beamStartX + (beamModel.StartX + beamModel.EndX) / 2, beamStartY + DimHeight * FrameScale, 0),
                                                                      Dimscale = FrameScale,
                                                                  });
            }

            // 主梁总长标记
            trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                  XLine1Point = new Point3d(GAModel.BeamList.First().StartX + beamStartX, beamStartY, 0),
                                                                  XLine2Point = new Point3d(GAModel.BeamList.Last().EndX + beamStartX, beamStartY, 0),
                                                                  DimLinePoint = new Point3d((GAModel.BeamList.First().StartX + GAModel.BeamList.Last().EndX) / 2 + beamStartX, beamStartY + DimHeight * FrameScale * 2, 0),
                                                                  Dimscale = FrameScale
                                                              });
        }

        // 立柱标注
        if (GAModel.PostList != null) {
            var dimStartX = frontViewInsertPoint.X;
            var dimStartY = frontViewInsertPoint.Y - GAModel.BeamCenterToGround - MaxPileEmbedment;
            // 第一层跨距，每跨标注
            foreach (var postModel in GAModel.PostList) {
                if (postModel.Num == 1) {
                    trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                          XLine1Point = new Point3d(dimStartX, frontViewInsertPoint.Y, 0),
                                                                          XLine2Point = new Point3d(dimStartX + postModel.LeftSpan, dimStartY, 0),
                                                                          DimLinePoint = new Point3d(dimStartX + postModel.LeftSpan / 2, dimStartY, 0),
                                                                          Dimscale = FrameScale
                                                                      });
                } else {
                    trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                          XLine1Point = new Point3d(dimStartX, dimStartY, 0),
                                                                          XLine2Point = new Point3d(dimStartX + postModel.LeftSpan, dimStartY, 0),
                                                                          DimLinePoint = new Point3d(dimStartX + postModel.LeftSpan / 2, dimStartY, 0),
                                                                          Dimscale = FrameScale
                                                                      });
                }

                dimStartX += postModel.LeftSpan;

                if (postModel.Num == GAModel.PostList.Count) {
                    trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                          XLine1Point = new Point3d(dimStartX, dimStartY, 0),
                                                                          XLine2Point = new Point3d(dimStartX + postModel.RightSpan, frontViewInsertPoint.Y, 0),
                                                                          DimLinePoint = new Point3d(dimStartX + postModel.RightSpan / 2, dimStartY, 0),
                                                                          Dimscale = FrameScale
                                                                      });
                }
            }

            // 第二册跨距标注，中间向两边发散
            var midPostIndex = GAModel.PostList.Count / 2;
            var midPost = GAModel.PostList[midPostIndex];
            if (midPostIndex > 0) {
                dimStartX = frontViewInsertPoint.X;
                // 从中间立柱向左侧标注
                for (var i = 0; i < midPostIndex - 1; i++) {
                    trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                          XLine1Point = new Point3d(dimStartX + GAModel.PostList[i].X, dimStartY, 0),
                                                                          XLine2Point = new Point3d(dimStartX + midPost.X, dimStartY, 0),
                                                                          DimLinePoint = new Point3d(dimStartX + (GAModel.PostList[i].X + midPost.X) / 2, dimStartY - DimHeight * FrameScale * (midPostIndex - i), 0),
                                                                          Dimscale = FrameScale
                                                                      });
                }

                var dimEndX = frontViewInsertPoint.X + GAModel.PostList.Last().X + GAModel.PostList.Last().RightSpan;
                // 从中间立柱向右侧标注
                for (var i = midPostIndex + 2; i < GAModel.PostList.Count; i++) {
                    trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                          XLine1Point = new Point3d(dimStartX + midPost.X, dimStartY, 0),
                                                                          XLine2Point = new Point3d(dimStartX + GAModel.PostList[i].X, dimStartY, 0),
                                                                          DimLinePoint = new Point3d(dimStartX + (GAModel.PostList[i].X + midPost.X) / 2, dimStartY - DimHeight * FrameScale * (i - midPostIndex), 0),
                                                                          Dimscale = FrameScale
                                                                      });
                }
            }
        }

        #endregion

        #region 文字

        // 切换文字图层添加主梁规格
        if (trans.LayerTable.Has("03-Text")) trans.Database.Clayer = trans.LayerTable["03-Text"];
        // 添加主梁规格
        if (GAModel.BeamList != null) {
            var beamStartX = frontViewInsertPoint.X;
            var beamStartY = frontViewInsertPoint.Y - GAModel.BeamHeight / (1 + GAModel.BeamRadio);
            foreach (var beamModel in GAModel.BeamList) {
                trans.CurrentSpace.AddEntity(new DBText {
                                                            Height = TextSize * FrameScale * 0.5,
                                                            TextString = beamModel.Section,
                                                            TextStyleId = trans.TextStyleTable[CompanyName + "Font"],
                                                            Justify = AttachmentPoint.BaseCenter,
                                                            AlignmentPoint = new Point3d(beamStartX + (beamModel.StartX + beamModel.EndX) / 2, beamStartY + GAModel.BeamHeight * GAModel.BeamRadio / (1 + GAModel.BeamRadio) + GAModel.PurlinHeight + GAModel.ModuleHeight + TextSize * FrameScale * 0.1, 0)
                                                        });
            }
        }

        // 添加立柱规格
        if (GAModel.PostList != null) {
            foreach (var postModel in GAModel.PostList) {
                trans.CurrentSpace.AddEntity(new DBText {
                                                            Justify = AttachmentPoint.TopCenter,
                                                            AlignmentPoint = new Point3d(frontViewInsertPoint.X + postModel.X, frontViewInsertPoint.Y - GAModel.BeamCenterToGround - MaxPileEmbedment - TextSize * FrameScale * 0.1, 0),
                                                            Height = TextSize * FrameScale * 0.5,
                                                            TextString = postModel.Section,
                                                            TextStyleId = trans.TextStyleTable[CompanyName + "Font"]
                                                        });
                trans.CurrentSpace.AddEntity(new DBText {
                                                            Justify = AttachmentPoint.TopCenter,
                                                            AlignmentPoint = new Point3d(frontViewInsertPoint.X + postModel.X, frontViewInsertPoint.Y - GAModel.BeamCenterToGround - MaxPileEmbedment - TextSize * FrameScale * 0.5 - TextSize * FrameScale * 0.1, 0),
                                                            Height = TextSize * FrameScale * 0.5,
                                                            TextString = postModel.IsDrive ? "DrivePost" : "GeneralPost",
                                                            TextStyleId = trans.TextStyleTable[CompanyName + "Font"]
                                                        });
            }
        }

        #endregion
    }

    public void DrawPlanFormView(Point3d plantFormViewInsertPoint) {
        #region 绘图

        // 切换默认图层绘制俯视图
        if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
        // 组件绘图
        var moduleDic = new Dictionary<string, double> {
                                                           { "组件宽", GAModel.ModuleWidth },
                                                           { "组件总高度", GAModel.Chord },
                                                           { "组件间隙-垂直于主梁", GAModel.ModuleGapChord }
                                                       };
        var moduleBlockName = "00-" + CompanyName + "-俯视图-" + GAModel.ModuleRowCounter + "P组件";
        if (GAModel.PurlinList != null) {
            var moduleIndex = 0;
            var purlinStartX = plantFormViewInsertPoint.X;
            var purlinStartY = plantFormViewInsertPoint.Y;
            while (moduleIndex < GAModel.PurlinList.Count) {
                while (GAModel.PurlinList[moduleIndex].Type != 1) {
                    var point = new Point3d(purlinStartX + (GAModel.PurlinList[moduleIndex].X + GAModel.PurlinList[moduleIndex + 1].X) / 2, purlinStartY, 0);
                    var moduleObj = trans.GetObject(trans.CurrentSpace.InsertBlock(point, trans.BlockTable[moduleBlockName])) as BlockReference;
                    SetDynamicBlock(moduleObj, moduleDic);
                    moduleIndex++;
                }

                moduleIndex++;
            }
        }

        #endregion

        #region 标注

        if (trans.LayerTable.Has("01-Dimension")) trans.Database.Clayer = trans.LayerTable["01-Dimension"]; // 切换标注图层

        // 组件宽标注
        trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                              XLine1Point = new Point3d(plantFormViewInsertPoint.X, plantFormViewInsertPoint.Y + GAModel.Chord / 2, 0),
                                                              XLine2Point = new Point3d(plantFormViewInsertPoint.X + GAModel.ModuleWidth, plantFormViewInsertPoint.Y + GAModel.Chord / 2, 0),
                                                              DimLinePoint = new Point3d(plantFormViewInsertPoint.X + GAModel.ModuleWidth / 2, plantFormViewInsertPoint.Y + GAModel.Chord / 2 + DimHeight * FrameScale, 0),
                                                              Dimscale = FrameScale
                                                          });
        // 组件间隙(垂直于弦长)
        trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                              XLine1Point = new Point3d(plantFormViewInsertPoint.X + GAModel.ModuleWidth, plantFormViewInsertPoint.Y + GAModel.Chord / 2, 0),
                                                              XLine2Point = new Point3d(plantFormViewInsertPoint.X + GAModel.ModuleWidth + GAModel.ModuleGapAxis, plantFormViewInsertPoint.Y + GAModel.Chord / 2, 0),
                                                              DimLinePoint = new Point3d(plantFormViewInsertPoint.X + GAModel.ModuleWidth + GAModel.ModuleGapAxis / 2, plantFormViewInsertPoint.Y + GAModel.Chord / 2 + DimHeight * FrameScale, 0),
                                                              Dimscale = FrameScale
                                                          });
        // 驱动间隙标注
        if (GAModel is { DriveGap: > 0, PostList: not null }) {
            var drivePostList = GAModel.PostList.Where(post => post.IsDrive);
            foreach (var drivePost in drivePostList) {
                trans.CurrentSpace.AddEntity(new RotatedDimension {
                                                                      XLine1Point = new Point3d(plantFormViewInsertPoint.X + drivePost.X - GAModel.DriveGap / 2, plantFormViewInsertPoint.Y + GAModel.Chord / 2, 0),
                                                                      XLine2Point = new Point3d(plantFormViewInsertPoint.X + drivePost.X + GAModel.DriveGap / 2, plantFormViewInsertPoint.Y + GAModel.Chord / 2, 0),
                                                                      DimLinePoint = new Point3d(plantFormViewInsertPoint.X + drivePost.X, plantFormViewInsertPoint.Y + GAModel.Chord / 2 + DimHeight * FrameScale, 0),
                                                                      Dimscale = FrameScale
                                                                  });
            }
        }

        // 组件高标注
        var moduleStartX = plantFormViewInsertPoint.X;
        var moduleStartY = plantFormViewInsertPoint.Y + (GAModel.ModuleRowCounter > 1 ? GAModel.ModuleGapChord / 2 : -GAModel.Chord / 2);
        trans.CurrentSpace.AddEntity(new AlignedDimension {
                                                              XLine1Point = new Point3d(moduleStartX, moduleStartY, 0),
                                                              XLine2Point = new Point3d(moduleStartX, moduleStartY + GAModel.ModuleLength, 0),
                                                              DimLinePoint = new Point3d(moduleStartX - DimHeight * FrameScale, moduleStartY + GAModel.ModuleLength / 2, 0),
                                                              Dimscale = FrameScale
                                                          });
        if (GAModel.ModuleRowCounter > 1) {
            trans.CurrentSpace.AddEntity(new AlignedDimension {
                                                                  XLine1Point = new Point3d(moduleStartX, moduleStartY, 0),
                                                                  XLine2Point = new Point3d(moduleStartX, moduleStartY - GAModel.ModuleGapChord, 0),
                                                                  DimLinePoint = new Point3d(moduleStartX - DimHeight * FrameScale, moduleStartY - GAModel.ModuleGapChord / 2, 0),
                                                                  Dimscale = FrameScale
                                                              });
        }

        #endregion
    }

    public void DrawSideView(Point3d sideViewInsertPoint, double sideScale) {
        // 切换默认图层插入侧视图动态块
        if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
        var sideObjIds = new List<Entity>(); // 旋转部的所有对象
        var rotationDic = new Dictionary<string, double> {
                                                             { "弦长", GAModel.Chord },
                                                             { "组件间隙-垂直于主梁", GAModel.ModuleGapChord },
                                                             { "檩条长度", GAModel.PurlinLength },
                                                             { "组件厚度", GAModel.ModuleHeight },
                                                             { "檩条高度", GAModel.PurlinHeight },
                                                             { "主梁上半高", GAModel.BeamHeight * GAModel.BeamRadio / (1 + GAModel.BeamRadio) },
                                                             { "主梁下半高", GAModel.BeamHeight / (1 + GAModel.BeamRadio) },
                                                             { "旋转角度", 0 }
                                                         };
        var rotationBlockName = "00-" + CompanyName + "-侧视图-" + Convert.ToString(GAModel.ModuleRowCounter) + "P旋转部";
        // 水平放置旋转部
        var rotationObj1 = trans.GetObject(trans.CurrentSpace.InsertBlock(sideViewInsertPoint, trans.BlockTable[rotationBlockName])) as BlockReference;
        SetDynamicBlock(rotationObj1, rotationDic);
        if (trans.LayerTable.Has("02-Rotation")) trans.Database.Clayer = trans.LayerTable["02-Rotation"];
        // 最大角度放置旋转部
        rotationDic["旋转角度"] = GAModel.MaxAngle * Math.PI / 180;
        var rotationObj2 = trans.GetObject(trans.CurrentSpace.InsertBlock(sideViewInsertPoint, trans.BlockTable[rotationBlockName])) as BlockReference;
        SetDynamicBlock(rotationObj2, rotationDic);
        // 最大角度放置旋转部反向
        rotationDic["旋转角度"] = -GAModel.MaxAngle * Math.PI / 180;
        var rotationObj3 = trans.GetObject(trans.CurrentSpace.InsertBlock(sideViewInsertPoint, trans.BlockTable[rotationBlockName])) as BlockReference;
        SetDynamicBlock(rotationObj3, rotationDic);

        // 侧视图中立柱部分
        if (trans.LayerTable.Has("0")) trans.Database.Clayer = trans.LayerTable["0"];
        var postDic = new Dictionary<string, double> {
                                                         { "主梁中心与基础顶面距离", GAModel.BeamCenterToGround - GAModel.PileUpGround },
                                                         { "基础露头", GAModel.PileUpGround },
                                                         { "基础埋深", GAModel.PileDownGround },
                                                         { "法兰板厚度", GAModel.PostList == null ? 0 : GAModel.PostList.First().FlangeThickness }
                                                     };
        var sidePostBlockName = "00-" + CompanyName + "-侧视图-" + TrackerType + (GAModel.PileUpGround > 0 ? "-PHC" : "");
        var sidePostObj = trans.GetObject(trans.CurrentSpace.InsertBlock(sideViewInsertPoint, trans.BlockTable[sidePostBlockName])) as BlockReference;
        SetDynamicBlock(sidePostObj, postDic);

        // 侧视图中的土壤示意图
        var sideSoilTop = sideViewInsertPoint.Y - GAModel.BeamCenterToGround;
        var sideSoilBottom = sideSoilTop - MaxPileEmbedment - DimHeight * FrameScale * 0.3;
        var sideSoilPts = new List<Point3d> {
                                                new(sideViewInsertPoint.X - GAModel.Chord, sideSoilTop, 0),
                                                new(sideViewInsertPoint.X + GAModel.Chord, sideSoilTop, 0),
                                                new(sideViewInsertPoint.X + GAModel.Chord, sideSoilBottom, 0),
                                                new(sideViewInsertPoint.X - GAModel.Chord, sideSoilBottom, 0)
                                            };
        var soilObj = trans.CurrentSpace.AddEntity(sideSoilPts.CreatePolyline(p => p.Closed = true)).GetObject<Polyline>();

        var soilHatchObj = new Hatch();
        trans.CurrentSpace.AddEntity(soilHatchObj);
        soilHatchObj.PatternScale = 10;
        soilHatchObj.SetHatchPattern(HatchPatternType.PreDefined, "AR-CONC");
        soilHatchObj.Associative = true;
        soilHatchObj.AppendLoop(HatchLoopTypes.Outermost, new() { soilObj!.ObjectId });
        soilHatchObj.EvaluateHatch(true);

        // 侧视图中的标注
        if (trans.LayerTable.Has("01-Dimension")) trans.Database.Clayer = trans.LayerTable["01-Dimension"];
        // 旋转中心离地
        trans.CurrentSpace.AddEntity(new AlignedDimension {
                                                              XLine1Point = new Point3d(sideViewInsertPoint.X, sideViewInsertPoint.Y - GAModel.BeamCenterToGround, 0),
                                                              XLine2Point = new Point3d(sideViewInsertPoint.X, sideViewInsertPoint.Y, 0),
                                                              DimLinePoint = new Point3d(sideViewInsertPoint.X - GAModel.BeamCenterToGround * 0.7, sideViewInsertPoint.Y - GAModel.BeamCenterToGround / 2, 0),
                                                              Dimscale = FrameScale / sideScale
                                                          });
        // 最小离地高度
        var moduleLowestPointX = sideViewInsertPoint.X - GAModel.Chord / 2 * Math.Cos(GAModel.MaxAngle * Math.PI / 180) - (GAModel.BeamHeight * GAModel.BeamRadio / (1 + GAModel.BeamRadio) + GAModel.PurlinHeight) * Math.Sin(GAModel.MaxAngle * Math.PI / 180);
        trans.CurrentSpace.AddEntity(new AlignedDimension {
                                                              XLine1Point = new Point3d(moduleLowestPointX, sideViewInsertPoint.Y - GAModel.BeamCenterToGround, 0),
                                                              XLine2Point = new Point3d(moduleLowestPointX, sideViewInsertPoint.Y - GAModel.BeamCenterToGround + GAModel.MinGroundDist, 0),
                                                              DimLinePoint = new Point3d(moduleLowestPointX - GAModel.MinGroundDist * 0.7, sideViewInsertPoint.Y - GAModel.BeamCenterToGround + GAModel.MinGroundDist / 2, 0),
                                                              Dimscale = FrameScale / sideScale
                                                          });

        // +最大角度标注
        var corner = new Point3d(moduleLowestPointX - GAModel.MinGroundDist / Math.Tan(GAModel.MaxAngle * Math.PI / 180), sideViewInsertPoint.Y - GAModel.BeamCenterToGround, 0); // 角点 
        var dimRadius = (GAModel.Chord + GAModel.MinGroundDist / Math.Sin(GAModel.MaxAngle * Math.PI / 180)) * 0.95;

        trans.CurrentSpace.AddEntity(new LineAngularDimension2 {
                                                                   XLine1Start = corner,
                                                                   XLine2Start = corner,
                                                                   XLine1End = new Point3d(corner.X + dimRadius, corner.Y, 0),
                                                                   XLine2End = new Point3d(corner.X + dimRadius * Math.Cos(GAModel.MaxAngle * Math.PI / 180), corner.Y + dimRadius * Math.Sin(GAModel.MaxAngle * Math.PI / 180), 0),
                                                                   ArcPoint = new Point3d(corner.X + dimRadius * Math.Cos(GAModel.MaxAngle * Math.PI / 180 / 2), corner.Y + dimRadius * Math.Sin(GAModel.MaxAngle * Math.PI / 180 / 2), 0),
                                                                   Dimscale = FrameScale / sideScale
                                                               });
    }

    // 修改动态块的属性
    public static void SetDynamicBlock(BlockReference? brf, Dictionary<string, double> dicData) {
        var props = brf?.DynamicBlockReferencePropertyCollection;
        if (props == null) return;
        foreach (DynamicBlockReferenceProperty prop in props) {
            if (dicData.TryGetValue(prop.PropertyName, out var value)) {
                prop.Value = value;
            }
        }
    }
}