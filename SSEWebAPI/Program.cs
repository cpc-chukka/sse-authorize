using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
      .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
      .Build();

// Add services to the container.

builder.Services.AddControllers();
var jwtConfig = configuration.GetSection("JWT");
const string JwtDescription
        = "JWT Authorization header using the Bearer scheme (e.g. \"Authorization: Bearer {token}\")";
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = JwtDescription,
                        Name = "Authorization",
                        In = ParameterLocation.Header

                    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference()
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                             },
                        Array.Empty<string>()
                     }
                    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Audience = jwtConfig["aud"];
            options.Authority = jwtConfig["iss"];
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
