using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SimpleCut.Common.Options;
using SimpleCut.Infrastructure.Context;
using SimpleCut.Infrastructure.Cqrs;
using SimpleCut.Infrastructure.BehaviourPipelines;
using System.Reflection;
using System.Text;
using FluentValidation;
using SimpleCut.Infrastructure.Services.Accounts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatR(Assembly.GetCallingAssembly(),
                Assembly.Load("SimpleCut.Logic"));

builder.Services.AddScoped<IDbContext>(
    x => new DbContext(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IDispatcher, Dispatcher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection(TokenOptions.Position));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<>));
builder.Services.AddValidatorsFromAssembly(Assembly.Load("SimpleCut.Logic"));

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var tokenKey = Encoding.ASCII.GetBytes(
        builder.Configuration.GetValue<string>("Token:Secrete"));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();

app.Run();
