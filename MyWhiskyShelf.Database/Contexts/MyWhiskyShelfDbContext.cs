using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Models;

namespace MyWhiskyShelf.Database.Contexts;

public class MyWhiskyShelfDbContext(DbContextOptions<MyWhiskyShelfDbContext> options) : DbContext(options)
{
    internal DbSet<DistilleryEntity> Distilleries { get; set; }

   
}