using Common.Logging;
using log4net;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using System.Threading;
using quarzApp.Utils;

namespace quarzApp
{
    class JobQueue : IJob
    {
        //Define log for use log4net that prints file information
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //name of the queue on rabbitmq
        private string queue { get; set; }

        //executes the job that every minute takes an element from the rabbitmq's queue
        public virtual Task Execute(IJobExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if(log== null) throw new ArgumentNullException(nameof(log));

            var queue = new Configuration(log).GetField("QueueName");

            try
            {
                //defines the token used to define the task as erasable
                var cancelToken = new CancellationTokenSource();
                CancellationToken token = cancelToken.Token;

                
                if (context.Trigger.JobKey.Name == "JobQueue")
                {
                    //create the task
                    Task taskA = new Task(() =>
                    {
                        token.ThrowIfCancellationRequested();
                        Receive(queue);
                    }, token);
                    log.Info("\n\nStart Task " + context.Trigger.JobKey.Name + "\n\n");
                    taskA.Start();
                    return taskA;
                }
                else
                {
                    Task taskB = new Task(() => {
                        token.ThrowIfCancellationRequested();
                        NewJobWork();
                    }, token);
                    log.Info("\n\nStart Task " + context.Trigger.JobKey.Name + "\n\n");
                    taskB.Start();
                    return taskB;
                }
            }
            catch(Exception e)
            {
                log.Error($"Error{System.Environment.NewLine}{e.Message}");
                throw;
            }

            
        }

        public void NewJobWork()
        {
            log.Info("\n\n-------------------------------\n---------New Job Work--------\n---------------------------------\n\n");
        }

        //method that takes an element from the rabbitmq tail, once this element is taken, it is converted from a json file to one of our objects
        public void Receive(string queue)
        {
            log.Info("\nReceive start");
            //create the connection
            try
            {
                using (IConnection connection = GetConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {

                        channel.QueueDeclare(queue, false, false, false, null);
                        var consumer = new EventingBasicConsumer(channel);
                        BasicGetResult result = channel.BasicGet(queue, true);
                        if (result != null)
                        {
                            //converts the string taken from the queue by a json file to our object
                            string jsonfield = Encoding.UTF8.GetString(result.Body);
                            CustomerTest data = JsonConvert.DeserializeObject<CustomerTest>(jsonfield);
                            Console.WriteLine("\nnome: " + data.nome + "\ncognome: " + data.cognome + "\nnumero: " + data.id + "\nAzienda: " + data.DatiPersonali.Azienda + "\nStipendio: " + data.DatiPersonali.Stipendio + "\nData di nascita: " + data.DatiPersonali.DataNascita);
                        }
                        else log.Info("\nQueue empty");

                    }
                }
            }
            catch(Exception e)
            {
                log.Error($"Connection failed{System.Environment.NewLine}{e.Message}");
            }
            
        }

        //defines the parameters to create the connection and create it ... the defout parameters are set
        public IConnection GetConnection(string userName = "guest", string password = "guest", string virtualHost = "/", string hostName = "localhost")
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostName;
            connectionFactory.UserName = userName;
            connectionFactory.VirtualHost = virtualHost;
            connectionFactory.Password = password;

            try
            {
                    return connectionFactory.CreateConnection();
            }
            catch (Exception ex)
            {

                log.Error("Connection failed\n" + ex);
                throw;
            }
        }

    }
}
