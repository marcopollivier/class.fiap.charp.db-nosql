using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PedidosApiSimples;

public class Cliente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("primeiraLetra")]
    public string PrimeiraLetra { get; set; } = string.Empty;

    // Propriedade calculada para determinar a primeira letra
    public void CalcularPrimeiraLetra()
    {
        PrimeiraLetra = string.IsNullOrEmpty(Nome) ? "A" : Nome[0].ToString().ToUpper();
    }

    public override string ToString()
    {
        return $"Id: {Id}, Nome: {Nome}, Email: {Email}, Shard: {PrimeiraLetra}";
    }
}