using Core.Services.SettingsModel;
using Main.View;
using Main.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using YamlDotNet.Serialization;

namespace Main
{
    public partial class App : PrismApplication
    {
        private readonly Core.Logic.Settings _settings;
        public readonly IServiceProvider ServiceProvider;
        public ResourceDictionary ThemeDictionary => Resources.MergedDictionaries[0];

        /*public void _APP()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionPopUp);

            ServiceCollection services = new();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            _settings = ServiceProvider.GetRequiredService<Core.Logic.Settings>();

            // Asignamos la región a utilizar para el formato de números y fechas
            Thread.CurrentThread.CurrentCulture = new CultureInfo(_settings.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(_settings.Culture);
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );

            // Carga el archivo que especifica el syntax highlighting para el lenguaje ZPL
            // Lo hacemos aca para tener que hacerlo una sola vez en el ciclo de vida del programa
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly
                .GetManifestResourceNames()
                .Single(str => str.EndsWith("ZPL.xshd"));

            var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new System.Xml.XmlTextReader(stream!);
            HighlightingManager.Instance.RegisterHighlighting(
                "ZPL",
                [],
                ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(
                    reader,
                    HighlightingManager.Instance
                )
            );
        }*/

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var deserializer = new DeserializerBuilder()
                    .Build();
                return deserializer.Deserialize<Core.Logic.Settings>(File.ReadAllText("Settings.yaml"));
            });
            services.AddSingleton<MainViewModel>();
            services.AddSingleton(provider => new MainView()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ChangeTheme(_settings.Theme);
        }

        public void ChangeTheme(Theme theme)
        {
            var uri = new Uri($"pack://application:,,,/AdonisUI;component/ColorSchemes/{Enum.GetName(theme)}.xaml");
            ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }

        private void ExceptionPopUp(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            ExceptionPopUp popUp = new(e.Message);
            popUp.ShowDialog();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
