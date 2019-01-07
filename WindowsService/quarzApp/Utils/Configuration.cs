
using System.IO;
using Microsoft.Extensions.Configuration;
using log4net;

namespace quarzApp.Utils
{
    class Configuration
    {
        private IConfigurationRoot config;
        private static ILog log;

        public Configuration(ILog logger)
        {
            log = logger;

            //Setto come current directory la bin del progetto(altrimenti una volta pubblicato come servizio
            //la current directory era C:\\Windows\\System32
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            string path = Directory.GetCurrentDirectory();
            log.Info( $"Directory: {path}");

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            config = builder.Build();
        }

        public string GetField(string name)
        {
            return config[name];
        }
    }
}
