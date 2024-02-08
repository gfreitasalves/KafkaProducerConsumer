using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Consumer.Console
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ConsumerConfig _consumerConfig;
        private readonly ILogger<ConsumerService> _logger;
        private readonly ParametersModel _parameters;

        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;

            _parameters = new ParametersModel();
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _parameters.BootstrapServer,
                GroupId = _parameters.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Waiting Messages.");


            _consumer.Subscribe(_parameters.TopicName);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var result = _consumer.Consume(stoppingToken);

                        var message = JsonSerializer.Deserialize<PessoaModel>(result.Message.Value);
                        if (message != null)
                        {
                            _logger.LogInformation($"{_parameters.GroupId} Olá, {message.Name}({message.Id}) seu email será enviado para {message.Email}.");
                        }
                        else
                        {
                            throw new NullReferenceException("Empty or invalid queue messge");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{_parameters.GroupId} ERROR - {ex.Message}.", ex);

                    }
                });                
            }
        }


        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _consumer.Close();
            _logger.LogInformation("Stopped - Closed Connection");
            return Task.CompletedTask;
        }
    }
}