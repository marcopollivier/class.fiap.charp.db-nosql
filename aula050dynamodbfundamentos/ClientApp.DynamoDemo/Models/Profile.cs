using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.DynamoDemo.Models;

/// <summary>
/// EDUCATIONAL-CONTEXT: Modelo Profile para DynamoDB
///
/// Representa o perfil detalhado de um cliente usando single-table design.
/// PK = CLIENTE#{ClienteId}, SK = PROFILE
///
/// PRACTICAL-APPLICATION: Separar dados básicos do cliente (nome, email)
/// dos dados de perfil (telefone, preferências) permite otimização de consultas.
/// </summary>
public class Profile : DynamoEntityBase
{
    /// <summary>
    /// ID do cliente ao qual este perfil pertence
    /// </summary>
    [DynamoDBProperty("cliente_id")]
    [Required]
    public int ClienteId { get; set; }

    /// <summary>
    /// Número de telefone do cliente
    /// </summary>
    [DynamoDBProperty("telefone")]
    [Phone(ErrorMessage = "Formato de telefone inválido")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Data de nascimento do cliente
    /// </summary>
    [DynamoDBProperty("data_nascimento")]
    public DateTime? DataNascimento { get; set; }

    /// <summary>
    /// Preferências de comunicação do cliente
    /// </summary>
    [DynamoDBProperty("preferencias")]
    public List<string> Preferencias { get; set; } = new();

    /// <summary>
    /// Documento (CPF/CNPJ)
    /// </summary>
    [DynamoDBProperty("documento")]
    [StringLength(20)]
    public string? Documento { get; set; }

    /// <summary>
    /// Gênero do cliente
    /// </summary>
    [DynamoDBProperty("genero")]
    public string? Genero { get; set; }

    /// <summary>
    /// Estado civil
    /// </summary>
    [DynamoDBProperty("estado_civil")]
    public string? EstadoCivil { get; set; }

    /// <summary>
    /// Profissão
    /// </summary>
    [DynamoDBProperty("profissao")]
    [StringLength(100)]
    public string? Profissao { get; set; }

    /// <summary>
    /// Data do último login
    /// </summary>
    [DynamoDBProperty("ultimo_login")]
    public DateTime? UltimoLogin { get; set; }

    /// <summary>
    /// Se o cliente aceitou marketing
    /// </summary>
    [DynamoDBProperty("aceita_marketing")]
    public bool AceitaMarketing { get; set; } = false;

    /// <summary>
    /// Idade calculada baseada na data de nascimento
    /// </summary>
    [DynamoDBIgnore]
    public int? Idade
    {
        get
        {
            if (!DataNascimento.HasValue) return null;
            var hoje = DateTime.Today;
            var idade = hoje.Year - DataNascimento.Value.Year;
            if (DataNascimento.Value.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }

    /// <summary>
    /// Construtor padrão para DynamoDB
    /// </summary>
    public Profile()
    {
        Tipo = "PROFILE";
    }

    /// <summary>
    /// Construtor com parâmetros básicos
    /// </summary>
    public Profile(int clienteId, string? telefone = null, DateTime? dataNascimento = null) : this()
    {
        ClienteId = clienteId;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        SetKeys();
    }

    /// <summary>
    /// CONCEPT-EXPLANATION: Configura as chaves para o perfil
    /// PK = CLIENTE#{ClienteId}, SK = PROFILE
    /// </summary>
    public void SetKeys()
    {
        PK = $"CLIENTE#{ClienteId}";
        SK = "PROFILE";
    }

    /// <summary>
    /// Adiciona uma preferência de comunicação
    /// </summary>
    public void AdicionarPreferencia(string preferencia)
    {
        if (!string.IsNullOrWhiteSpace(preferencia) && !Preferencias.Contains(preferencia))
        {
            Preferencias.Add(preferencia);
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Remove uma preferência de comunicação
    /// </summary>
    public void RemoverPreferencia(string preferencia)
    {
        if (Preferencias.Remove(preferencia))
        {
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Registra um novo login
    /// </summary>
    public void RegistrarLogin()
    {
        UltimoLogin = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkAsUpdated()
    {
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Validação customizada do perfil
    /// </summary>
    public bool IsValid(out List<string> errors)
    {
        errors = new List<string>();

        if (ClienteId <= 0)
            errors.Add("ClienteId deve ser maior que zero");

        if (DataNascimento.HasValue && DataNascimento.Value > DateTime.Today)
            errors.Add("Data de nascimento não pode ser no futuro");

        if (DataNascimento.HasValue && DateTime.Today.Year - DataNascimento.Value.Year > 120)
            errors.Add("Idade não pode ser maior que 120 anos");

        return errors.Count == 0;
    }

    public override string ToString()
    {
        var detalhes = new List<string>();

        if (!string.IsNullOrEmpty(Telefone))
            detalhes.Add($"Tel: {Telefone}");

        if (Idade.HasValue)
            detalhes.Add($"Idade: {Idade}");

        if (Preferencias.Any())
            detalhes.Add($"Prefs: {string.Join(", ", Preferencias)}");

        return $"Profile Cliente {ClienteId}: {string.Join(" | ", detalhes)}";
    }
}
