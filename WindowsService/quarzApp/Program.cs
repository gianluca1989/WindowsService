using Quartz;
using Quartz.Impl;
using System;
using log4net;
using System.Collections.Specialized;
using System.Reflection.Metadata;
using System.Xml;
using System.IO;
using System.Reflection;
using log4net.Config;
using quarzApp.topShelf;
using DasMulli.Win32.ServiceUtils;
using System.Configuration;

namespace quarzApp
{
    class Program
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //create files to print information
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));


            var myService = new MyService();
            //create service
            var serviceHost = new Win32ServiceHost(myService);
            //start windows service
            serviceHost.Run();

           //myService.Start(null,null);

            Console.ReadLine();
        }
    }
}
