using EmailService;
using EmailService.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<IEmailService, EmailService.Service.EmailService>();

builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();
host.Run();
