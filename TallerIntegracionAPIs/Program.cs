using TallerIntegracionAPIs.Data;
using TallerIntegracionAPIs.Interfaces;
using TallerIntegracionAPIs.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("OpenAI", client => { });
builder.Services.AddHttpClient("Gemini", client => { });

builder.Services.AddScoped<OpenAIRepository>();
builder.Services.AddScoped<GeminiRepository>();

builder.Services.AddScoped<IDictionary<string, IChatbotService>>(sp => new Dictionary<string, IChatbotService>
{
    ["openai"] = sp.GetRequiredService<OpenAIRepository>(),
    ["gemini"] = sp.GetRequiredService<GeminiRepository>()
});

builder.Services.AddDbContext<ChatbotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();