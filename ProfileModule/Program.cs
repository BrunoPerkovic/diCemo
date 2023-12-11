using Microsoft.EntityFrameworkCore;
using ProfileModule.BL.Intefaces;
using ProfileModule.BL.Services;
using ProfileModule.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Services implementation
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddDbContext<ProfileDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("DićemoDatabase"));
    o.EnableDetailedErrors();
});

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

app.Run();