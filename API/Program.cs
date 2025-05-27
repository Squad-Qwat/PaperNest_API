using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service Scope
builder.Services.AddScoped<DocumentBodyService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserWorkspaceService>();
builder.Services.AddScoped<WorkspaceService>();
builder.Services.AddScoped<CitationService>();

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