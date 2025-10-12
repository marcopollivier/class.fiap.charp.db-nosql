using Microsoft.AspNetCore.Mvc;

using PedidosApi.Models;
using PedidosApi.Repositories;

namespace PedidosApi.Controllers;

[ApiController]
[Route("api/mongo")]
public class MongoController : ControllerBase
{
    private readonly MongoRepository _repository;

    public MongoController(MongoRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("clientes")]
    public async Task<IActionResult> CriarCliente([FromBody] Cliente cliente)
    {
        var id = await _repository.CriarClienteAsync(cliente);
        return Ok(new { Id = id });
    }

    [HttpGet("clientes/{id}")]
    public async Task<IActionResult> ObterCliente(string id)
    {
        var cliente = await _repository.ObterClienteAsync(id);
        return cliente == null ? NotFound() : Ok(cliente);
    }

    [HttpPost("pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] Pedido pedido)
    {
        var id = await _repository.CriarPedidoAsync(pedido);
        return Ok(new { Id = id });
    }

    [HttpGet("pedidos/{id}")]
    public async Task<IActionResult> ObterPedido(string id)
    {
        var pedido = await _repository.ObterPedidoAsync(id);
        return pedido == null ? NotFound() : Ok(pedido);
    }
}
