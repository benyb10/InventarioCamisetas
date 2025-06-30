using Microsoft.EntityFrameworkCore; // Asegúrate de tener este using
using XWY.WebAPI.DataAccess.Context;
using XWY.WebAPI.Extensions;
using XWY.WebAPI.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios usando las extensiones
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddBusinessServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddValidationServices();
builder.Services.AddCustomFilters();
builder.Services.AddCorsPolicy();
builder.Services.AddAutoMapperService();
builder.Services.AddControllerServices();
builder.Services.AddBasicHealthChecks();
builder.Services.AddRateLimiting();
builder.Services.AddLoggingServices();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "XWY Inventario API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

// Middlewares en orden correcto
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
// COMENTADO TEMPORALMENTE: app.UseMiddleware<AuditMiddleware>();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Inicializar base de datos y verificar conexión
if (!app.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<XWYDbContext>();
        try
        {
            // ---> CÓDIGO DE DIAGNÓSTICO <---
            var connectionString = context.Database.GetConnectionString();
            Console.WriteLine(); // Línea en blanco para separar
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("--- LA APLICACIÓN SE ESTÁ CONECTANDO A ESTA BASE DE DATOS ---");
            Console.WriteLine(connectionString);
            Console.WriteLine("********************************************************************************");
            Console.WriteLine(); // Línea en blanco para separar

            context.Database.EnsureCreated();
            Console.WriteLine("Base de datos verificada/creada exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
        }
    }
}

app.Run();

public partial class Program { }