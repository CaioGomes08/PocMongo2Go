using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using UserApi.Models;
using UserApi.Service;

var builder = WebApplication.CreateBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.AddSingleton<UserService>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Rotas da API
app.MapGet("/users", async (UserService userService) =>
	Results.Ok(await userService.GetAllAsync()));

app.MapGet("/users/{id:guid}", async (UserService userService, Guid id) =>
{
	var user = await userService.GetByIdAsync(id);
	return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", async (UserService userService, User user) =>
{
	await userService.CreateAsync(user);
	return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id:guid}", async (UserService userService, Guid id, User updatedUser) =>
{
	var user = await userService.GetByIdAsync(id);
	if (user is null) return Results.NotFound();

	updatedUser.Id = id;
	await userService.UpdateAsync(id, updatedUser);
	return Results.NoContent();
});

app.MapDelete("/users/{id:guid}", async (UserService userService, Guid id) =>
{
	var user = await userService.GetByIdAsync(id);
	if (user is null) return Results.NotFound();

	await userService.DeleteAsync(id);
	return Results.NoContent();
});


app.Run();
