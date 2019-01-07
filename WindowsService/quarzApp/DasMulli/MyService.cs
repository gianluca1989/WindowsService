using DasMulli.Win32.ServiceUtils;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using NLog;
using quarzApp.Utils;
using log4net;

namespace quarzApp.topShelf
{
    class MyService : IWin32Service
    {

        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ServiceName => "Test service";

        private IScheduler scheduler;

        public MyService()
        {
            try
            {
                var config = new Configuration(log).GetField("QuartzJobConfigPath");

                //create job and triggers from xml configuration files
                var properties = new NameValueCollection
                {
                    ["quartz.scheduler.instanceName"] = "XmlConfiguredInstance",
                    ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                    ["quartz.threadPool.threadCount"] = "5",
                    ["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
                    ["quartz.plugin.xml.fileNames"] = $"{config}",
                    // this is the default
                    ["quartz.plugin.xml.FailOnFileNotFound"] = "true",
                    // this is not the default
                    ["quartz.plugin.xml.failOnSchedulingError"] = "true"

                };
                //create the scheduler for start the job
                Quartz.ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                scheduler = schedulerFactory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
                //IScheduler scheduler = await schedulerFactory.GetScheduler();
            }
            catch(Exception e)
            {
                log.Error($"Error{System.Environment.NewLine}{e.Message}");
            }
        }


        // Windows Service start.
        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {

            log.Info("Scheduler start");
            try
            {
                //Start the job
                scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch(Exception e)
            {
                log.Error($"Eerror when starting the job{System.Environment.NewLine}{e.Message}");
            }
            
        }

        // Windows Service stops.
        public void Stop()
        {
            //var cancelToken = new CancellationTokenSource();
            //cancelToken.Cancel();

            try
            {
                log.Info("Scheduler stop");

                scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch(Exception e)
            {
                log.Error($"Eerror when stop the job{System.Environment.NewLine}{e.Message}");
            }
           
        }

        //defines job and triggers from xml configuration files and starts the job by calling the execute
        public void RecallXml()
        {
            try
            {
                var config = new Configuration(log).GetField("QuartzJobConfigPath");

                //create job and triggers from xml configuration files
                var properties = new NameValueCollection
                {
                    ["quartz.scheduler.instanceName"] = "XmlConfiguredInstance",
                    ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                    ["quartz.threadPool.threadCount"] = "5",
                    ["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins",
                    ["quartz.plugin.xml.fileNames"] = $"{config}",
                    // this is the default
                    ["quartz.plugin.xml.FailOnFileNotFound"] = "true",
                    // this is not the default
                    ["quartz.plugin.xml.failOnSchedulingError"] = "true"

                };
                //create the scheduler for start the job
                Quartz.ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                IScheduler scheduler = schedulerFactory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
                //IScheduler scheduler = await schedulerFactory.GetScheduler();
                log.Info("\n\n-------- num: 10 sec\n" + scheduler.SchedulerName + "\n------------");
                //Start the job
                scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();

                //await scheduler.Start();
            }
            catch (Exception e)
            {
                log.Error($"Error{System.Environment.NewLine}{e.Message}");
            }
        }

        //defines job and triggers and starts the job by calling the execute
        public async void RecallNoXml()
        {
            try
            {
                //create the scheduler for start the job
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                IScheduler scheduler = await schedulerFactory.GetScheduler();

                //start the job
                await scheduler.Start();

                //define the job
                IJobDetail job = new JobDetailImpl(
                                 "JobQueue",
                                 null,
                                 typeof(JobQueue));

                //define the trigger
                ITrigger trigger = new Quartz.Impl.Triggers.SimpleTriggerImpl(
                              "trigger",
                              DateTime.UtcNow,
                              null,
                              Quartz.Impl.Triggers.SimpleTriggerImpl.RepeatIndefinitely,
                              new TimeSpan(0, 0, 10));

                await scheduler.ScheduleJob(job, trigger);
            }
            catch(Exception e)
            {
                log.Error($"Error{System.Environment.NewLine}{e.Message}");
            }
            
        }

    }
}
