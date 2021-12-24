using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SchoolContextSQLite")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var maxModelBindingCollectionSize = Convert.ToInt32(builder.Configuration["MaxModelBindingCollectionSize"] ?? "100");
builder.Services.Configure<MvcOptions>(options =>
    options.MaxModelBindingCollectionSize = maxModelBindingCollectionSize);

// Force en-US for a consistent culture.
var supportedCultures = new[] { "en-US" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);


var app = builder.Build();
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SchoolContext>();
    DbInitializer.Initialize(context);
}    

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();