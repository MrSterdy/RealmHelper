using RealmHelper.Application;
using RealmHelper.Infrastructure;
using RealmHelper.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUiServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Error");

app.MapRazorPages();
app.MapControllers();

app.Run();