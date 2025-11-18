using ClientApp.DynamoDemo.Models;

namespace ClientApp.DynamoDemo.Helpers;

/// <summary>
/// EDUCATIONAL-CONTEXT: Helper class para demonstrar validações e utilitários
///
/// Esta classe contém métodos auxiliares para validação de dados,
/// formatação e utilitários comuns do projeto.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Valida se um email tem formato correto
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Valida se um CEP brasileiro tem formato correto
    /// </summary>
    public static bool IsValidCEP(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return false;

        // Remove caracteres não numéricos
        var cleanCep = System.Text.RegularExpressions.Regex.Replace(cep, @"[^\d]", "");

        // CEP deve ter exatamente 8 dígitos
        return cleanCep.Length == 8 && cleanCep.All(char.IsDigit);
    }

    /// <summary>
    /// Formata CEP no padrão brasileiro (99999-999)
    /// </summary>
    public static string FormatCEP(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return string.Empty;

        var cleanCep = System.Text.RegularExpressions.Regex.Replace(cep, @"[^\d]", "");

        if (cleanCep.Length == 8)
            return $"{cleanCep.Substring(0, 5)}-{cleanCep.Substring(5)}";

        return cep;
    }

    /// <summary>
    /// Valida se um telefone brasileiro tem formato correto
    /// </summary>
    public static bool IsValidPhoneBR(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Remove caracteres não numéricos
        var cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

        // Telefone brasileiro: 10 (fixo) ou 11 (celular) dígitos
        return cleanPhone.Length >= 10 && cleanPhone.Length <= 11;
    }

    /// <summary>
    /// Valida se um valor monetário é válido
    /// </summary>
    public static bool IsValidMonetaryValue(decimal value)
    {
        return value >= 0 && value <= decimal.MaxValue;
    }

    /// <summary>
    /// Valida se uma quantidade é válida
    /// </summary>
    public static bool IsValidQuantity(int quantity)
    {
        return quantity > 0 && quantity <= 10000; // Limite arbitrário para exemplo
    }

    /// <summary>
    /// Gera um ID único para pedidos baseado em timestamp
    /// </summary>
    public static string GeneratePedidoId(DateTime? data = null)
    {
        var dateTime = data ?? DateTime.UtcNow;
        return $"{dateTime:yyyy}-{dateTime:MM}{dateTime:dd}{dateTime:HH}{dateTime:mm}";
    }

    /// <summary>
    /// Valida se uma entidade está em estado válido para persistência
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateEntity<T>(T entity) where T : DynamoEntityBase
    {
        var errors = new List<string>();

        if (entity == null)
        {
            errors.Add("Entidade não pode ser nula");
            return (false, errors);
        }

        if (string.IsNullOrWhiteSpace(entity.PK))
            errors.Add("Partition Key (PK) é obrigatória");

        if (string.IsNullOrWhiteSpace(entity.SK))
            errors.Add("Sort Key (SK) é obrigatória");

        if (string.IsNullOrWhiteSpace(entity.Tipo))
            errors.Add("Tipo da entidade é obrigatório");

        // Validações específicas por tipo
        switch (entity)
        {
            case Cliente cliente:
                if (!IsValidEmail(cliente.Email))
                    errors.Add("Email do cliente inválido");

                if (cliente.ClienteId <= 0)
                    errors.Add("ID do cliente deve ser maior que zero");

                if (string.IsNullOrWhiteSpace(cliente.Nome))
                    errors.Add("Nome do cliente é obrigatório");
                break;

            case Profile profile:
                if (!string.IsNullOrWhiteSpace(profile.Telefone) && !IsValidPhoneBR(profile.Telefone))
                    errors.Add("Formato de telefone inválido");

                if (profile.DataNascimento.HasValue && profile.DataNascimento.Value > DateTime.Today)
                    errors.Add("Data de nascimento não pode ser no futuro");
                break;

            case Pedido pedido:
                if (!IsValidMonetaryValue(pedido.ValorTotal))
                    errors.Add("Valor total do pedido inválido");

                if (string.IsNullOrWhiteSpace(pedido.PedidoId))
                    errors.Add("ID do pedido é obrigatório");
                break;

            case Item item:
                if (!IsValidQuantity(item.Quantidade))
                    errors.Add("Quantidade do item inválida");

                if (!IsValidMonetaryValue(item.PrecoUnitario))
                    errors.Add("Preço unitário inválido");

                if (string.IsNullOrWhiteSpace(item.Nome))
                    errors.Add("Nome do item é obrigatório");
                break;

            case Endereco endereco:
                if (!IsValidCEP(endereco.CEP))
                    errors.Add("CEP inválido");

                if (string.IsNullOrWhiteSpace(endereco.Logradouro))
                    errors.Add("Logradouro é obrigatório");

                if (string.IsNullOrWhiteSpace(endereco.Cidade))
                    errors.Add("Cidade é obrigatória");

                if (string.IsNullOrWhiteSpace(endereco.Estado) || endereco.Estado.Length != 2)
                    errors.Add("Estado deve ter exatamente 2 caracteres");
                break;
        }

        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Formata um valor monetário para exibição
    /// </summary>
    public static string FormatCurrency(decimal value)
    {
        return value.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
    }

    /// <summary>
    /// Calcula a idade baseada na data de nascimento
    /// </summary>
    public static int? CalculateAge(DateTime? birthDate)
    {
        if (!birthDate.HasValue) return null;

        var today = DateTime.Today;
        var age = today.Year - birthDate.Value.Year;

        if (birthDate.Value.Date > today.AddYears(-age))
            age--;

        return age >= 0 ? age : null;
    }
}
