using RestaurantReviewPlatformWithNLP.Application.Mappings;
using RestaurantReviewPlatformWithNLP.Infrastructure.Configurations;
using RestaurantReviewPlatformWithNLP.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

// configure database
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// configure identity
builder.Services.AddIdentityConfiguration();

// configure JWT Authentication 
builder.Services.AddJwtAuthentication(builder.Configuration);

// configure lifetime for services
builder.Services.AddApplicationServices(builder.Configuration);

// configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.SeedDatabase();

app.Run();
