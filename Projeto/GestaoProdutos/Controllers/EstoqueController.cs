using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GestaoProdutos.Exceptions;
using GestaoProdutos.Helpers;
using GestaoProdutos.Model;
using GestaoProdutos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GestaoProdutos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _service;
        private readonly ILogger _logger;

        public EstoqueController(IEstoqueService service, ILogger<EstoqueController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<PagedResult<Estoque>> Get(
            [FromQuery]int? page = 1, [FromQuery]int? pageSize = 25,
            [FromQuery]DateTime? data = null, [FromQuery]string produto = null,
            [FromQuery]string empresa = null, [FromQuery]string ordem = null,
            [FromQuery]bool desc = false
        )
        {
            if (!page.HasValue && !pageSize.HasValue)
                return BadRequest("O número da página e a quantidade de itens por página devem ser informados!");

            try
            {
                return _service.Filter(data, produto, empresa, ordem, desc)
                    .GetPaged(page.Value, pageSize.Value);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido!");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Estoque> Get(int id)
        {
            try
            {
                Estoque registro = _service.Find(id);
                if (registro == null) return NotFound("Registro de Histórico não encontrado!");
                return Ok(registro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido!");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<Estoque> Post(Estoque historico)
        {
            try
            {
                return Ok(_service.Add(historico));
            }
            catch (Exception ex)
            {
                if (_service.Exist(h => h.Data == historico.Data && h.ProdutoId == historico.ProdutoId && h.EmpresaId == historico.EmpresaId))
                    return Conflict($"Já existe um registro referente a esse produto na data {historico.Data.ToString("dd/MM/yyyy")}!");
                _logger.LogError(ex, "Erro desconhecido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Estoque historico)
        {
            try
            {
                return Ok(_service.Update(historico, id));
            }
            catch (NotFoundException)
            {
                return NotFound($"Registro de Histórico de cód. {id} não encontrado!");
            }
            catch (Exception ex)
            {
                if (_service.Exist(h => h.Data == historico.Data && h.ProdutoId == historico.ProdutoId && h.EmpresaId == historico.EmpresaId && h.Id != historico.Id))
                    return Conflict($"Já existe um registro referente a esse produto na data {historico.Data.ToString("dd/MM/yyyy")}!");
                _logger.LogError(ex, "Erro desconhecido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                return Ok(_service.Delete(id));
            }
            catch (NotFoundException)
            {
                return NotFound($"Registro de Histórico de cód. {id} não encontrado!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("importacao"), DisableRequestSizeLimit]
        public ActionResult ImportFile(List<IFormFile> files)
        {
            try
            {
                Util.ProcessFiles(Request.Form.Files, _service.ImportFile);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}