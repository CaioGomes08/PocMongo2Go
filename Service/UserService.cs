using MongoDB.Driver;
using UserApi.Models;

namespace UserApi.Service
{
	public class UserService
	{
		private readonly IMongoCollection<User> _usersCollection;

		public UserService(IConfiguration configuration)
		{
			var mongoClient = new MongoClient(configuration["MongoDB:ConnectionString"]);
			var database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"]);
			_usersCollection = database.GetCollection<User>(configuration["MongoDB:CollectionName"]);
		}

		public async Task<List<User>> GetAllAsync() =>
		await _usersCollection.Find(_ => true).ToListAsync();

		public async Task<User?> GetByIdAsync(Guid id) =>
			await _usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();

		public async Task CreateAsync(User user) =>
			await _usersCollection.InsertOneAsync(user);

		public async Task UpdateAsync(Guid id, User updatedUser)
		{
			var existingUser = await GetByIdAsync(id);
			if (existingUser is null)
				throw new Exception("User not found");

			// Preservar o campo Id do usuário original
			updatedUser.Id = id;

			// Realizar a atualização
			await _usersCollection.ReplaceOneAsync(user => user.Id == id, updatedUser);
		}

		public async Task DeleteAsync(Guid id) =>
			await _usersCollection.DeleteOneAsync(user => user.Id == id);
	}
}
