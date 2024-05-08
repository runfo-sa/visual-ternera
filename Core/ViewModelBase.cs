namespace Core
{
    public abstract class ViewModelBase(Settings settings) : ObservableObject
    {
        protected Settings Settings { get; set; } = settings;
    }
}
