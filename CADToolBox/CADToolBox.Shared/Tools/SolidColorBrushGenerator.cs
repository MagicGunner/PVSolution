using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CADToolBox.Shared.Tools;

public class SolidColorBrushGenerator {
    public static List<SolidColorBrush> GenerateSolidColorBrushes(int count) {
        if (count <= 0) { throw new ArgumentException("数量只能为正数"); }

        var brushes = new List<SolidColorBrush> {
                                                    Capacity = 0
                                                };
        const double saturation = 0.8; // 调整饱和度
        const double value      = 0.8; // 调整亮度

        for (var i = 0; i < count; i++) {
            var hue   = (360.0 / count) * i;
            var color = HSVToRGB(hue, saturation, value);
            var brush = new SolidColorBrush(color);
            brushes.Add(brush);
        }

        return brushes;
    }

    private static Color HSVToRGB(double hue,
                                  double saturation,
                                  double value) {
        var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        var f  = hue / 60 - Math.Floor(hue / 60);

        value *= 255;
        var v = Convert.ToInt32(value);
        var p = Convert.ToInt32(value * (1 - saturation));
        var q = Convert.ToInt32(value * (1 - f       * saturation));
        var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        return hi switch {
                   0 => Color.FromRgb((byte)v, (byte)t, (byte)p),
                   1 => Color.FromRgb((byte)q, (byte)v, (byte)p),
                   2 => Color.FromRgb((byte)p, (byte)v, (byte)t),
                   3 => Color.FromRgb((byte)p, (byte)q, (byte)v),
                   4 => Color.FromRgb((byte)t, (byte)p, (byte)v),
                   _ => Color.FromRgb((byte)v, (byte)p, (byte)q)
               };
    }
}