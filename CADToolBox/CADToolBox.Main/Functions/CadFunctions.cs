using System.Globalization;
using CADToolBox.Resource.NameDictionarys;
using CADToolBox.Shared.Models.CADModels.Implement;
using System.Net;
using System.Windows.Documents;

namespace CADToolBox.Main.Functions;

public static class CadFunctions {
#region 截面绘制函数

    public static void DrawHSteel(DBTrans trans,
                                  Point3d insertPoint,
                                  double  h,
                                  double  b,
                                  double  tw,
                                  double  tf,
                                  double  r) {
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;
        // 直线部分
        var startPoint = new Point3d(x0, y0, 0);
        var endPoint   = new Point3d(x0 + b, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = endPoint;
        endPoint   = new Point3d(startPoint.X, startPoint.Y + tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0, 0);
        endPoint   = new Point3d(x0 + b, y0 + tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0        + tf, 0);
        endPoint   = new Point3d(x0 + (b + tw) / 2 + r, y0 + tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + (b + tw) / 2, y0     + tf + r, 0);
        endPoint   = new Point3d(x0 + (b + tw) / 2, y0 + h - tf - r, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + (b + tw) / 2 + r, y0 + h - tf, 0);
        endPoint   = new Point3d(x0 + b, y0                + h - tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + h - tf, 0);
        endPoint   = new Point3d(x0 + b, y0     + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0     + b, y0 + h, 0);
        endPoint   = new Point3d(x0, y0 + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0     + h, 0);
        endPoint   = new Point3d(x0, y0 + h - tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + h            - tf, 0);
        endPoint   = new Point3d(x0     + (b - tw) / 2 - r, y0 + h - tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + (b - tw) / 2, y0 + h - tf - r, 0);
        endPoint   = new Point3d(x0 + (b - tw) / 2, y0     + tf + r, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + (b - tw) / 2 - r, y0 + tf, 0);
        endPoint   = new Point3d(x0, y0            + tf, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + tf, 0);
        endPoint   = new Point3d(x0, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));

        if (r == 0)
            return;
        // 倒角
        // 左下
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + (b - tw) / 2 - r, y0 + tf, 0),
                                                     new Point3d(x0 + (b - tw) / 2 - r, y0 + tf + r, 0), Math.PI / 2));
        // 右下
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + (b + tw) / 2, y0 + tf + r, 0),
                                                     new Point3d(x0 + (b + tw) / 2 + r, y0 + tf + r, 0), Math.PI / 2));
        // 右上
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + (b + tw) / 2 + r, y0 + h - tf, 0),
                                                     new Point3d(x0 + (b + tw) / 2 + r, y0 + h - tf - r, 0),
                                                     Math.PI / 2));
        // 左上
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + (b - tw) / 2, y0 + h - tf - r, 0),
                                                     new Point3d(x0 + (b - tw) / 2 - r, y0 + h - tf - r, 0),
                                                     Math.PI / 2));
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
        endPoint   = new Point3d(x0 + b, y0 + l, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0     + b, y0 + l, 0);
        endPoint   = new Point3d(x0 + b - t, y0 + l, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t, y0 + l, 0);
        endPoint   = new Point3d(x0 + b - t, y0 + t + rin, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t - rin, y0 + t, 0);
        endPoint   = new Point3d(x0     + t + rin, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t, y0     + t + rin, 0);
        endPoint   = new Point3d(x0 + t, y0 + h - t - rin, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t + rin, y0 + h - t, 0);
        endPoint   = new Point3d(x0               + b - t - rin, y0 + h - t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t, y0 + h - t - rin, 0);
        endPoint   = new Point3d(x0 + b - t, y0 + h - l, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - t, y0 + h - l, 0);
        endPoint   = new Point3d(x0     + b, y0 + h - l, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + h - l, 0);
        endPoint   = new Point3d(x0 + b, y0 + h - rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - rout, y0 + h, 0);
        endPoint   = new Point3d(x0     + rout, y0 + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + h - rout, 0);
        endPoint   = new Point3d(x0, y0     + rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        // 画R角
        if (rin == 0)
            return;
        // 左下圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0, y0 + rout, 0), new Point3d(x0 + rout, y0 + rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + t, y0 + rout, 0), new Point3d(x0 + rout, y0 + rout, 0),
                                        Math.PI / 2));
        // 右下圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0 + b - rout, y0, 0), new Point3d(x0 + b - rout, y0 + rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + b - rout, y0 + t, 0), new Point3d(x0 + b - rout, y0 + rout, 0),
                                        Math.PI / 2));
        // 右上圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0 + b, y0 + h - rout, 0), new Point3d(x0 + b - rout, y0 + h - rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + b - t, y0    + h - rout, 0),
                                        new Point3d(x0 + b - rout, y0 + h - rout, 0), Math.PI / 2));
        // 左上圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0 + rout, y0 + h, 0), new Point3d(x0 + rout, y0 + h - rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + rout, y0 + h - t, 0), new Point3d(x0 + rout, y0 + h - rout, 0),
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
        var endPoint   = new Point3d(x0 + b, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0, 0);
        endPoint   = new Point3d(x0 + b, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + t, 0);
        endPoint   = new Point3d(x0 + t     + rin, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t, y0     + t + rin, 0);
        endPoint   = new Point3d(x0 + t, y0 + h - t - rin, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t + rin, y0 + h - t, 0);
        endPoint   = new Point3d(x0 + b, y0       + h - t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + h - t, 0);
        endPoint   = new Point3d(x0 + b, y0     + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0    + h, 0);
        endPoint   = new Point3d(x0 + rout, y0 + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + h - rout, 0);
        endPoint   = new Point3d(x0, y0     + rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        // 画R角
        if (rin == 0)
            return;
        // 左下圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0, y0 + rout, 0), new Point3d(x0 + rout, y0 + rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + t, y0 + rout, 0), new Point3d(x0 + rout, y0 + rout, 0),
                                        Math.PI / 2));

        // 左上圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0 + rout, y0 + h, 0), new Point3d(x0 + rout, y0 + h - rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + rout, y0 + h - t, 0), new Point3d(x0 + rout, y0 + h - rout, 0),
                                        Math.PI / 2));
    }

    public static void DrawRollCSteel(DBTrans trans,
                                      Point3d insertPoint,
                                      double  h,
                                      double  b,
                                      double  d,
                                      double  t,
                                      double  r,
                                      double  r1) {
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;

        var slopeDeg = Math.Atan(1.0 / 10);

        // 计算斜线段长度
        var lSlopeX = b - d - (r1 - r1 * Math.Sin(slopeDeg)) - (r - r * Math.Sin(slopeDeg));
        var lSlopeY = lSlopeX                                      * Math.Tan(slopeDeg);
        var ratio1  = ((b - d) / 2 - (r - r * Math.Sin(slopeDeg))) / lSlopeX; // 左侧的比例
        var ratio2  = 1 - ratio1;                                             // 右侧的比例


        var t1 = t - lSlopeY * ratio2 - r1 * Math.Cos(slopeDeg);                // 翼缘边缘的直线段长度
        var h1 = h - 2       * (t + lSlopeY * ratio1 + r * Math.Cos(slopeDeg)); // 腹板内侧直线段长度


        // 先画直线段部分，逆时针
        var startPoint = new Point3d(x0, y0, 0);
        var endPoint   = new Point3d(x0 + b, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + t1, 0);
        endPoint   = new Point3d(x0 + b, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + (b + d) / 2 + lSlopeX * ratio2, y0 + t - lSlopeY * ratio2, 0);
        endPoint   = new Point3d(x0 + (b + d) / 2 - lSlopeX * ratio1, y0 + t + lSlopeY * ratio1, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + d, y0 + (h - h1) / 2, 0);
        endPoint   = new Point3d(x0 + d, y0 + (h + h1) / 2, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + (b + d) / 2 - lSlopeX * ratio1, y0 + h - t - lSlopeY * ratio1, 0);
        endPoint   = new Point3d(x0               + (b + d) / 2 + lSlopeX * ratio2, y0 + h - t + lSlopeY * ratio2, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0 + h - t1, 0);
        endPoint   = new Point3d(x0 + b, y0     + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0     + b, y0 + h, 0);
        endPoint   = new Point3d(x0, y0 + h, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + h, 0);
        endPoint   = new Point3d(x0, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));

        // 画倒角
        // 左下
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + d, y0 + (h         - h1) / 2, 0),
                                                     new Point3d(x0 + d     + r, y0 + (h - h1) / 2, 0),
                                                     Math.PI / 2 - slopeDeg));
        // 右下
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0     + b, y0  + t1, 0),
                                                     new Point3d(x0 + b - r1, y0 + t1, 0), Math.PI / 2 - slopeDeg));
        // 右上 
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0 + b - (r1 - r1 * Math.Sin(slopeDeg)), y0 + h - t + lSlopeY * ratio2, 0),
                                        new Point3d(x0 + b - r1, y0 + h - t1, 0), Math.PI / 2 - slopeDeg));
        // 左上
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0 + d + r - r * Math.Sin(slopeDeg), y0 + h - t - lSlopeY * ratio1, 0),
                                        new Point3d(x0 + d + r, y0 + (h + h1) / 2, 0), Math.PI / 2 - slopeDeg));
    }

    public static void DrawLSteel(DBTrans trans,
                                  Point3d insertPoint,
                                  double  l1,
                                  double  l2,
                                  double  t,
                                  double  rin) {
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;
        // 逆时针画
        // 先画平直部分
        var rout       = rin == 0 ? 0 : rin + t;
        var startPoint = new Point3d(x0 + rout, y0, 0);
        var endPoint   = new Point3d(x0 + l2, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + l2, y0, 0);
        endPoint   = new Point3d(x0 + l2, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + l2, y0 + t, 0);
        endPoint   = new Point3d(x0 + t      + rin, y0 + t, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + t, y0 + t + rin, 0);
        endPoint   = new Point3d(x0 + t, y0 + l1, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0     + t, y0 + l1, 0);
        endPoint   = new Point3d(x0, y0 + l1, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + l1, 0);
        endPoint   = new Point3d(x0, y0 + rout, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));

        // 画R角
        if (rin == 0)
            return;
        // 左下圆弧
        trans.CurrentSpace
             .AddEntity(ArcEx.CreateArc(new Point3d(x0, y0 + rout, 0), new Point3d(x0 + rout, y0 + rout, 0), Math.PI / 2),
                        ArcEx.CreateArc(new Point3d(x0 + t, y0 + rout, 0), new Point3d(x0 + rout, y0 + rout, 0),
                                        Math.PI / 2));
    }

    public static void DrawRollLSteel(DBTrans trans,
                                      Point3d insertPoint,
                                      double  B,
                                      double  b,
                                      double  d,
                                      double  r) {
        var r1 = d / 3;
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;

        // 先画直线部分
        var startPoint = new Point3d(x0, y0, 0);
        var endPoint   = new Point3d(x0 + b, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b, y0, 0);
        endPoint   = new Point3d(x0 + b, y0 + d - r1, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - r1, y0 + d, 0);
        endPoint   = new Point3d(x0     + d      + r, y0 + d, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + b - r1, y0 + d, 0);
        endPoint   = new Point3d(x0     + d      + r, y0 + d, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + d, y0     + d + r, 0);
        endPoint   = new Point3d(x0 + d, y0 + B - r1, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + d - r1, y0 + B, 0);
        endPoint   = new Point3d(x0, y0 + B, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0, y0 + B, 0);
        endPoint   = new Point3d(x0, y0, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));

        // 圆弧
        // 右下
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + b, y0 + d - r1, 0),
                                                     new Point3d(x0         + b - r1, y0 + d - r1, 0), Math.PI / 2));
        // 左上
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + d, y0 + B - r1, 0),
                                                     new Point3d(x0         + d - r1, y0 + B - r1, 0), Math.PI / 2));

        // 左下
        trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + d, y0 + d     + r, 0),
                                                     new Point3d(x0 + d     + r, y0 + d + r, 0), Math.PI / 2));
    }

    public static void DrawPile(DBTrans trans,
                                Point3d insertPoint,
                                double  D,
                                double  t) {
        // 两点画圆
        // 外圈
        trans.CurrentSpace.AddEntity(CircleEx.CreateCircle(new Point3d(insertPoint.X - D / 2, insertPoint.Y, 0),
                                                           new Point3d(insertPoint.X + D / 2, insertPoint.Y, 0)));
        // 内圈
        trans.CurrentSpace.AddEntity(CircleEx.CreateCircle(new Point3d(insertPoint.X - D / 2 + t, insertPoint.Y, 0),
                                                           new Point3d(insertPoint.X + D / 2 - t, insertPoint.Y, 0)));
    }

    public static void DrawSquareTube(DBTrans trans,
                                      Point3d insertPoint,
                                      double  w,
                                      double  h,
                                      double  t,
                                      double  r) {
        var x0 = insertPoint.X;
        var y0 = insertPoint.Y;
        w -= t * 2;
        h -= t * 2;
        // 画内圈
        var startPoint = new Point3d(x0 - w / 2 + r, y0 - h / 2, 0);
        var endPoint   = new Point3d(x0 + w / 2 - r, y0 - h / 2, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + w / 2, y0 + h / 2 - r, 0);
        endPoint   = new Point3d(x0 + w / 2, y0 - h / 2 + r, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + w / 2 - r, y0 + h / 2, 0);
        endPoint   = new Point3d(x0 - w / 2 + r, y0 + h / 2, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 - w / 2, y0 + h / 2 - r, 0);
        endPoint   = new Point3d(x0 - w / 2, y0 - h / 2 + r, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        if (r != 0) {
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 - w / 2, y0 - h / 2 + r, 0),
                                                         new Point3d(x0 - w / 2 + r, y0 - h / 2 + r, 0), Math.PI / 2));
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + w / 2 - r, y0 - h / 2, 0),
                                                         new Point3d(x0 + w / 2 - r, y0 - h / 2 + r, 0), Math.PI / 2));
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + w / 2, y0 + h / 2 - r, 0),
                                                         new Point3d(x0 + w / 2 - r, y0 + h / 2 - r, 0), Math.PI / 2));
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 - w / 2 + r, y0         + h / 2, 0),
                                                         new Point3d(x0 - w / 2 + r, y0 + h / 2 - r, 0), Math.PI / 2));
        }

        w          += t * 2;
        h          += t * 2;
        r          =  r == 0 ? 0 : r + t;
        startPoint =  new Point3d(x0 - w / 2 + r, y0 - h / 2, 0);
        endPoint   =  new Point3d(x0 + w / 2 - r, y0 - h / 2, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + w / 2, y0 + h / 2 - r, 0);
        endPoint   = new Point3d(x0 + w / 2, y0 - h / 2 + r, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 + w / 2 - r, y0 + h / 2, 0);
        endPoint   = new Point3d(x0 - w / 2 + r, y0 + h / 2, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        startPoint = new Point3d(x0 - w / 2, y0 + h / 2 - r, 0);
        endPoint   = new Point3d(x0 - w / 2, y0 - h / 2 + r, 0);
        trans.CurrentSpace.AddEntity(new Line(startPoint, endPoint));
        if (r != 0) {
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 - w / 2, y0 - h / 2 + r, 0),
                                                         new Point3d(x0 - w / 2 + r, y0 - h / 2 + r, 0), Math.PI / 2));
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + w / 2 - r, y0 - h / 2, 0),
                                                         new Point3d(x0 + w / 2 - r, y0 - h / 2 + r, 0), Math.PI / 2));
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 + w / 2, y0 + h / 2 - r, 0),
                                                         new Point3d(x0 + w / 2 - r, y0 + h / 2 - r, 0), Math.PI / 2));
            trans.CurrentSpace.AddEntity(ArcEx.CreateArc(new Point3d(x0 - w / 2 + r, y0         + h / 2, 0),
                                                         new Point3d(x0 - w / 2 + r, y0 + h / 2 - r, 0), Math.PI / 2));
        }
    }

