using RealmHelper.Realm.Common.WebApi.Middlewares;
using RealmHelper.Realm.Java.Application;
using RealmHelper.Realm.Java.Infrastructure;
using RealmHelper.Realm.Java.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJavaApplicationServices();
builder.Services.AddJavaInfrastructureServices();
builder.Services.AddJavaWebApiServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.Run();