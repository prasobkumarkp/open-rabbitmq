// See https://aka.ms/new-console-template for more information
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var queueName = "psb.broadcast-pluto";
var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();
channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

var message = new { From = "earth", Message = "hello!" };
var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
channel.BasicPublish("", queueName, null, body);

//---------------------------------------------------------

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var body = e.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(message);
};
channel.BasicConsume(queueName,true,consumer);

Console.ReadLine();