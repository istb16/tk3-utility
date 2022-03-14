var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    var allowedHosts = builder.Configuration.GetValue(typeof(string), "AllowedHosts") as string;
    options.AddDefaultPolicy(
        builder =>
        {
            if (allowedHosts == null || allowedHosts == "*")
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
                return;
            }
            string[] hosts;
            if (allowedHosts.Contains(';')) hosts = allowedHosts.Split(';');
            else
            {
                hosts = new string[1];
                hosts[0] = allowedHosts;
            }
            builder.WithOrigins(hosts)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running tk3.biz utility.");

app.Run();
