using Prism.Mvvm;

namespace SapToolBox.Base.Sap2000 {
    public class OverWriteObj : BindableBase {
        public int Index { get; set; }

        public double Value { get; set; }

        public bool IsDefault { get; set; }

        public string DisplayName { get; set; }

        public bool NeedModify { get; set; }

        public PropertyObj Property { get; set; }
    }
}