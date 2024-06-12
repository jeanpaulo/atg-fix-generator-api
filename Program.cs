using OrderGenerator.API;
using OrderGenerator.API.DTO;
using QuickFix.Transport;
using QuickFix;
using Microsoft.AspNetCore.SignalR;
using OrderGenerator.API.hub;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();

try
{
	var file = new StreamReader("./client.cfg");

	SessionSettings settings = new SessionSettings(file);
	FIXCommunicator application = new FIXCommunicator();
	IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
	ILogFactory logFactory = new FileLogFactory(settings);

	builder.Services.AddSingleton(application);

	var initiator = new SocketInitiator(application, storeFactory, settings, logFactory);
	//var initiator = new ThreadedSocketAcceptor(application, storeFactory, settings, logFactory);

	initiator.Start();
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
}



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/ordergenerator", async (
	[FromBody] OrderGeneratorRequestDTO request,
	HttpContext context,
	FIXCommunicator fixApp) =>
{
    if (!MiniValidator.TryValidate(request, out var errors))
    {
        return Results.BadRequest(errors);
    }

    try
	{
		await fixApp.Run(request);
	}
	catch (Exception ex)
	{
		return Results.BadRequest(ex.Message);
	}

	return Results.Ok();
})
.WithName("ordergenerator")
.WithOpenApi();

app.MapHub<FixHub>("/fixhub");
app.MapPost("broadcast", async (string message, IHubContext<FixHub, IFixHub> context) =>
{
    await context.Clients.All.ReceiveMessage(message);

    return Results.NoContent();
});


app.UseCors();
app.Run();

