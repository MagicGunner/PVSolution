using System.Threading.Tasks;
using Prism.Services.Dialogs;

namespace SapToolBox.Base.Common.Interface {
    public interface IDialogHostService : IDialogService {
        Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "Root");
    }
}
