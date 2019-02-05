using GestaoProdutos.Exceptions;
using GestaoProdutos.Helpers;
using GestaoProdutos.Model;
using GestaoProdutos.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace GestaoProdutosTest
{
    public class EstoqueServiceTest
    {

        private DbContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Produtos");
            return new Context(optionsBuilder.Options);
        }

        private EstoqueService GetEstoqueService()
        {
            DbContext db = GetDbContext();
            IProdutosService stubProdutosService = new ProdutosService(db);
            IEmpresasService stubEmpresasService = new EmpresasService(db);
            return new EstoqueService(db, stubProdutosService, stubEmpresasService);
        }

        [Fact]
        public void TrataDados_DadosValidos_RetornaEstoque()
        {
            ImportEstoqueData stubEstoque = new ImportEstoqueData
            {
                CodigoProduto = "0",
                Data = new DateTime(2019, 1, 1),
                Empresa = "Empresa Teste",
                Entrada = "1",
                Saida = "2",
                Estoque = "3",
                NomeProduto = "Teste",
                Row = 1
            };
            ImportEstoqueData mockEstoque = new ImportEstoqueData
            {
                CodigoProduto = 0,
                Data = new DateTime(2019, 1, 1),
                Empresa = "Empresa Teste",
                Entrada = 1,
                Saida = 2,
                Estoque = 3,
                NomeProduto = "Teste",
                Row = 1
            };
            EstoqueService estoqueService = GetEstoqueService();

            var actual = estoqueService.TrataDados(stubEstoque);

            Assert.Equal(mockEstoque, actual, new ObjectDeepCompare<ImportEstoqueData>());
        }

        [Fact]
        public void TrataDados_DadosValidos_ThrowFormatException()
        {
            ImportEstoqueData stubEstoque = new ImportEstoqueData
            {
                CodigoProduto = "0",
                Data = new DateTime(2019, 1, 1),
                Empresa = "Empresa Teste",
                Entrada = "xyz",
                Saida = "2",
                Estoque = "3",
                NomeProduto = "Teste",
                Row = 1
            };
            EstoqueService estoqueService = GetEstoqueService();

            Exception ex = Assert.Throws<FormatException>(() => estoqueService.TrataDados(stubEstoque));

            Assert.Equal("A entrada de produtos na linha 1 da planilha não está em formato númerico!", ex.Message);
        }

        [Fact]
        public void TrataDados_DadosValidos_ThrowInformacaoObrigatoriaException()
        {
            ImportEstoqueData stubEstoque = new ImportEstoqueData
            {
                CodigoProduto = "0",
                Data = new DateTime(2019, 1, 1),
                Empresa = "Empresa Teste",
                Entrada = "1",
                Saida = "2",
                Estoque = "3",
                NomeProduto = "",
                Row = 2
            };
            EstoqueService estoqueService = GetEstoqueService();

            Exception ex = Assert.Throws<InformacaoObrigatoriaException>(() => estoqueService.TrataDados(stubEstoque));
            Assert.Equal("Nome do produto não informado na linha 2 da planilha!", ex.Message);
        }

        [Fact]
        public void EstoqueValido_UltimoEstoqueNull_ReturnTrue()
        {
            Estoque stubEstoque = new Estoque();
            EstoqueService estoqueService = GetEstoqueService();

            bool actual = estoqueService.EstoqueValido(stubEstoque, null);

            Assert.True(actual);
        }

        [Theory]
        [InlineData(8, 10, 5, 13)]
        [InlineData(26, 20, 7, 39)]
        [InlineData(900, 100, 0, 1000)]
        public void EstoqueValido_EstoqueValido_ReturnTrue(int estoqueAnterior, int entrada, int saida, int novoEstoque)
        {
            Estoque stubEstoque = new Estoque
            {
                Entrada = entrada,
                Saida = saida,
                Qtda = novoEstoque
            };
            Estoque mockEstoque = new Estoque
            {
                Qtda = estoqueAnterior
            };

            EstoqueService estoqueService = GetEstoqueService();

            bool actual = estoqueService.EstoqueValido(stubEstoque, mockEstoque);

            Assert.True(actual);
        }

        [Theory]
        [InlineData(8, 10, 5, 99)]
        [InlineData(26, 20, 7, 34)]
        [InlineData(900, 100, 0, 3454)]
        public void EstoqueValido_EstoqueInvalido_ReturnFalse(int estoqueAnterior, int entrada, int saida, int novoEstoque)
        {
            Estoque stubEstoque = new Estoque
            {
                Entrada = entrada,
                Saida = saida,
                Qtda = novoEstoque
            };
            Estoque mockEstoque = new Estoque
            {
                Qtda = estoqueAnterior
            };

            EstoqueService estoqueService = GetEstoqueService();

            bool actual = estoqueService.EstoqueValido(stubEstoque, mockEstoque);

            Assert.False(actual);
        }
    }
}
