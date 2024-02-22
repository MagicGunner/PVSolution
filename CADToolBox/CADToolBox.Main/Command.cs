using CADToolBox.Modules.TrackerGA;
using CADToolBox.Modules.TrackerGA.Views;
using CADToolBox.Shared.Models.CADModels.Implement;
using Microsoft.Extensions.DependencyInjection;

namespace CADToolBox.Main;

public class Command {
    [CommandMethod(nameof(HelloWorld))]
    public void HelloWorld() {
        using var tr = new DBTrans();
        Env.Editor.WriteMessage("Hello 333!");
    }

    [CommandMethod(nameof(TestWpf))]
    public void TestWpf() {
        //TrackerApp.Services.GetRequiredService<TrackerMainView>().Show();
        //var x = new TrackerApp();
        //x.Run();
        var trackerModel = new TrackerModel(2400);
        TrackerApp.Current.TrackerModel = trackerModel;
        TrackerApp.Current.Run();
        //if (TrackerApp.Services != null) TrackerApp.Services.GetService<TrackerMainView>()!.ShowDialog();
    }
}