using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Models;

namespace ToDoGrpc.Data;

public class AppDbCOntext : DbContext
{

	public AppDbCOntext(DbContextOptions<AppDbCOntext> options) : base(options) { }

    public DbSet<ToDoItem> ToDoItems { get; set; }
}
