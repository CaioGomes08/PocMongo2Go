using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UserApi.Models
{
	public class User
	{
		[BsonId] // Marca o campo como identificador no MongoDB
		[BsonRepresentation(BsonType.String)]
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public int Age { get; set; }
	}
}
