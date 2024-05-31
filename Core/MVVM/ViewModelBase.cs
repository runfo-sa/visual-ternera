using Core.Logic;
using Core.MVVM;

namespace Core
{
    public abstract class ViewModelBase(Settings settings) : ObservableObject
    {
        protected Settings Settings { get; set; } = settings;
    }
}
