using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using RomjulDev2022.Database;
using RomjulDev2022.Server.Options;
using RomjulDev2022.Tokens;
using RomjulDev2022.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://kv-romjuldev2022-prod.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options => 
    {
        options.ClientId = builder.Configuration["Google:ClientId"] 
            ?? throw new ArgumentException("Missing configuration [Google:ClientId].");
        options.ClientSecret = builder.Configuration["Google:ClientSecret"] 
            ?? throw new ArgumentException("Missing configuration [Google:ClientSecret].");
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
builder.Services.Configure<SpotifyOptions>(builder.Configuration.GetSection("Spotify"));

builder.Services.AddCosmosDb(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();