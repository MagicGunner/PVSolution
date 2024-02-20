#nullable enable
    using Prism.Mvvm;

    namespace SapToolBox.Shared.Models {
        public class MenuBar : BindableBase {
            public string? Icon { get; set; }

            public string? Title { get; set; }

            public string? NameSpace { get; set; }
        }
    }