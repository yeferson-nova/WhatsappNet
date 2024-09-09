using WhatsappNet.Api.Services.WhatsappCloud.SendMessages;
using WhatsappNet.Api.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IWhasappCloudSendMessage, WhasappCloudSendMessage>();
builder.Services.AddSingleton<IUtil, Util>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
