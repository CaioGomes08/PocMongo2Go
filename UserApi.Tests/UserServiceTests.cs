using Mongo2Go;
using UserApi.Models;
using FluentAssertions;
using UserApi.Service;
using Microsoft.Extensions.Configuration;

namespace UserApi.Tests;

public class UserServiceTests : IDisposable
{
	private readonly MongoDbRunner _mongoRunner;
	private readonly UserService _userService;

	public UserServiceTests()
	{
		// Inicia o MongoDB in-memory
		_mongoRunner = MongoDbRunner.Start();

		// Configura o serviço com o MongoDB in-memory
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string>
			{
				{ "MongoDB:ConnectionString", _mongoRunner.ConnectionString },
				{ "MongoDB:DatabaseName", "TestDatabase" },
				{ "MongoDB:CollectionName", "Users" }
			})
			.Build();

		_userService = new UserService(configuration);
	}

	[Fact]
	public async Task CreateAsync_ShouldAddUserToDatabase()
	{
		// Arrange
		var user = new User
		{
			Name = "John Doe",
			Email = "johndoe@example.com",
			Age = 30
		};

		// Act
		await _userService.CreateAsync(user);
		var users = await _userService.GetAllAsync();

		// Assert
		users.Should().ContainSingle();
		users[0].Name.Should().Be("John Doe");
		users[0].Email.Should().Be("johndoe@example.com");
	}

	[Fact]
	public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
	{
		// Arrange
		var user = new User
		{
			Name = "Jane Doe",
			Email = "janedoe@example.com",
			Age = 25
		};

		await _userService.CreateAsync(user);

		// Act
		var retrievedUser = await _userService.GetByIdAsync(user.Id);

		// Assert
		retrievedUser.Should().NotBeNull();
		retrievedUser!.Name.Should().Be("Jane Doe");
	}

	[Fact]
	public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
	{
		// Act
		var user = await _userService.GetByIdAsync(Guid.NewGuid());

		// Assert
		user.Should().BeNull();
	}

	[Fact]
	public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
	{
		// Arrange
		var user = new User
		{
			Name = "Old Name",
			Email = "oldemail@example.com",
			Age = 20
		};

		await _userService.CreateAsync(user);

		var updatedUser = new User
		{
			Name = "New Name",
			Email = "newemail@example.com",
			Age = 21
		};

		// Act
		await _userService.UpdateAsync(user.Id, updatedUser);
		var result = await _userService.GetByIdAsync(user.Id);

		// Assert
		result.Should().NotBeNull();
		result!.Name.Should().Be("New Name");
		result.Email.Should().Be("newemail@example.com");
	}

	[Fact]
	public async Task DeleteAsync_ShouldRemoveUserFromDatabase()
	{
		// Arrange
		var user = new User
		{
			Name = "To Be Deleted",
			Email = "delete@example.com",
			Age = 40
		};

		await _userService.CreateAsync(user);

		// Act
		await _userService.DeleteAsync(user.Id);
		var users = await _userService.GetAllAsync();

		// Assert
		users.Should().BeEmpty();
	}

	public void Dispose()
	{
		// Finaliza o MongoDB in-memory
		_mongoRunner.Dispose();
	}
}
