namespace VendasAPI.Tests;

using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VendasAPI.Domain.Entities;
using VendasAPI.API.Controllers;
using VendasAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class VendaTests
{
    private readonly VendasController _controller;
    private readonly VendasContext _context;

    public VendaTests()
    {
        int idCancela = 0;

        // Configuração do contexto SQLite em memória para os testes
        var options = new DbContextOptionsBuilder<VendasContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new VendasContext(options);

        // Criar o banco de dados em memória
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        // Inicializando o controller com o contexto in-memory
        _controller = new VendasController(_context);
    }

    [Fact]
    public void Deve_Criar_Uma_Venda()
    {
        // Arrange
        var venda = new Venda
        {
            ClienteId = Guid.NewGuid(),
            DataVenda = DateTime.Now,
            ValorTotal = 100,
            Cancelado = false,
            Itens = new List<ItemVenda>
            {
                new ItemVenda { ProdutoId = Guid.NewGuid(), Quantidade = 1, ValorUnitario = 100 , Cancelado = false}
            }
        };

        // Act
        var result = _controller.CreateVenda(venda);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();

        var vendas = _context.Vendas.ToList();
        vendas.Should().ContainSingle(v => v.ClienteId == venda.ClienteId);        
    }

    [Fact]
    public void Deve_Cancelar_Uma_Venda()
    {
        // Arrange
        var venda = new Venda
        {
            ClienteId = Guid.NewGuid(),
            DataVenda = DateTime.Now,
            ValorTotal = 100,
            Cancelado = false,
            Itens = new List<ItemVenda>
            {
                new ItemVenda { ProdutoId = Guid.NewGuid(), Quantidade = 1, ValorUnitario = 100, Cancelado = false }
            }
        };

        _context.Vendas.Add(venda);
        _context.SaveChanges();

        // Act
        var result = _controller.CancelVenda(venda.Id);

        // Assert
        result.Should().BeOfType<OkResult>();

        var vendaCancelada = _context.Vendas.First(v => v.Id == venda.Id);
        vendaCancelada.Cancelado.Should().BeTrue();
    }
}
