var builder = WebApplication.CreateBuilder(args);

// הוספת שירותי Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הוספת שירותי Controller
builder.Services.AddControllers();  // <--- חשוב!!!

var app = builder.Build();

// הפעלת Swagger במצב פיתוח
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// הפעלת Routing ל-Controllers
app.MapControllers();  // <--- חשוב!!!

Console.BackgroundColor = ConsoleColor.Green;
Console.WriteLine("Now listening on: http://localhost:5155");
Console.BackgroundColor = ConsoleColor.DarkBlue;
Console.WriteLine("http://localhost:5155/api/aircraft");
Console.ResetColor();

app.Run();
