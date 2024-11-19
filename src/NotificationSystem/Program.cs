using NotificationSystem.Brokers;
using NotificationSystem.Handlers;
using NotificationSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IMessageBroker, RabbitMqBroker>();
builder.Services.AddSingleton<IDistributedCache, RedisCache>();
builder.Services.AddSingleton<EmailNotificationHandler>();
builder.Services.AddSingleton<SmsNotificationHandler>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddControllers();

// Configure the HTTP request pipeline
var app = builder.Build();

app.MapControllers();

// RabbitMQ subscriptions
var broker = app.Services.GetRequiredService<IMessageBroker>();
var emailHandler = app.Services.GetRequiredService<EmailNotificationHandler>();
var smsHandler = app.Services.GetRequiredService<SmsNotificationHandler>();

broker.Subscribe("email_queue", async notification => await emailHandler.HandleAsync(notification));
broker.Subscribe("sms_queue", async notification => await smsHandler.HandleAsync(notification));

app.Run("http://0.0.0.0:5000");
