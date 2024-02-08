using Kafka.Producer.API;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ProducerService>();

var app = builder.Build();

app.MapPost("/pessoa", async ([FromServices]ProducerService service,[FromBody] PessoaModel pessoa)=>
{
    return await service.SendMessage(pessoa);
});

app.Run();
