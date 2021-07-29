using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var sqlConnection = builder.Configuration.GetSection("ConnectionStrings")["SqlServer"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<TodoContext>(opts => {
    if (builder.Environment.IsDevelopment() && string.IsNullOrEmpty(sqlConnection))
        opts.UseInMemoryDatabase("TodoList");
    else
        opts.UseSqlServer(sqlConnection);
});

builder.Services.AddSwaggerGen(opts =>
    opts.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" }));

await using var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage()
       .UseSwagger()
       .UseSwaggerUI();
}

app.UseTodoRoutes(app.Configuration["BasePath"] ?? "/api/TodoItems");

await app.RunAsync();