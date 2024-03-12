namespace CADToolBox.Shared.Models.CADModels.Interface;

public interface IItemModel {
    public double StartX { get; set; }
    public double EndX   { get; set; }

    public IItemModel? PreItem  { get; set; }
    public IItemModel? NextItem { get; set; }
}