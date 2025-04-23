using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RunMate.EngagementService.Application.Interfaces;
using RunMate.EngagementService.Application.Services;
using RunMate.EngagementService.Infrastructure.Persistence.Repositories;
using RunMate.EngagementService.RunMate.EngagementService.Infrastructure.Messaging.Consumers.User;
using RunMate.EngagementService.RunMate.EngagementService.Infrastructure.Persistence;
using RunMate.Shared.Auth.Auth;
// Imports para os novos consumidores de eventos

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddDbContext<RunMateEngagementDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("RunMateEngagementDatabase")));

builder.Services.AddControllers();

// Adicione a autenticação JWT compartilhada
builder.Services.AddSharedJwtAuthentication(builder.Configuration);

// Registrar interfaces e implementações para injeção de dependência
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Registrar AutoMapper
builder.Services.AddSingleton(provider => {
    var config = new AutoMapper.MapperConfiguration(cfg => {
        // Adicione seus perfis de mapeamento aqui, se necessário
        cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
    });
    return config.CreateMapper();
});

// Configurar MassTransit com RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Registrar consumidores de eventos do RunMate.User
    x.AddConsumer<UserCreatedEventConsumer>();
    //x.AddConsumer<UserUpdatedEventConsumer>();
    //x.AddConsumer<UserDeletedEventConsumer>();
    //x.AddConsumer<UserStatusChangedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"];
        var username = builder.Configuration["RabbitMQ:Username"];
        var password = builder.Configuration["RabbitMQ:Password"];
        var virtualHost = builder.Configuration["RabbitMQ:VirtualHost"];

        // Se o virtualhost estiver configurado, incluir na string de conexão
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
            // Caso contrário, usar o virtualhost padrão
            cfg.Host(host, h =>
            {
                h.Username(username);
                h.Password(password);
            });
        }

        // Configurar as filas para os consumidores
        cfg.ConfigureEndpoints(context);

        // Configurar retry policy
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
    });
});

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RunMate Engagement Service", Version = "v1" });

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: UseAuthentication deve vir antes de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Executar migrações do banco de dados
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RunMateEngagementDbContext>();
    dbContext.Database.Migrate();
}

app.Run();