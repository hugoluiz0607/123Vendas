namespace VendasAPI.Domain.Entities
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        // Identificador externo para o cliente
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        // Identificador externo para o cliente
        public Guid FilialId { get; set; }
        public bool Cancelado { get; set; } // Exclusão lógica
        public List<ItemVenda> Itens { get; set; }
    }
}