using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoProdutos.Exceptions;
using GestaoProdutos.Helpers;
using GestaoProdutos.Model;
using GestaoProdutos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GestaoProdutos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        private readonly IEmpresasService _service;
        private readonly ILogger _logger;

        public EmpresasController(IEmpresasService service, ILogger<EmpresasController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<PagedResult<Empresa>> Get(
            [FromQuery]int? page = 1, [FromQuery]int? pageSize = 25,
            [FromQuery]string query = null, [FromQuery]string ordem = "nome",
            [FromQuery]bool desc = false)
        {
            if (!page.HasValue && !pageSize.HasValue)
                return BadRequest("O número da página e a quantidade de itens por página devem ser informados!");

            try
            {
                return _service.Filter(query, ordem, desc)
                    .GetPaged(page.Value, pageSize.Value);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido!");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Empresa> Get(int id)
        {
            try
            {
                Empresa registro = _service.Find(id);
                if (registro == null) return NotFound("Empresa não encontrada!");
                return Ok(registro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido!");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<Empresa> Post(Empresa empresa)
        {
            try
            {
                return Ok(_service.Add(empresa));
            }
            catch (Exception ex)
            {
                if (_service.Exist(e => e.Nome == empresa.Nome))
                    return Conflict($"Já existe uma empresa cadastrada com esse nome!");
                _logger.LogError(ex, "Erro desconhecido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, Empresa empresa)
        {
            try
            {
                return Ok(_service.Update(empresa, id));
            }
            catch (NotFoundException)
            {
                return NotFound($"Empresa de cód. {id} não encontrada!");
            }
            catch (Exception ex)
            {
                if (_service.Exist(e => e.Nome == empresa.Nome))
                    return Conflict($"Já existe uma empresa cadastrada com esse nome!");
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
                return NotFound($"Empresa de cód. {id} não encontrado!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}