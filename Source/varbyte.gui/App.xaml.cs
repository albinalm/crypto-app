using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using varbyte.encryption.Interfaces;
using varbyte.encryption.ORM;
using varbyte.encryption.Service;
using varbyte.gui.View;

namespace varbyte.gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainView>();
            services.AddSingleton<Services>();
  
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainView>();
            mainWindow?.Show();
        }
    }
}