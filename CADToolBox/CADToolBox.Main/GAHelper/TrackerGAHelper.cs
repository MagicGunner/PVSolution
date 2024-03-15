using CADToolBox.Shared.Models.CADModels.Implement;

namespace CADToolBox.Main.GAHelper;

// 此处增加绘图辅助函数
public class TrackerGAHelper(
    TrackerModel trackerModel,
    DBTrans      trans,
    Point3d      insertPoint
) {
    private TrackerModel GAModel { get; } = trackerModel;

    private DBTrans CurrentTrans { get; } = trans;

    public void InitStyles() {
        // 初始化字体
        if (!CurrentTrans.TextStyleTable.Has("Linsum")) {
            try {
                CurrentTrans.TextStyleTable.Add("Linsum", "gbeitc.shx", 0.8);
            } catch {
                MessageBox.Show("缺少GB字体，使用标准字体代替");
                CurrentTrans.TextStyleTable.Add("Linsum", FontTTF.仿宋GB2312, 0.8);
            }
        }
    }
}