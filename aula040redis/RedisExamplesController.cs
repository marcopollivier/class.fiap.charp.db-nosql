using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace PedidosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedisExamplesController : ControllerBase
{
    private readonly IDatabase _redis;
    private readonly ILogger<RedisExamplesController> _logger;

    public RedisExamplesController(IConnectionMultiplexer redis, ILogger<RedisExamplesController> logger)
    {
        _redis = redis.GetDatabase();
        _logger = logger;
    }

    // Exemplo 1: Cache básico com TTL
    [HttpGet("cache-produto/{id}")]
    public async Task<IActionResult> ObterProdutoComCache(int id)
    {
        var chaveCache = $"produto:{id}";

        // Tentar buscar no cache
        var produtoCache = await _redis.StringGetAsync(chaveCache);
        if (produtoCache.HasValue)
        {
            _logger.LogInformation("Cache HIT para produto {Id}", id);
            return Ok(new { source = "cache", data = JsonSerializer.Deserialize<object>(produtoCache.ToString()) });
        }

        _logger.LogInformation("Cache MISS para produto {Id}", id);

        // Simular busca no banco (dados mock)
        var produto = new
        {
            Id = id,
            Nome = $"Produto {id}",
            Preco = 99.99m * id,
            Categoria = "Eletrônicos",
            DataCadastro = DateTime.UtcNow
        };

        // Salvar no cache com TTL de 5 minutos
        var produtoJson = JsonSerializer.Serialize(produto);
        await _redis.StringSetAsync(chaveCache, produtoJson, TimeSpan.FromMinutes(5));

        return Ok(new { source = "database", data = produto });
    }

    // Exemplo 2: Contador de visitas usando INCR
    [HttpPost("incrementar-visitas/{produtoId}")]
    public async Task<IActionResult> IncrementarVisitas(int produtoId)
    {
        var chave = $"produto:{produtoId}:visitas";
        var visitas = await _redis.StringIncrementAsync(chave);

        // Definir expiração se for a primeira visita
        if (visitas == 1)
        {
            await _redis.KeyExpireAsync(chave, TimeSpan.FromDays(30));
        }

        return Ok(new { produtoId, totalVisitas = visitas });
    }

    // Exemplo 3: Hash para dados estruturados
    [HttpPost("usuario")]
    public async Task<IActionResult> SalvarUsuario([FromBody] Usuario usuario)
    {
        var chave = $"usuario:{usuario.Id}";

        await _redis.HashSetAsync(chave, new HashEntry[]
        {
            new("nome", usuario.Nome),
            new("email", usuario.Email),
            new("idade", usuario.Idade),
            new("ultimoLogin", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
        });

        await _redis.KeyExpireAsync(chave, TimeSpan.FromHours(24));

        return Ok("Usuário salvo no Redis");
    }

    [HttpGet("usuario/{id}")]
    public async Task<IActionResult> ObterUsuario(int id)
    {
        var chave = $"usuario:{id}";
        var dadosUsuario = await _redis.HashGetAllAsync(chave);

        if (!dadosUsuario.Any())
        {
            return NotFound("Usuário não encontrado");
        }

        var usuario = new
        {
            Id = id,
            Nome = dadosUsuario.FirstOrDefault(h => h.Name == "nome").Value,
            Email = dadosUsuario.FirstOrDefault(h => h.Name == "email").Value,
            Idade = dadosUsuario.FirstOrDefault(h => h.Name == "idade").Value,
            UltimoLogin = dadosUsuario.FirstOrDefault(h => h.Name == "ultimoLogin").Value
        };

        return Ok(usuario);
    }

    // Exemplo 4: Set para tags
    [HttpPost("produto/{id}/tag")]
    public async Task<IActionResult> AdicionarTag(int id, [FromBody] string tag)
    {
        var chave = $"produto:{id}:tags";
        await _redis.SetAddAsync(chave, tag);

        var totalTags = await _redis.SetLengthAsync(chave);
        return Ok(new { produtoId = id, tag, totalTags });
    }

    [HttpGet("produto/{id}/tags")]
    public async Task<IActionResult> ObterTags(int id)
    {
        var chave = $"produto:{id}:tags";
        var tags = await _redis.SetMembersAsync(chave);

        return Ok(new { produtoId = id, tags = tags.Select(t => t.ToString()) });
    }

    // Exemplo 5: Sorted Set para ranking
    [HttpPost("leaderboard")]
    public async Task<IActionResult> AdicionarPontuacao([FromBody] PontuacaoRequest request)
    {
        await _redis.SortedSetAddAsync("leaderboard:mensal", request.Jogador, request.Pontos);

        var posicao = await _redis.SortedSetRankAsync("leaderboard:mensal", request.Jogador, Order.Descending);

        return Ok(new
        {
            jogador = request.Jogador,
            pontos = request.Pontos,
            posicao = posicao + 1
        });
    }

    [HttpGet("leaderboard/top/{quantidade}")]
    public async Task<IActionResult> ObterTopJogadores(int quantidade = 10)
    {
        var ranking = await _redis.SortedSetRangeByRankWithScoresAsync(
            "leaderboard:mensal",
            0, quantidade - 1,
            Order.Descending);

        var resultado = ranking.Select((entry, index) => new
        {
            Posicao = index + 1,
            Jogador = entry.Element.ToString(),
            Pontos = entry.Score
        });

        return Ok(resultado);
    }

    // Exemplo 6: Rate Limiting simples
    [HttpGet("api-limitada/{userId}")]
    public async Task<IActionResult> ApiComRateLimit(string userId)
    {
        var chave = $"rate_limit:{userId}:{DateTime.UtcNow:yyyyMMddHHmm}"; // Janela de 1 minuto
        var tentativas = await _redis.StringIncrementAsync(chave);

        if (tentativas == 1)
        {
            await _redis.KeyExpireAsync(chave, TimeSpan.FromMinutes(1));
        }

        const int LIMITE = 10;
        if (tentativas > LIMITE)
        {
            Response.Headers["X-RateLimit-Remaining"] = "0";
            return StatusCode(429, "Rate limit exceeded. Máximo 10 requests por minuto.");
        }

        Response.Headers["X-RateLimit-Remaining"] = (LIMITE - tentativas).ToString();

        return Ok(new
        {
            message = "API acessada com sucesso",
            tentativasRestantes = LIMITE - tentativas
        });
    }

    // Exemplo 7: Lista para fila de processamento
    [HttpPost("fila-pedidos")]
    public async Task<IActionResult> AdicionarPedidoFila([FromBody] int pedidoId)
    {
        await _redis.ListLeftPushAsync("fila:pedidos", pedidoId);
        var tamanhofila = await _redis.ListLengthAsync("fila:pedidos");

        return Ok(new { pedidoId, posicaoNaFila = tamanhofila });
    }

    [HttpPost("processar-pedido")]
    public async Task<IActionResult> ProcessarProximoPedido()
    {
        var pedidoId = await _redis.ListRightPopAsync("fila:pedidos");

        if (!pedidoId.HasValue)
        {
            return Ok(new { message = "Nenhum pedido na fila" });
        }

        // Simular processamento
        await Task.Delay(100);

        return Ok(new
        {
            message = "Pedido processado",
            pedidoId = pedidoId.ToString()
        });
    }

    // Exemplo 8: Informações do Redis
    [HttpGet("redis-info")]
    public async Task<IActionResult> ObterInfoRedis()
    {
        try
        {
            var totalChaves = await _redis.ExecuteAsync("DBSIZE");
            var ping = await _redis.PingAsync();

            var infoRelevante = new
            {
                Status = "Conectado",
                TotalChaves = totalChaves,
                PingMs = ping.TotalMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            return Ok(infoRelevante);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao obter informações do Redis", Details = ex.Message });
        }
    }
}

// DTOs para os exemplos
public record Usuario(int Id, string Nome, string Email, int Idade);
public record PontuacaoRequest(string Jogador, double Pontos);
