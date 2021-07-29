using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ResultMapper;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseTodoRoutes(this WebApplication app, string basePath)
        {
            // Get All Todos
            app.MapGet(basePath, async (TodoContext _context) => await _context.TodoItems.ToListAsync());

            // Get single Todo
            app.MapGet($"{basePath}/{{id}}", async (TodoContext _context, long id) =>
                await _context.TodoItems.FindAsync(id) is TodoItem todoItem ? Ok(todoItem) : NotFound());

            // Update Todo
            app.MapPut($"{basePath}/{{id}}", async (TodoContext _context, long id, TodoItem todoItem) => {
                if (id != todoItem.Id)
                    return BadRequest();

                _context.Entry(todoItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            });

            // Create Todo
            app.MapPost(basePath, async (TodoContext _context, TodoItem todoItem) => {
                await _context.TodoItems.AddAsync(todoItem);
                await _context.SaveChangesAsync();

                return Created();
            });

            // Delete Todo
            app.MapDelete($"{basePath}/{{id}}", async (TodoContext _context, long id) => {
                if (await _context.TodoItems.FindAsync(id) is TodoItem todoItem) {
                    _context.TodoItems.Remove(todoItem);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }

                return NotFound();
            });

            return app;
        }
    }
}
