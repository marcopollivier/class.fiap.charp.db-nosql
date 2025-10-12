using Microsoft.AspNetCore.Mvc;
using PedidosApi.Models.Sql;
using PedidosApi.Repositories;

namespace PedidosApi.Controllers;

[ApiController]
[Route("api/sql")]
public class SqlController : ControllerBase
{
    private readonly SqlRepository _repository;

    public SqlController(SqlRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("clientes")]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteSql cliente)
    {
        var id = await _repository.CriarClienteAsync(cliente);
        return Ok(new { Id = id });
    }

    [HttpGet("clientes/{id}")]
    public async Task<IActionResult> ObterCliente(int id)
    {
        var cliente = await _repository.ObterClienteAsync(id);
        return cliente == null ? NotFound() : Ok(cliente);
    }

    [HttpPost("pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoSql pedido)
    {
        var id = await _repository.CriarPedidoAsync(pedido);
        return Ok(new { Id = id });
    }

    [HttpGet("pedidos/{id}")]
    public async Task<IActionResult> ObterPedido(int id)
    {
        var pedido = await _repository.ObterPedidoAsync(id);
        return pedido == null ? NotFound() : Ok(pedido);
    }
}
