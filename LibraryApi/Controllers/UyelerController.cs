using LibraryApi.DTOs;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UyelerController : ControllerBase
    {
        private readonly IUyeRepository _uyeRepository;

        public UyelerController(IUyeRepository uyeRepository)
        {
            _uyeRepository = uyeRepository;
        }

        [HttpGet]
        public IActionResult GetUyeler()
        {
            try
            {
                var uyeler = _uyeRepository.GetAll();
                return Ok(uyeler);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddUye([FromBody] UyeRequestDto yeniUye)
        {
            try
            {
                _uyeRepository.Add(yeniUye);
                return Ok("Üye başarıyla eklendi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUye(int id, [FromBody] UyeRequestDto guncelUye)
        {
            try
            {
                _uyeRepository.Update(id, guncelUye);
                return Ok($"{id} numaralı üye güncellendi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUye(int id)
        {
            try
            {
                _uyeRepository.Delete(id);
                return Ok($"{id} numaralı üye sistemden silindi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}