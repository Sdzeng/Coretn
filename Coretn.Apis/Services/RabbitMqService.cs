using Coretn.Apis.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coretn.Apis.Services
{
    public class RabbitMqService  
    {
        private IConnectionFactory _conFactory;
        public RabbitMqService(IConnectionFactory conFactory)
        {
            _conFactory = conFactory;
        }

        public void BasicPublish(MqRequestModel mqRequest)
        {
            Console.WriteLine($"发送线程{Thread.CurrentThread.ManagedThreadId}");
            using (IConnection con = _conFactory.CreateConnection())//创建连接对象
            {
                using (IModel channel = con.CreateModel())//创建连接会话对象
                {
                    string queueName = "mqqueue1";
                    //声明一个队列
                    channel.QueueDeclare(
                      queue: queueName,//消息队列名称
                      durable: false,//是否缓存
                      exclusive: false,
                      autoDelete: false,
                      arguments: null
                       );
                    while (true)
                    {
                        //消息内容
                        var message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Console.WriteLine("发送消息:" + message);
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        //发送消息
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                        Thread.Sleep(1000);
                    }
                }
            }
        }


 

        public bool Received()
        {
        
            using (IConnection conn = _conFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    string queueName = "mqqueue1";
                  
                    //声明一个队列
                    channel.QueueDeclare(
                      queue: queueName,//消息队列名称
                      durable: false,//是否缓存
                      exclusive: false,
                      autoDelete: false,
                      arguments: null
                       );
                    //创建消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        Console.WriteLine($"接收线程{Thread.CurrentThread.ManagedThreadId}");
                        byte[] message = ea.Body;//接收到的消息
                        Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message));
                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    //消费者开启监听
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                 
                    Thread.Sleep(2 * 60 * 1000);
                    Console.WriteLine($"监听线程{Thread.CurrentThread.ManagedThreadId}结束");

                  
                }
            }
            return true;
        }
    }
}
