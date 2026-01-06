using Microsoft.AspNetCore.Mvc;
using PedidosApi.Models;
using PedidosApi.Services;

namespace PedidosApi.Controllers;

[ApiController]
[Route("api")]
public class PedidosController : ControllerBase
{
    private readonly PedidosService _pedidosService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(PedidosService pedidosService, ILogger<PedidosController> logger)
    {
        _pedidosService = pedidosService;
        _logger = logger;
    }

    // Endpoints de Cliente
    [HttpPost("clientes")]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteDto clienteDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(clienteDto.Nome) || string.IsNullOrWhiteSpace(clienteDto.Email))
            {
                return BadRequest("Nome e Email são obrigatórios");
            }

            var id = await _pedidosService.CriarClienteAsync(clienteDto);
            return CreatedAtAction(nameof(BuscarCliente), new { id }, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("clientes/{id}")]
    public async Task<IActionResult> BuscarCliente(string id)
    {
        try
        {
            var cliente = await _pedidosService.BuscarClienteAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> ListarClientes()
    {
        try
        {
            var clientes = await _pedidosService.ListarClientesAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar clientes");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Endpoints de Pedido
    [HttpPost("pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoDto pedidoDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(pedidoDto.ClienteId))
            {
                return BadRequest("ClienteId é obrigatório");
            }

            if (!pedidoDto.Itens.Any())
            {
                return BadRequest("Pedido deve ter pelo menos um item");
            }

            var id = await _pedidosService.CriarPedidoAsync(pedidoDto);
            return CreatedAtAction(nameof(BuscarPedido), new { id }, new { Id = id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("pedidos/{id}")]
    public async Task<IActionResult> BuscarPedido(string id)
    {
        try
        {
            var pedido = await _pedidosService.BuscarPedidoAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("pedidos")]
    public async Task<IActionResult> ListarPedidos()
    {
        try
        {
            var pedidos = await _pedidosService.ListarPedidosAsync();
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("clientes/{clienteId}/pedidos")]
    public async Task<IActionResult> ListarPedidosPorCliente(string clienteId)
    {
        try
        {
            var pedidos = await _pedidosService.ListarPedidosPorClienteAsync(clienteId);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos do cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Endpoint informativo
    [HttpGet("info")]
    public IActionResult ObterInformacoes()
    {
        return Ok(new
        {
            Titulo = "MongoDB Aplicado - Aula 021",
            Descricao = "API demonstrativa usando MongoDB para operações CRUD",
            TecnologiasUtilizadas = new[]
            {
                ".NET 9",
                "MongoDB Driver 3.5.0",
                "ASP.NET Core Web API",
                "Swagger/OpenAPI"
            },
            MongoDB = new
            {
                Tipo = "Banco de Documentos (NoSQL)",
                Schema = "Flexível - documentos BSON",
                Relacionamentos = "Documentos embarcados e referências",
                Escalabilidade = "Horizontal",
                Consultas = "Query API e Aggregation Framework",
                Vantagens = new[]
                {
                    "Schema flexível e evolutivo",
                    "Documentos embarcados eliminam JOINs",
                    "Escalabilidade horizontal nativa",
                    "Alto desempenho para leituras",
                    "Indexação avançada"
                }
            },
            ModelagemDados = new
            {
                Cliente = "Documento simples com ObjectId",
                Pedido = "Documento com array de itens embarcados",
                Relacionamento = "Referência via ObjectId do cliente"
            },
            ExemplosUso = "E-commerce, catálogos, CMS, APIs REST, aplicações mobile"
        });
    }
}
