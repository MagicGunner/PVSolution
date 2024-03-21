namespace CADToolBox.Modules.FixedGA.Messages;

public class WindowSizeChangedMessage(
    double canvasWidth,
    double canvasHeight
) {
    public double CanvasWidth  { get; set; } = canvasWidth;
    public double CanvasHeight { get; set; } = canvasHeight;
}