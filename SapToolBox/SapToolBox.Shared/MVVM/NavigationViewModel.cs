using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;

namespace SapToolBox.Shared.MVVM {
    public class NavigationViewModel : BindableBase, INavigationAware {
        private readonly IContainerProvider _provider;
        public readonly  IEventAggregator   Aggregator;

        public NavigationViewModel() {
        }

        public NavigationViewModel(IContainerProvider provider) {
            _provider  = provider;
            Aggregator = provider.Resolve<IEventAggregator>();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext) {
        }
    }
}