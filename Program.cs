using Microsoft.EntityFrameworkCore;
using MinimalAPIProject.Data;
using MinimalAPIProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<UserModel>> GetUsers(AppDbContext context)
{
    return await context.Users.ToListAsync();
}

app.MapGet("/Users", async (AppDbContext context) =>
{
    return await GetUsers(context);

});

app.MapGet("/User/{id}", async (AppDbContext context, int id) => 
{
    var user = await context.Users.FindAsync(id);

    if( user == null)
    {
        return Results.NotFound("Usuário não encontrado");
    }

    return Results.Ok(user);

});
    

app.MapPost("/User", async(AppDbContext context, UserModel user) =>
{
    context.Users.Add(user);
    await context.SaveChangesAsync();

    return await GetUsers(context);

});


app.MapPut("/User", async (AppDbContext context, UserModel user) =>
{
    var userDb = await context.Users.AsNoTracking().FirstOrDefaultAsync(userDb => userDb.Id == userDb.Id);

    if (userDb == null) return Results.NotFound("Usuário não localizado!");

    userDb.UserName = user.UserName;
    userDb.Email = user.Email;
    userDb.Name = user.Name;

    context.Update(user);
    await context.SaveChangesAsync();

    return Results.Ok(await GetUsers(context));

});


app.MapDelete("/User/{id}", async(AppDbContext context, int id) =>
{
    var userDb = await context.Users.FindAsync(id);
    if(userDb == null) return Results.NotFound("Usuário não localizado!");

    context.Users.Remove(userDb);
    await context.SaveChangesAsync();   

    return Results.Ok(await GetUsers (context));

});



app.Run();

