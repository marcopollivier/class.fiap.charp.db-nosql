using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.DynamoDemo.Models;

/// <summary>
/// Modelo Cliente para DynamoDB usando single-table design
/// PK = CLIENTE#{ClienteId}, SK = CLIENTE#{ClienteId}
/// </summary>
public class Cliente : DynamoEntityBase
{
    [DynamoDBProperty("cliente_id")]
    [Required]
    public int ClienteId { get; set; }

    [DynamoDBProperty("nome")]
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [DynamoDBProperty("email")]
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public Cliente()
    {
        Tipo = "CLIENTE";
    }

    public Cliente(int clienteId, string nome, string email) : this()
    {
        ClienteId = clienteId;
        Nome = nome;
        Email = email;
        SetKeys();
    }

    public void SetKeys()
    {
        PK = $"CLIENTE#{ClienteId}";
        SK = $"CLIENTE#{ClienteId}";
    }

    public void MarkAsUpdated()
    {
        AtualizadoEm = DateTime.UtcNow;
    }

    public bool IsValid(out List<string> errors)
    {
        errors = new List<string>();

        if (ClienteId <= 0)
            errors.Add("ClienteId deve ser maior que zero");

        if (string.IsNullOrWhiteSpace(Nome))
            errors.Add("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
            errors.Add("Email deve ser válido");

        return errors.Count == 0;
    }

    public override string ToString()
    {
        return $"Cliente {ClienteId}: {Nome} ({Email})";
    }
}
