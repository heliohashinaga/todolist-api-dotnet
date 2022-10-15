using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoListApi.DataContracts;
using TodoListApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/todos", async (TodoDb db) => await db.Todos.ToArrayAsync())
    .Produces<Todo[]>()
    .Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/todos/{id}", async (Guid id, TodoDb db) => 
    await db.Todos.FindAsync(id)
    is Todo todo
    ? Results.Ok(todo)
    : Results.NotFound())
    .Produces<Todo>()
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError)
    .ProducesValidationProblem();

app.MapPost("/todos", async (CreateTodo request, TodoDb db, IMapper mapper) =>
    {
        var todo = mapper.Map<Todo>(request);

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return Results.Created($"/todos/{todo.Id}", todo);
    })
    .Produces<Todo>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status500InternalServerError)
    .ProducesValidationProblem();

app.MapPut("/todos/{id}", async (Guid id, UpdateTodo request, TodoDb db) =>
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return Results.NotFound();

        todo.Description = request.Description;
        todo.IsComplete = request.IsComplete;

        await db.SaveChangesAsync();

        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError)
    .ProducesValidationProblem();

app.MapDelete("/todos/{id}", async (Guid id, TodoDb db) =>
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError)
    .ProducesValidationProblem();

app.Run();

internal class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}