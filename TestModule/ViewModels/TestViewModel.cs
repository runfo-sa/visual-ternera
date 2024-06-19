namespace TestModule.ViewModels
{
    public class TestViewModel : BindableBase
    {
        private string _title = "Test Title";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
