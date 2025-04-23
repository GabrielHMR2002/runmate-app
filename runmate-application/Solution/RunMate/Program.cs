using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RunMate.RunMate.Application.Interfaces;
using RunMate.RunMate.Application.Services;
using System.Text;
using RunMate.Domain.Entities;
using RunMate.RunMate.Application.DTOs.UserDTOs;
using RunMate.User.RunMate.Application.DTOs.UserDTOs;

// Imports do MassTransit
using MassTransit;
using RunMate.Authentication.RunMate.Infrastructure.Persistence.Context;

// Import para o EventBus com namespaces corretos
using RunMate.User.RunMate.Infrastructure.Messaging.EventBus;
using RunMate.User.RunMate.Infrastructure.Messaging.EventBus.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Configuração do AutoMapper diretamente no Program.cs para evitar ambiguidades e problemas de namespace
builder.Services.AddSingleton<IMapper>(provider => {
    var config = new MapperConfiguration(cfg => {
        // Configure todos os mapeamentos necessários diretamente aqui
        cfg.CreateMap<UserEntity, UserDto>();

        // Adicione outros mapeamentos que sua aplicação necessita
        // cfg.CreateMap<RegisterUserDto, UserEntity>();
        // cfg.CreateMap<UserEntity, UserDetailDto>();
        // etc.
    });

    // Valide a configuração em ambiente de desenvolvimento
    if (builder.Environment.IsDevelopment())
    {
        config.AssertConfigurationIsValid();
    }

    return config.CreateMapper();
});

// Add services to the container.
builder.Services.AddDbContext<RunMateContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("RunMateDatabase")));

// Adicionar CORS para permitir requisições de outros microsserviços
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração do MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Não consumimos mais eventos, apenas publicamos

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"];
        var username = builder.Configuration["RabbitMQ:Username"];
        var password = builder.Configuration["RabbitMQ:Password"];
        var virtualHost = builder.Configuration["RabbitMQ:VirtualHost"];

        if (!string.IsNullOrEmpty(virtualHost) && virtualHost != "/")
        {
            cfg.Host(host, virtualHost, h =>
            {
                h.Username(username);
                h.Password(password);
            });
        }
        else
        {
            cfg.Host(host, h =>
            {
                h.Username(username);
                h.Password(password);
            });
        }

        cfg.ConfigureEndpoints(context);
    });
});

// Registrar o serviço EventBus com os namespaces corretos
builder.Services.AddScoped<IEventBus, MassTransitEventBus>();

// Configuração da autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Registrar serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>(); // Serviço de usuário adicionado

builder.Services.AddControllers();

// Configurar Swagger para suportar JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RunMate API", Version = "v1" });

    // Adicionar configuração de segurança para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
});
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adicionar middleware CORS antes da autenticação
app.UseCors("AllowAll");

// Importante: UseAuthentication deve vir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RunMateContext>();
    dbContext.Database.Migrate();
}
app.Run();