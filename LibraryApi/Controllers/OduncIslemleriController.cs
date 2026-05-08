using LibraryApi.DTOs;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OduncIslemleriController : ControllerBase
    {
        private readonly IOduncRepository _oduncRepository;

        public OduncIslemleriController(IOduncRepository oduncRepository)
        {
            _oduncRepository = oduncRepository;
        }

        [HttpGet]
        public IActionResult GetIslemler()
        {
            try
            {
                var islemler = _oduncRepository.GetAll();
                return Ok(islemler);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult OduncVer([FromBody] OduncRequestDto dto)
        {
            try
            {
                _oduncRepository.Add(dto);
                return Ok("Kitap başarıyla ödünç verildi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/teslim")]
        public IActionResult TeslimAl(int id)
        {
            try
            {
                _oduncRepository.TeslimAl(id);
                return Ok($"{id} numaralı işlem teslim alındı.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}