#endregion

#region 跟踪支架GA辅助

    private static Dictionary<string, string> GetAttrDic(TrackerModel trackerModel) {
        var properties = typeof(TrackerModel).GetProperties();
        var attrDic = properties.Where(propertyInfo => CadNameDictionarys.AttrNameDic.ContainsKey(propertyInfo.Name))
                                .ToDictionary(propertyInfo => CadNameDictionarys.AttrNameDic[propertyInfo.Name],
                                              propertyInfo => propertyInfo.GetValue(trackerModel).ToString());
        // 将立柱信息填入动态块
        attrDic.Add("立柱序号", "");
        attrDic.Add("立柱类型", "");
        attrDic.Add("立柱截面类型", "");
        attrDic.Add("立柱截面规格", "");
        attrDic.Add("立柱截面材质", "");
        attrDic.Add("左侧跨距", "");
        attrDic.Add("右侧跨距", "");
        attrDic.Add("左侧开断", "");
        attrDic.Add("右侧开断", "");
        attrDic.Add("基础露头", "");
        attrDic.Add("基础埋深", "");
        var postList = trackerModel.PostList;
        // 立柱序号
        if (postList != null) {
            for (var i = 0; i < postList.Count; i++) {
                attrDic["立柱序号"]   += postList[i].Num;
                attrDic["立柱类型"]   += postList[i].IsDrive ? "驱动立柱" : "普通立柱";
                attrDic["立柱截面类型"] += postList[i].SectionType;
                attrDic["立柱截面规格"] += postList[i].Section;
                attrDic["立柱截面材质"] += postList[i].Material;
                attrDic["左侧跨距"]   += postList[i].LeftSpan;
                attrDic["右侧跨距"]   += postList[i].RightSpan;
                attrDic["左侧开断"]   += postList[i].LeftToBeam;
                attrDic["右侧开断"]   += postList[i].RightToBeam;
                attrDic["基础露头"]   += postList[i].PileUpGround;
                attrDic["基础埋深"]   += postList[i].PileDownGround;
                if (i == postList.Count - 1)
                    continue;
                attrDic["立柱序号"]   += "\\P";
                attrDic["立柱类型"]   += "\\P";
                attrDic["立柱截面类型"] += "\\P";
                attrDic["立柱截面规格"] += "\\P";
                attrDic["立柱截面材质"] += "\\P";
                attrDic["左侧跨距"]   += "\\P";
                attrDic["右侧跨距"]   += "\\P";
                attrDic["左侧开断"]   += "\\P";
                attrDic["右侧开断"]   += "\\P";
                attrDic["基础露头"]   += "\\P";
                attrDic["基础埋深"]   += "\\P";
            }
        }

        var beamList = trackerModel.BeamList;
        attrDic.Add("主梁序号", "");
        attrDic.Add("主梁截面类型", "");
        attrDic.Add("主梁截面规格", "");
        attrDic.Add("主梁截面材质", "");
        attrDic.Add("分段长度", "");
        if (beamList != null) {
            for (var i = 0; i < beamList.Count; i++) {
                attrDic["主梁序号"]   += beamList[i].Num;
                attrDic["主梁截面类型"] += beamList[i].SectionType;
                attrDic["主梁截面规格"] += beamList[i].Section;
                attrDic["主梁截面材质"] += beamList[i].Material;
                attrDic["分段长度"]   += beamList[i].Length;
                if (i == beamList.Count - 1)
                    continue;
                attrDic["主梁序号"]   += "\\P";
                attrDic["主梁截面类型"] += "\\P";
                attrDic["主梁截面规格"] += "\\P";
                attrDic["主梁截面材质"] += "\\P";
                attrDic["分段长度"]   += "\\P";
            }
        }


        return attrDic;
    }

    public static void WriteToInput(DBTrans      trans,
                                    Point3d      insertPoint,
                                    TrackerModel trackerModel,
                                    Scale3d      scale,
                                    double       rotation,
                                    string       blockName) {
        if (!trans.BlockTable.Has(blockName)) return;
        trans.CurrentSpace.InsertBlock(insertPoint, blockName, scale, rotation, GetAttrDic(trackerModel));
    }

    public static void SaveToInput(DBTrans        trans,
                                   BlockReference blockReference,
                                   TrackerModel   trackerModel) {
        var insertPoint = blockReference.Position;
        var scale       = blockReference.ScaleFactors;
        var rotation    = blockReference.Rotation;
        var blockName   = blockReference.BlockTableRecord;
        blockReference.ObjectId.Erase();
        WriteToInput(trans, insertPoint, trackerModel, scale, rotation, blockReference.Name);
    }

    public static void GetTrackerGA(DBTrans      trans,
                                    TrackerModel trackerModel) {
    }

#endregion
}