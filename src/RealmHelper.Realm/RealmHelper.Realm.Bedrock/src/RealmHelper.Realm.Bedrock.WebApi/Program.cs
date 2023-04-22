using RealmHelper.Realm.Common.WebApi.Middlewares;
using RealmHelper.Realm.Bedrock.Application;
using RealmHelper.Realm.Bedrock.Infrastructure;
using RealmHelper.Realm.Bedrock.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBedrockApplicationServices();
builder.Services.AddBedrockInfrastructureServices();
builder.Services.AddBedrockWebApiServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();