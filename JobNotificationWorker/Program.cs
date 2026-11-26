using JobNotificationWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();