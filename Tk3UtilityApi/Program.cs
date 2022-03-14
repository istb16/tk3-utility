var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "SiteCorsPolicy", builder => {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("SiteCorsPolicy");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => $"Welcome to running tk3.biz utility. Builded at {File.GetLastWriteTimeUtc(System.Reflection.Assembly.GetExecutingAssembly().Location)}");

app.Run();
