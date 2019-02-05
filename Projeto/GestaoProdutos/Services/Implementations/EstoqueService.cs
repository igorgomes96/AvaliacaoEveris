using ClosedXML.Excel;
using GestaoProdutos.Exceptions;
using GestaoProdutos.Helpers;
using GestaoProdutos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoProdutos.Services
{

    public class EstoqueService : GenericCrud<Estoque>, IEstoqueService
    {
        private readonly DbContext _db;
        private readonly IProdutosService _produtosService;
        private readonly IEmpresasService _empresasService;

        /// <summary>
        /// Instância é passada para a classe via injeção de dependência
        /// </summary>
        public EstoqueService(DbContext db, IProdutosService produtosService, IEmpresasService empresasService) : base(db)
        {
            _db = db;
            _produtosService = produtosService;
            _empresasService = empresasService;
        }

        public IQueryable<Estoque> Filter(DateTime? data = null, string produto = null, string empresa = null, string ordem = "data", bool desc = false)
        {
            // Monta a Tree-expression
            IQueryable<Estoque> query = base.Query()
                .Include(h => h.Empresa)
                .Include(h => h.Produto);

            if (data.HasValue)
                query = query.Where(h => h.Data.Equals(data.Value));

            if (!string.IsNullOrEmpty(produto))
                query = query.Where(h => h.Produto.Nome.Contains(produto.Trim(), StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrEmpty(empresa))
                query = query.Where(h => h.Empresa.Nome.Contains(empresa, StringComparison.CurrentCultureIgnoreCase));


            switch (ordem.ToLower().Trim())
            {
                case "data":
                    query = query.OrderBy(e => e.Data, desc);
                    break;
                case "empresa":
                    query = query.OrderBy(e => e.Empresa.Nome, desc);
                    break;
                case "produto":
                    query = query.OrderBy(e => e.Produto.Nome, desc);
                    break;
                case "entrada":
                    query = query.OrderBy(e => e.Entrada, desc);
                    break;
                case "saida":
                    query = query.OrderBy(e => e.Saida, desc);
                    break;
                case "estoque":
                    query = query.OrderBy(e => e.Qtda, desc);
                    break;
                default:
                    query = query.OrderBy(e => e.Data, desc);
                    break;
            }


            return query;
        }


        public override Estoque Find(params object[] key)
        {
            return Query(h => h.Id == (int)key[0])
                .Include(e => e.Empresa)
                .Include(e => e.Produto)
                .FirstOrDefault();
        }

        public override Estoque Add(Estoque estoque)
        {
            Estoque ultimoEstoque = GetUltimoEstoque(estoque);
            if (!EstoqueValido(estoque, ultimoEstoque))
                throw new EstoqueInconsistenteException($@"Produto de cód. {estoque.ProdutoId} da empresa {ultimoEstoque.Empresa.Nome} está com estoque inconsistente com o último registro, do dia {ultimoEstoque.Data.ToString("dd/MM/yyyy")}. Por favor, verifique.");
            return base.Add(estoque);
        }

        public override Estoque Update(Estoque estoque, params object[] key)
        {
            Estoque ultimoEstoque = GetUltimoEstoque(estoque);
            if (!EstoqueValido(estoque, ultimoEstoque))
                throw new EstoqueInconsistenteException($@"Produto de cód. {estoque.ProdutoId} da empresa {ultimoEstoque.Empresa.Nome} está com estoque inconsistente com o último registro, do dia {ultimoEstoque.Data.ToString("dd/MM/yyyy")}. Por favor, verifique.");
            return base.Update(estoque, key);
        }

        public void ImportFile(string path)
        {
            var wb = new XLWorkbook(path);
            var ws = wb.Worksheet(1);  // 1ª sheet

            DateTime? data = ws.Cell("b2").Value as DateTime?;

            if (!data.HasValue)
                throw new Exception("A data não foi informada na planilha!");

            // Lê todos os dados da planilha
            List<Estoque> estoqueList = GetDadosPlanilha(ws, data.Value);

            using (var transaction = _db.Database.BeginTransaction())
            {
                // Consolida e salva os produtos (agrupa pelo id do produto e projeta o objeto Produto)
                List<Produto> produtos = estoqueList
                    .GroupBy(key => key.Produto.Id, value => value.Produto, (key, value) => value.First())
                    .ToList();
                foreach (var produto in produtos)
                {
                    if (_produtosService.Exist(produto.Id))
                        _produtosService.Update(produto, produto.Id);
                    else
                        _produtosService.Add(produto);
                }

                // Obtém os ids das empresa
                foreach (var estoque in estoqueList)
                {
                    Empresa empresa = _empresasService
                        .Query(e => e.Nome.Equals(estoque.Empresa.Nome, StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();
                    if (empresa == null)
                    {
                        empresa = _empresasService.Add(estoque.Empresa);
                    }
                    estoque.EmpresaId = empresa.Id;
                }


                foreach (Estoque estoque in estoqueList)
                {

                    Estoque oldEstoque = Query(h => h.Data == estoque.Data && h.EmpresaId == estoque.EmpresaId && h.ProdutoId == estoque.ProdutoId).FirstOrDefault();

                    // Evitar que ef tente criar o produto e a empresa novamente
                    estoque.Produto = null;
                    estoque.Empresa = null;

                    if (oldEstoque == null)
                    {
                        Add(estoque);
                    }
                    else
                    {
                        estoque.Id = oldEstoque.Id;
                        Update(estoque, estoque.Id);
                    }

                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Lê os dados da planilha e valida as informações
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<Estoque> GetDadosPlanilha(IXLWorksheet ws, DateTime data)
        {
            List<ImportEstoqueData> estoqueList = new List<ImportEstoqueData>();
            int row = 5;
            while (!ws.Cell(row, 1).IsEmpty() || !ws.Cell(row, 2).IsEmpty() ||
                !ws.Cell(row, 3).IsEmpty() || !ws.Cell(row, 4).IsEmpty() ||
                !ws.Cell(row, 5).IsEmpty() || !ws.Cell(row, 6).IsEmpty())
            {
                ImportEstoqueData estoque = GetDadosRow(ws.Row(row));
                estoque.Data = data;
                estoqueList.Add(estoque);
                row++;
            }
            return ValidaDadosImportados(estoqueList);
        }

        /// <summary>
        /// Valida os dados da planilha
        /// </summary>
        /// <param name="importEstoqueData"></param>
        /// <returns></returns>
        public List<Estoque> ValidaDadosImportados(List<ImportEstoqueData> importEstoqueData)
        {

            List<Estoque> estoqueList = new List<Estoque>();

            foreach (var dataImport in importEstoqueData)
            {
                Estoque estoque = new Estoque
                {
                    Empresa = new Empresa(),
                    Produto = new Produto(),
                    Data = dataImport.Data
                };
                var newDataImport = TrataDados(dataImport);

                estoque.Data = dataImport.Data;
                estoque.Empresa.Nome = dataImport.Empresa.ToString();
                estoque.Entrada = (int)dataImport.Entrada;
                estoque.ProdutoId = estoque.Produto.Id = (int)dataImport.CodigoProduto;
                estoque.Produto.Nome = dataImport.NomeProduto.ToString();
                estoque.Qtda = (int)dataImport.Estoque;
                estoque.Saida = (int)dataImport.Saida;

                estoqueList.Add(estoque);

            }

            // Valida os nomes do produtos
            var produtos = estoqueList
                .GroupBy(k => k.Produto.Id, v => v.Produto.Nome, (key, value) => new { produtoId = key, nomes = value.ToList() });
            // Verifica se existem produtos com o mesmo Id mas com nome diferente
            var invalido = produtos.FirstOrDefault(p => p.nomes.Distinct().Count() > 1);
            if (invalido != null)
            {
                throw new Exception($"O produto de código {invalido.produtoId} consta na planilha com {invalido.nomes.Distinct().Count()} nomes diferentes!");
            }

            return estoqueList;
        }

        public ImportEstoqueData TrataDados(ImportEstoqueData dataImport)
        {
            bool parsed = false;
            int row = dataImport.Row;

            // Nome da Empresa
            dataImport.Empresa = dataImport.Empresa?.ToString() ?? "";
            if (string.IsNullOrEmpty(dataImport.Empresa.ToString()))
                throw new InformacaoObrigatoriaException($"Nome da empresa não informado na linha {row} da planilha!");

            // Código do produto
            if (string.IsNullOrEmpty(dataImport.CodigoProduto?.ToString()))
                throw new InformacaoObrigatoriaException($"Código do produto não informado na linha {row} da planilha!");

            parsed = int.TryParse(dataImport.CodigoProduto.ToString(), out int codigoProduto);
            if (!parsed)
                throw new FormatException($"O código do produto na linha {row} da planilha não está em formato númerico!");
            dataImport.CodigoProduto = codigoProduto;


            // Nome do Produto
            dataImport.NomeProduto = dataImport.NomeProduto?.ToString() ?? "";
            if (string.IsNullOrEmpty(dataImport.NomeProduto.ToString()))
                throw new InformacaoObrigatoriaException($"Nome do produto não informado na linha {row} da planilha!");

            // Entrada
            if (string.IsNullOrEmpty(dataImport.Entrada?.ToString()))
            {
                dataImport.Entrada = 0;
            }
            else
            {
                parsed = int.TryParse(dataImport.Entrada.ToString(), out int entrada);
                if (!parsed)
                    throw new FormatException($"A entrada de produtos na linha {row} da planilha não está em formato númerico!");
                dataImport.Entrada = entrada;
            }

            // Saída
            if (string.IsNullOrEmpty(dataImport.Saida?.ToString()))
            {
                dataImport.Saida = 0;
            }
            else
            {
                parsed = int.TryParse(dataImport.Saida.ToString() ?? "0", out int saida);
                if (!parsed)
                    throw new FormatException($"A saída de produtos na linha {row} da planilha não está em formato númerico!");
                dataImport.Saida = saida;
            }

            // Estoque
            if (string.IsNullOrEmpty(dataImport.Estoque?.ToString()))
                throw new InformacaoObrigatoriaException($"Estoque do produto não informado na linha {row} da planilha!");

            parsed = int.TryParse(dataImport.Estoque.ToString(), out int qtdaEstoquePlanilha);
            if (!parsed)
                throw new FormatException($"O estoque de produtos na linha {row} da planilha não está em formato númerico!");

            dataImport.Estoque = qtdaEstoquePlanilha;
            return dataImport;

        }

        private Estoque GetUltimoEstoque(Estoque estoque)
        {
            return Query(e => e.Data < estoque.Data && e.EmpresaId == estoque.EmpresaId && e.ProdutoId == estoque.ProdutoId)
                .OrderByDescending(e => e.Data)
                .Include(x => x.Produto)
                .Include(x => x.Empresa)
                .FirstOrDefault();
        }

        
        public bool EstoqueValido(Estoque estoque, Estoque ultimoEstoque)
        {
            if (estoque != null && ultimoEstoque != null)
            {
                // Valida consistência do estoque
                int qtdaCorreta = ultimoEstoque.Qtda + estoque.Entrada - estoque.Saida;
                if (estoque.Qtda != qtdaCorreta)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Lês todos os dados da planilha, sem fazer tratativa
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private ImportEstoqueData GetDadosRow(IXLRow row)
        {
            ImportEstoqueData data = new ImportEstoqueData
            {
                Empresa = row.Cell(1).Value,
                CodigoProduto = row.Cell(2).Value.ToString(),
                NomeProduto = row.Cell(3).Value.ToString(),
                Entrada = row.Cell(4).Value.ToString(),
                Saida = row.Cell(5).Value.ToString(),
                Estoque = row.Cell(6).Value.ToString(),
                Row = row.RowNumber()
            };
            return data;
        }

    }
}
