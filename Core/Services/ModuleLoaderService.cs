using Core.Events;

namespace Core.Services
{
    public class ModuleLoaderService
    {
        private static readonly Lazy<IEventAggregator> _lazyEventAggregator =
            new(() => ContainerLocator.Container.Resolve<IEventAggregator>());

        private static IEventAggregator EventAggregator => _lazyEventAggregator.Value;

        public ModuleLoaderService(string moduleName, Action<string> action)
        {
            EventAggregator
                .GetEvent<LoadModuleEvent>()
                .Subscribe(action, ThreadOption.UIThread, true, name =>
                {
                    var pos = name.IndexOf('#');
                    return (pos > 0) ? name[..pos] == moduleName : name == moduleName;
                });
        }
    }
}
