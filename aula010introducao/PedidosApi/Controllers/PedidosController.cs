using Microsoft.AspNetCore.Mvc;

using PedidosApi.Models;
using PedidosApi.Services;

namespace PedidosApi.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly PedidosService _pedidosService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(PedidosService pedidosService, ILogger<PedidosController> logger)
    {
        _pedidosService = pedidosService;
        _logger = logger;
    }

    [HttpPost("clientes")]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteDto clienteDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(clienteDto.Nome) || string.IsNullOrWhiteSpace(clienteDto.Email))
            {
                return BadRequest("Nome e Email são obrigatórios");
            }

            var resultado = await _pedidosService.CriarClienteAsync(clienteDto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("clientes/{mongoId}/{sqlId:int}")]
    public async Task<IActionResult> BuscarCliente(string mongoId, int sqlId)
    {
        try
        {
            var resultado = await _pedidosService.BuscarClienteAsync(mongoId, sqlId);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("pedidos")]
    public async Task<IActionResult> CriarPedido([FromBody] PedidoDto pedidoDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(pedidoDto.ClienteId) || pedidoDto.SqlClienteId <= 0)
            {
                return BadRequest("ClienteId (MongoDB) e SqlClienteId são obrigatórios");
            }

            if (!pedidoDto.Itens.Any())
            {
                return BadRequest("Pedido deve ter pelo menos um item");
            }

            var resultado = await _pedidosService.CriarPedidoAsync(pedidoDto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("pedidos/{mongoId}/{sqlId:int}")]
    public async Task<IActionResult> BuscarPedido(string mongoId, int sqlId)
    {
        try
        {
            var resultado = await _pedidosService.BuscarPedidoAsync(mongoId, sqlId);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("comparacao")]
    public IActionResult ObterComparacao()
    {
        return Ok(new
        {
            Titulo = "Comparação SQL vs NoSQL - Aula 010",
            SqlServer = new
            {
                Tipo = "Banco Relacional (RDBMS)",
                Schema = "Rígido - definido previamente",
                Relacionamentos = "Chaves estrangeiras e JOINs",
                Transacoes = "ACID completo",
                Escalabilidade = "Vertical (mais poder no servidor)",
                Consultas = "SQL padronizado",
                UsoCaso = "Dados estruturados, relacionamentos complexos"
            },
            MongoDB = new
            {
                Tipo = "Banco de Documentos (NoSQL)",
                Schema = "Flexível - documentos podem variar",
                Relacionamentos = "Documentos embarcados ou referências",
                Transacoes = "ACID em documentos (limitado entre collections)",
                Escalabilidade = "Horizontal (mais servidores)",
                Consultas = "Queries específicas do MongoDB",
                UsoCaso = "Dados semi-estruturados, esquemas evolutivos"
            },
            ExemplosUso = new
            {
                SqlIdeal = "Sistemas financeiros, inventário, CRM tradicional",
                MongoIdeal = "Catálogos de produtos, blogs, APIs, dados IoT"
            }
        });
    }
}
