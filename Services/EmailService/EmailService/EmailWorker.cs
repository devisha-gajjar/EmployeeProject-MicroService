using System.Text;
using System.Text.Json;
using EmailService.DTOs;
using EmailService.Service;
using Employee.Shared.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailService;

public class EmailWorker : BackgroundService
{
    private readonly ILogger<EmailWorker> _logger;
    private readonly IEmailService _emailSender;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public EmailWorker(ILogger<EmailWorker> logger, IEmailService emailSender, IConfiguration config)
    {
        _logger = logger;
        _emailSender = emailSender;

        // Connection to CloudAMQP
        var connectionString = config["RabbitMQ:ConnectionString"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new AppException("RabbitMQ connection string not found in configuration!");
        }

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Queue
        await _channel.QueueDeclareAsync(queue: "email_queue", durable: true, exclusive: false, autoDelete: false);

        // Consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<EmailRequestDto>(Encoding.UTF8.GetString(body));

            if (message != null)
            {
                _logger.LogInformation("Processing email for: {To}", message.To);
                await _emailSender.SendEmailAsync(message);
            }
        };

        await _channel.BasicConsumeAsync(queue: "email_queue", autoAck: true, consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}