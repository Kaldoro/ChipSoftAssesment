using System.Globalization;
using System.Reflection;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Repository.Repositiories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IPatientService, PatientService>();
builder.Services.AddSingleton<IPatientRepository, PatientRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
    options.MapType<FileStreamResult>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    options.MapType<CultureInfo>(() => new OpenApiSchema { Type = "string", Format = "string" });
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