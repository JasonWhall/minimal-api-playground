using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using static ResultMapper;

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

await using WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage()
       .UseSwagger()
       .UseSwaggerUI();
}

var basePath = "/api/TodoItems";

// Get All Todos
app.MapGet(basePath, async ([FromServices] TodoContext _context) => await _context.TodoItems.ToListAsync());

// Get single Todo
app.MapGet($"{basePath}/{{id}}", async ([FromServices] TodoContext _context, long id) =>
    await _context.TodoItems.FindAsync(id) is TodoItem todoItem ? Ok(todoItem) : NotFound());

// Update Todo
app.MapPut($"{basePath}/{{id}}", async ([FromServices] TodoContext _context, long id, TodoItem todoItem) => {
    if (id != todoItem.Id)
        return BadRequest();

    _context.Entry(todoItem).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    return NoContent();
});

// Create Todo
app.MapPost(basePath, async ([FromServices] TodoContext _context, TodoItem todoItem) => {
    await _context.TodoItems.AddAsync(todoItem);
    await _context.SaveChangesAsync();

    return Created();
});

// Delete Todo
app.MapDelete($"{basePath}/{{id}}", async ([FromServices] TodoContext _context, long id) => {
    if (await _context.TodoItems.FindAsync(id) is TodoItem todoItem) {
        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    return NotFound();
});

await app.RunAsync();

public record TodoItem(long Id, [Required] string Name, bool IsComplete);

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}

