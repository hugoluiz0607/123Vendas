namespace VendasAPI.Domain.Entities
{
    public class ItemVenda
    {
        public int Id { get; set; }
        // Identificador externo para o produto
        public Guid ProdutoId { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal => ValorUnitario * Quantidade;
        public bool Cancelado { get; set; } // Exclusão lógica
    }
}