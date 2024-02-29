using CADToolBox.Shared.Models.CADModels.Implement;

namespace CADToolBox.Main.GAHelper;

public class TrackerGAHelper(
    TrackerModel trackerModel
) {
    public TrackerModel GAModel { get; } = trackerModel;
}