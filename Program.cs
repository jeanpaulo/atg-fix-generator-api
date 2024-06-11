using OrderGenerator.API;
using OrderGenerator.API.DTO;
using QuickFix.Transport;
using QuickFix;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapPost("/ordergenerator", async (OrderGeneratorRequestDTO request, FIXCommunicator fixApp) =>
{
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



app.Run();

