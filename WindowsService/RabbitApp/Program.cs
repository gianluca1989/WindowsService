using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using Newtonsoft.Json;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;

namespace RabbitApp
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //create files to print information
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            Send("prova", new CustomerTest(5,"Luigi","Tommasello"));
        }

        //Inserts a new element to the queue on rabbitmq converting it first into a json file.
        //The element is inserted only if it is an element in a pre-existing queue, otherwise it throws an exception
        public static void Send(string queue, CustomerTest data)
        {
            log.Info("Send start");
            using (IConnection connection = GetConnection())
            {
                using (IModel channel = connection.CreateModel())
                {
                    try
                    {
                        IBasicProperties basicProperties = channel.CreateBasicProperties();
                        basicProperties.SetPersistent(true);
                        string jsonified = JsonConvert.SerializeObject(data);
                        channel.QueueDeclarePassive(queue);
                        //channel.QueueDeclare(queue, false, false, false, null);//enable to insert a new queue on rabbitmq and disable QueueDeclarePassive
                        channel.BasicPublish(string.Empty, queue, basicProperties, Encoding.UTF8.GetBytes(jsonified));
                    }
                    catch(RabbitMQ.Client.Exceptions.OperationInterruptedException e)
                    {
                        log.Error("Queue non found\n" + e);
                    }
                    catch(Exception ex)
                    {
                        log.Error(ex);
                    }
                    
                }
            }
        }

        //defines the parameters to create the connection and create it ... the defout parameters are set
        public static IConnection GetConnection( string userName= "guest", string password = "guest", string virtualHost= "/", string hostName = "localhost")
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
