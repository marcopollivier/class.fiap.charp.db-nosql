namespace PedidosApi.Models;

public record ClienteDto(string Nome, string Email);

public record PedidoDto(string ClienteId, int SqlClienteId, List<ItemDto> Itens);

public record ItemDto(string Nome, decimal Preco, int Quantidade);
