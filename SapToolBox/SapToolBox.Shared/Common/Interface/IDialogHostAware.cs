using Prism.Commands;
using Prism.Services.Dialogs;

namespace SapToolBox.Shared.Common.Interface {
    public interface IDialogHostAware {
        string DialogHostName { get; set; }

        void OnDialogOpened(IDialogParameters parameters);

        DelegateCommand SaveCommand   { get; set; }
        DelegateCommand CancelCommand { get; set; }
    }
}