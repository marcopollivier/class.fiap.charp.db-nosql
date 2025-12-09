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

    /// <summary>
    /// Cria um cliente em ambos os bancos de dados (SQL Server e MongoDB)
    /// Demonstra as diferentes abordagens de ID e estrutura
    /// </summary>
    /// <param name="clienteDto">Dados do cliente</param>
    /// <returns>IDs gerados em ambos os bancos</returns>
    [HttpPost("clientes")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
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

    /// <summary>
    /// Busca um cliente em ambos os bancos para comparação
    /// Demonstra as diferentes formas de consulta
    /// </summary>
    /// <param name="mongoId">ID do cliente no MongoDB (ObjectId)</param>
    /// <param name="sqlId">ID do cliente no SQL Server (int)</param>
    /// <returns>Cliente de ambos os bancos com comparação</returns>
    [HttpGet("clientes/{mongoId}/{sqlId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
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

    /// <summary>
    /// Cria um pedido em ambos os bancos de dados
    /// Demonstra modelagem relacional vs documento embarcado
    /// </summary>
    /// <param name="pedidoDto">Dados do pedido</param>
    /// <returns>IDs do pedido em ambos os bancos</returns>
    [HttpPost("pedidos")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
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

    /// <summary>
    /// Busca um pedido em ambos os bancos para comparação
    /// Demonstra diferenças na recuperação de dados relacionados
    /// </summary>
    /// <param name="mongoId">ID do pedido no MongoDB (ObjectId)</param>
    /// <param name="sqlId">ID do pedido no SQL Server (int)</param>
    /// <returns>Pedido de ambos os bancos com comparação</returns>
    [HttpGet("pedidos/{mongoId}/{sqlId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
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

    /// <summary>
    /// Endpoint educacional que explica as diferenças entre SQL e NoSQL
    /// </summary>
    [HttpGet("comparacao")]
    [ProducesResponseType(200)]
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
