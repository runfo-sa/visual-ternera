﻿using Main.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Main
{
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-MX");
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );

            ServiceCollection services = new();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(HyphenatedNamingConvention.Instance)
                    .Build();
                return deserializer.Deserialize<Core.Settings>(File.ReadAllText("settings.yaml"));
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
            serviceProvider.GetRequiredService<MainView>().Show();
        }
    }
}
