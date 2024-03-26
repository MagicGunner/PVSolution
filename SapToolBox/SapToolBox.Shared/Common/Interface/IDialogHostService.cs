using System.Threading.Tasks;
using Prism.Services.Dialogs;

namespace SapToolBox.Shared.Common.Interface {
    public interface IDialogHostService : IDialogService {
        Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "Root");
    }
}
