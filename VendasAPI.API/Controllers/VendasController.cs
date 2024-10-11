using Microsoft.AspNetCore.Mvc;
using VendasAPI.Domain.Entities;
using VendasAPI.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace VendasAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly VendasContext _context;

        public VendasController(VendasContext context)
        {
            _context = context;
        }

        // Listar todas as vendas (somente não canceladas)
        [HttpGet]
        public IActionResult GetVendas()
        {
            var vendas = _context.Vendas
                .Include(v => v.Itens.Where(i => !i.Cancelado))
                .Where(v => !v.Cancelado).ToList();
            return Ok(vendas);
        }

        // Obter uma venda por ID (somente se não estiver cancelada)
        [HttpGet("{id}")]
        public IActionResult GetVendaById(int id)
        {
            var venda = _context.Vendas
                .Include(v => v.Itens.Where(i => !i.Cancelado))
                .FirstOrDefault(v => v.Id == id && !v.Cancelado);
            if (venda == null)
            {
                return NotFound();
            }
            return Ok(venda);
        }

        // Criar uma nova venda
        [HttpPost]
        public IActionResult CreateVenda([FromBody] Venda venda)
        {
            if (venda == null || venda.Itens == null || !venda.Itens.Any())
            {
                return BadRequest("Venda inválida.");
            }

            venda.ValorTotal = venda.Itens.Where(i => !i.Cancelado).Sum(i => i.ValorTotal);

            _context.Vendas.Add(venda);
            _context.SaveChanges();

            // Log do evento CompraCriada
            Log.Information("CompraCriada: Venda {VendaId} para Cliente {ClienteId}", venda.Id, venda.ClienteId);

            return CreatedAtAction(nameof(GetVendaById), new { id = venda.Id }, venda);
        }

        // Atualizar uma venda existente
        [HttpPut("{id}")]
        public IActionResult UpdateVenda(int id, [FromBody] Venda vendaAtualizada)
        {
            var vendaExistente = _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefault(v => v.Id == id && !v.Cancelado);
            if (vendaExistente == null)
            {
                return NotFound();
            }

            // Atualizar dados da venda existente
            vendaExistente.ClienteId = vendaAtualizada.ClienteId;
            vendaExistente.DataVenda = vendaAtualizada.DataVenda;
            vendaExistente.ValorTotal = vendaAtualizada.ValorTotal;
            vendaExistente.FilialId = vendaAtualizada.FilialId;

            // Atualizar a lista de itens da venda
            foreach (var itemAtualizado in vendaAtualizada.Itens)
            {
                // Verificar se o item já existe
                var itemExistente = vendaExistente.Itens.FirstOrDefault(i => i.Id == itemAtualizado.Id);

                if (itemExistente != null)
                {
                    // Atualizar item existente
                    itemExistente.ProdutoId = itemAtualizado.ProdutoId;
                    itemExistente.Quantidade = itemAtualizado.Quantidade;
                    itemExistente.ValorUnitario = itemAtualizado.ValorUnitario;
                    itemExistente.Desconto = itemAtualizado.Desconto;

                    if (!itemExistente.Cancelado && itemAtualizado.Cancelado)
                    {
                        itemExistente.Cancelado = itemAtualizado.Cancelado;

                        // Log do evento ItemCancelado
                        Log.Information("ItemCancelado: Item {Id} da Venda {VendaId} para Cliente {ClienteId}", itemExistente.Id, vendaExistente.Id, vendaExistente.ClienteId);
                    }                                        
                }
                else
                {
                    // Adicionar novo item
                    vendaExistente.Itens.Add(itemAtualizado);
                }
            }

            vendaExistente.ValorTotal = vendaExistente.Itens.Where(i => !i.Cancelado).Sum(i => i.ValorTotal);

            _context.SaveChanges();

            // Log do evento CompraAlterada
            Log.Information("CompraAlterada: Venda {VendaId} para Cliente {ClienteId}", vendaExistente.Id, vendaExistente.ClienteId);

            return NoContent();
        }

        // Cancelar uma venda (exclusão lógica)
        [HttpDelete("{id}")]
        public IActionResult CancelVenda(int id)
        {
            var vendaExistente = _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefault(v => v.Id == id && !v.Cancelado);
            if (vendaExistente == null)
            {
                return NotFound();
            }

            // Marcar a venda como cancelada
            vendaExistente.Cancelado = true;
            _context.SaveChanges();

            // Log do evento CompraCancelada
            Log.Information("CompraCancelada: Venda {VendaId} para Cliente {ClienteId}", vendaExistente.Id, vendaExistente.ClienteId);

            return Ok();
        }
    }
}
