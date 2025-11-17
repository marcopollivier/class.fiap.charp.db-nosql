using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLakeProcessor.Models;

/// <summary>
/// Modelo para eventos processados no Data Lake
/// Estrutura denormalizada para análises
/// </summary>
public class EventoDataLake
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("evento")]
    public string Evento { get; set; } = string.Empty;
    
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }
    
    [BsonElement("operacao")]
    public string Operacao { get; set; } = string.Empty; // insert, update, delete
    
    [BsonElement("colecao")]
    public string Colecao { get; set; } = string.Empty; // pedidos, clientes, itens
    
    [BsonElement("documentId")]
    public ObjectId DocumentId { get; set; }
    
    [BsonElement("pedido")]
    public PedidoDenormalizado? Pedido { get; set; }
    
    [BsonElement("metadados")]
    public MetadadosEvento Metadados { get; set; } = new();
}

/// <summary>
/// Pedido denormalizado para o Data Lake
/// Inclui dados do cliente e itens completos
/// </summary>
public class PedidoDenormalizado
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }
    
    [BsonElement("total")]
    public decimal Total { get; set; }
    
    [BsonElement("dataPedido")]
    public DateTime DataPedido { get; set; }
    
    [BsonElement("cliente")]
    public ClienteResumido Cliente { get; set; } = new();
    
    [BsonElement("itens")]
    public List<ItemPedido> Itens { get; set; } = new();
    
    [BsonElement("quantidadeItens")]
    public int QuantidadeItens => Itens?.Sum(i => i.Quantidade) ?? 0;
    
    [BsonElement("valorMedio")]
    public decimal ValorMedio => Itens?.Count > 0 ? Total / Itens.Count : 0;
}

/// <summary>
/// Cliente resumido para inclusão em eventos
/// </summary>
public class ClienteResumido
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }
    
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Metadados do evento para auditoria e versionamento
/// </summary>
public class MetadadosEvento
{
    [BsonElement("origem")]
    public string Origem { get; set; } = "change_stream";
    
    [BsonElement("versao")]
    public string Versao { get; set; } = "1.0";
    
    [BsonElement("processadoEm")]
    public DateTime ProcessadoEm { get; set; } = DateTime.UtcNow;
    
    [BsonElement("hostname")]
    public string Hostname { get; set; } = Environment.MachineName;
}