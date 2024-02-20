using CADToolBox.Modules.TrackerGA;

namespace CADToolBox.Main;

public class Command {
    [CommandMethod(nameof(HelloWorld))]
    public void HelloWorld() {
        using var tr = new DBTrans();
        Env.Editor.WriteMessage("Hello 2222!");
    }

    [CommandMethod(nameof(TestWpf))]
    public void TestWpf() {
        TrackerApp.Current.StartUp();
    }
}