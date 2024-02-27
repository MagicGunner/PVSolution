using System.Windows.Documents;

namespace CADToolBox.Main.Functions;

public static class CadFunctions {
    public static void DrawHSteel(DBTrans trans, Point3d insertPoint, double h, double b, double tw, double tf) {
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;
        // 逆时针画
        var startPoint = new Point3d(x0,     y0, 0);
        var endPoint   = new Point3d(x0 + b, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X, startPoint.Y + tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X - (b - tw) / 2, startPoint.Y, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X, startPoint.Y + h - 2 * tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X + (b - tw) / 2, startPoint.Y, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X, startPoint.Y + tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X - b, startPoint.Y, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X, startPoint.Y - tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X + (b - tw) / 2, startPoint.Y, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X, startPoint.Y - (h - 2 * tf), 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X - (b - tw) / 2, startPoint.Y, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(x0, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
    }

    public static void DrawCSteel(DBTrans trans,
                                  Point3d insertPoint,
                                  double  h,
                                  double  b,
                                  double  l,
                                  double  t,
                                  double  rin) {
        if (l == 0) { // 折弯槽钢的情况
            DrawCSteel(trans, insertPoint, h, b, t, rin);
            return;
        }

        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;
        // 逆时针画
        // 先画平直部分
        var rout       = rin == 0 ? 0 : rin + t;
        var startPoint = new Point3d(x0     + rout, y0, 0);
        var endPoint   = new Point3d(x0 + b - rout, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + rout, 0);
        endPoint   = new Point3d(x0 + b, y0 + l,    0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0     + b, y0 + l, 0);
        endPoint   = new Point3d(x0 + b - t, y0 + l, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t, y0 + l,       0);
        endPoint   = new Point3d(x0 + b - t, y0 + t + rin, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t - rin, y0 + t, 0);
        endPoint   = new Point3d(x0     + t + rin, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t, y0     + t + rin, 0);
        endPoint   = new Point3d(x0 + t, y0 + h - t - rin, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t + rin, y0 + h - t,       0);
        endPoint   = new Point3d(x0               + b - t - rin, y0 + h - t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t, y0 + h - t - rin, 0);
        endPoint   = new Point3d(x0 + b - t, y0 + h - l,       0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t, y0 + h - l, 0);
        endPoint   = new Point3d(x0     + b, y0 + h - l, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + h - l,    0);
        endPoint   = new Point3d(x0 + b, y0 + h - rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - rout, y0 + h, 0);
        endPoint   = new Point3d(x0     + rout, y0 + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + h - rout, 0);
        endPoint   = new Point3d(x0, y0     + rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        // 画R角
        if (rin == 0) return;
        // 左下圆弧
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0, y0 + rout, 0),
                                                     new Point3d(x0     + rout, y0 + rout, 0),
                                                     Math.PI / 2),
                                     ArcEx.CreateArc(new Point3d(x0 + t,    y0 + rout, 0),
                                                     new Point3d(x0 + rout, y0 + rout, 0),
                                                     Math.PI / 2));
        // 右下圆弧
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + b - rout, y0,        0),
                                                     new Point3d(x0 + b - rout, y0 + rout, 0),
                                                     Math.PI / 2),
                                     ArcEx.CreateArc(new Point3d(x0 + b - rout, y0 + t,    0),
                                                     new Point3d(x0 + b - rout, y0 + rout, 0),
                                                     Math.PI / 2));
        // 右上圆弧
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + b, y0 + h - rout, 0),
                                                     new Point3d(x0         + b - rout, y0 + h - rout, 0),
                                                     Math.PI / 2),
                                     ArcEx.CreateArc(new Point3d(x0 + b - t,    y0 + h - rout, 0),
                                                     new Point3d(x0 + b - rout, y0 + h - rout, 0),
                                                     Math.PI / 2));
        // 左上圆弧
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + rout, y0     + h,    0),
                                                     new Point3d(x0 + rout, y0 + h - rout, 0),
                                                     Math.PI / 2),
                                     ArcEx.CreateArc(new Point3d(x0 + rout, y0 + h - t,    0),
                                                     new Point3d(x0 + rout, y0 + h - rout, 0),
                                                     Math.PI / 2));
    }

    public static void DrawCSteel(DBTrans trans,
                                  Point3d insertPoint,
                                  double  h,
                                  double  b,
                                  double  t,
                                  double  rin) {
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;
        // 逆时针画
        // 先画平直部分
        var rout       = rin == 0 ? 0 : rin + t;
        var startPoint = new Point3d(x0 + rout, y0, 0);
        var endPoint   = new Point3d(x0 + b,    y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0,     0);
        endPoint   = new Point3d(x0 + b, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + t,   0);
        endPoint   = new Point3d(x0 + t     + rin, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t, y0     + t + rin, 0);
        endPoint   = new Point3d(x0 + t, y0 + h - t - rin, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t + rin, y0 + h - t, 0);
        endPoint   = new Point3d(x0 + b,       y0 + h - t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + h - t, 0);
        endPoint   = new Point3d(x0 + b, y0     + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b,    y0 + h, 0);
        endPoint   = new Point3d(x0 + rout, y0 + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + h - rout, 0);
        endPoint   = new Point3d(x0, y0     + rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        // 画R角
        if (rin == 0) return;
        // 左下圆弧
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0, y0 + rout, 0),
                                                     new Point3d(x0     + rout, y0 + rout, 0),
                                                     Math.PI / 2),
                                     ArcEx.CreateArc(new Point3d(x0 + t,    y0 + rout, 0),
                                                     new Point3d(x0 + rout, y0 + rout, 0),
                                                     Math.PI / 2));

        // 左上圆弧
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + rout, y0     + h,    0),
                                                     new Point3d(x0 + rout, y0 + h - rout, 0),
                                                     Math.PI / 2),
                                     ArcEx.CreateArc(new Point3d(x0 + rout, y0 + h - t,    0),
                                                     new Point3d(x0 + rout, y0 + h - rout, 0),
                                                     Math.PI / 2));
    }
}