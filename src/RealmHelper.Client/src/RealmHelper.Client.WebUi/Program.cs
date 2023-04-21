using RealmHelper.Client.Application;
using RealmHelper.Client.Infrastructure;
using RealmHelper.Client.WebUi;

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

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Error");

app.MapRazorPages();
app.MapControllers();

app.Run();