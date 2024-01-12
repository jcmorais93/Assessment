using Confluent.Kafka;
using Newtonsoft.Json;
using Pixel;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddSingleton<IProducer<Null, string>>(_ =>
{
    var config = new ProducerConfig { BootstrapServers = configuration["StorageEndpoint"] };
    return new ProducerBuilder<Null, string>(config).Build();
});

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/pixel-service/track", async (HttpContext context, IProducer<Null, string> kafkaProducer) =>
{
    var tracker = new Tracker
    {
        Referer = context.Request.Headers["Referer"].ToString(),
        UserAgent = context.Request.Headers["User-Agent"].ToString(),
        IpAddress = context.Connection.RemoteIpAddress?.ToString()
    };
    
    await kafkaProducer.ProduceAsync("pixel-topic", new Message<Null, string> { Value = JsonConvert.SerializeObject(tracker) });
    
    context.Response.ContentType = "image/gif";
    await context.Response.Body.WriteAsync(GifBytes.PiXel);
});

app.Run();