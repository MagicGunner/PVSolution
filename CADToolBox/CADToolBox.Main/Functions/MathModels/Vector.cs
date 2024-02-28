namespace CADToolBox.Main.Functions.MathModels;

public class Vector(
    double x,
    double y,
    double z
) {
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Z { get; set; } = z;

    public Vector(double x,
                  double y) : this(x, y, 0) {
    }
}