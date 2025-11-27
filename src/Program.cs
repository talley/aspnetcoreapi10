var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"]!;
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<northwindContext>(options =>
    options.UseNpgsql(DB.CS)
);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/login", async (northwindContext db, LoginRequest login, IConfiguration config) =>
{
    // TODO: authenticate user. Replace with your real auth logic (hash check)
    // Option A: check a UserAuths table
    // Option B: check db.users for matching username/password (not recommended without hashing)
    var user = await db.users.FirstOrDefaultAsync(u => u.name == login.Username);
    if (user == null)
        return Results.Unauthorized();

    // TODO: In real life verify hashed password here
    // If you have a password hash field, validate it (BCrypt/Argon2/etc)

    var token = AuthenticationHelpers.GenerateJwtToken(user.id.ToString(), config);
    return Results.Ok(new { token });
})
.WithName("Login");

app.MapUsersEndPoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record LoginRequest(string Username, string Password);