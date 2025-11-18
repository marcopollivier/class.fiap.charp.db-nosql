using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.DynamoDemo.Models;

/// <summary>
/// EDUCATIONAL-CONTEXT: Modelo Endereco para DynamoDB
///
/// Representa endereços de um cliente usando single-table design.
/// PK = CLIENTE#{ClienteId}, SK = ENDERECO#{TipoEndereco}
///
/// CONCEPT-EXPLANATION: Multiple Records per Entity Pattern
/// Um cliente pode ter múltiplos endereços (HOME, WORK, DELIVERY, etc.)
/// cada um com sua própria entrada na tabela.
/// </summary>
public class Endereco : DynamoEntityBase
{
    /// <summary>
    /// ID do cliente ao qual este endereço pertence
    /// </summary>
    [DynamoDBProperty("cliente_id")]
    [Required]
    public int ClienteId { get; set; }

    /// <summary>
    /// Tipo do endereço (HOME, WORK, DELIVERY, etc.)
    /// </summary>
    [DynamoDBProperty("tipo_endereco")]
    [Required]
    public TipoEndereco TipoEndereco { get; set; }

    /// <summary>
    /// Nome/descrição do endereço
    /// </summary>
    [DynamoDBProperty("nome")]
    [StringLength(50)]
    public string? Nome { get; set; }

    /// <summary>
    /// Logradouro (rua, avenida, etc.)
    /// </summary>
    [DynamoDBProperty("logradouro")]
    [Required]
    [StringLength(200)]
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>
    /// Número do endereço
    /// </summary>
    [DynamoDBProperty("numero")]
    [Required]
    [StringLength(20)]
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Complemento (apartamento, bloco, etc.)
    /// </summary>
    [DynamoDBProperty("complemento")]
    [StringLength(100)]
    public string? Complemento { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    [DynamoDBProperty("bairro")]
    [Required]
    [StringLength(100)]
    public string Bairro { get; set; } = string.Empty;

    /// <summary>
    /// Cidade
    /// </summary>
    [DynamoDBProperty("cidade")]
    [Required]
    [StringLength(100)]
    public string Cidade { get; set; } = string.Empty;

    /// <summary>
    /// Estado (UF)
    /// </summary>
    [DynamoDBProperty("estado")]
    [Required]
    [StringLength(2)]
    [RegularExpression("^[A-Z]{2}$", ErrorMessage = "Estado deve ter 2 letras maiúsculas")]
    public string Estado { get; set; } = string.Empty;

    /// <summary>
    /// CEP
    /// </summary>
    [DynamoDBProperty("cep")]
    [Required]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve ter formato 99999-999")]
    public string CEP { get; set; } = string.Empty;

    /// <summary>
    /// País
    /// </summary>
    [DynamoDBProperty("pais")]
    [StringLength(50)]
    public string Pais { get; set; } = "Brasil";

    /// <summary>
    /// Se este é o endereço principal
    /// </summary>
    [DynamoDBProperty("principal")]
    public bool Principal { get; set; } = false;

    /// <summary>
    /// Observações sobre o endereço
    /// </summary>
    [DynamoDBProperty("observacoes")]
    [StringLength(200)]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Coordenadas geográficas - Latitude
    /// </summary>
    [DynamoDBProperty("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Coordenadas geográficas - Longitude
    /// </summary>
    [DynamoDBProperty("longitude")]
    public double? Longitude { get; set; }

    /// <summary>
    /// Endereço completo formatado
    /// </summary>
    [DynamoDBIgnore]
    public string EnderecoCompleto
    {
        get
        {
            var endereco = $"{Logradouro}, {Numero}";
            if (!string.IsNullOrEmpty(Complemento))
                endereco += $", {Complemento}";
            endereco += $", {Bairro}, {Cidade}/{Estado}, {CEP}";
            return endereco;
        }
    }

    /// <summary>
    /// Construtor padrão para DynamoDB
    /// </summary>
    public Endereco()
    {
        Tipo = "ENDERECO";
    }

    /// <summary>
    /// Construtor com parâmetros básicos
    /// </summary>
    public Endereco(int clienteId, TipoEndereco tipoEndereco, string logradouro, string numero,
                   string bairro, string cidade, string estado, string cep) : this()
    {
        ClienteId = clienteId;
        TipoEndereco = tipoEndereco;
        Logradouro = logradouro;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado.ToUpper();
        CEP = cep;
        SetKeys();
    }

    /// <summary>
    /// CONCEPT-EXPLANATION: Configura as chaves para o endereço
    /// PK = CLIENTE#{ClienteId}, SK = ENDERECO#{TipoEndereco}
    /// </summary>
    public void SetKeys()
    {
        PK = $"CLIENTE#{ClienteId}";
        SK = $"ENDERECO#{TipoEndereco}";
    }

    /// <summary>
    /// Define coordenadas geográficas
    /// </summary>
    public void DefinirCoordenadas(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        MarkAsUpdated();
    }

    /// <summary>
    /// Marca como endereço principal
    /// </summary>
    public void MarcarComoPrincipal()
    {
        Principal = true;
        MarkAsUpdated();
    }

    public void MarkAsUpdated()
    {
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Validação customizada do endereço
    /// </summary>
    public bool IsValid(out List<string> errors)
    {
        errors = new List<string>();

        if (ClienteId <= 0)
            errors.Add("ClienteId deve ser maior que zero");

        if (string.IsNullOrWhiteSpace(Logradouro))
            errors.Add("Logradouro é obrigatório");

        if (string.IsNullOrWhiteSpace(Numero))
            errors.Add("Número é obrigatório");

        if (string.IsNullOrWhiteSpace(Cidade))
            errors.Add("Cidade é obrigatória");

        if (string.IsNullOrWhiteSpace(Estado) || Estado.Length != 2)
            errors.Add("Estado deve ter exatamente 2 caracteres");

        if (string.IsNullOrWhiteSpace(CEP))
            errors.Add("CEP é obrigatório");

        return errors.Count == 0;
    }

    public override string ToString()
    {
        return $"Endereço {TipoEndereco} - Cliente {ClienteId}: {EnderecoCompleto}";
    }
}

/// <summary>
/// Enum para tipos de endereço
/// </summary>
public enum TipoEndereco
{
    HOME,       // Residencial
    WORK,       // Comercial/Trabalho
    DELIVERY,   // Entrega
    BILLING,    // Cobrança
    OTHER       // Outro
}
