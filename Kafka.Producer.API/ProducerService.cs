using System.Runtime.CompilerServices;
using System.Text.Json;
using Confluent.Kafka;

namespace Kafka.Producer.API
{
    public class ProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly ProducerConfig _producerConfig;
        private readonly ILogger<ProducerService> _logger;

        public ProducerService(IConfiguration configuration, ILogger<ProducerService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var bootstrap = _configuration.GetSection("KafkaConfig").GetSection("BootstrapServer").Value;

            _producerConfig = new ProducerConfig()
            {
                BootstrapServers = bootstrap
            };
        }

        public async Task<string> SendMessage(PessoaModel pessoa)
        {
            var message= JsonSerializer.Serialize(pessoa);
            return await SendMessage(message);
        }
        public async Task<string> SendMessage(string message)
        {
            var topic = _configuration.GetSection("KafkaConfig").GetSection("TopicName").Value;
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
                {
                    var result = await producer.ProduceAsync(topic, new() { Value = message });

                    var strResult = $"{result.Status.ToString()} - {message}";

                    _logger.LogInformation(strResult);

                    return strResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return "Erro ao enviar mensagem.";
            }
        }
    }
}