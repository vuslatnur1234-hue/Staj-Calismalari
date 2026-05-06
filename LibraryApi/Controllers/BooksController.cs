using LibraryApi.DTOs;
using LibraryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public IActionResult GetBooks()
        {
            try
            {
                var kitaplar = _bookRepository.GetAll();
                return Ok(kitaplar);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] BookRequestDto yeniKitap)
        {
            try
            {
                _bookRepository.Add(yeniKitap);
                return Ok("Kitap başarıyla eklendi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] BookRequestDto guncelKitap)
        {
            try
            {
                _bookRepository.Update(id, guncelKitap);
                return Ok($"{id} numaralı kitap güncellendi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            try
            {
                _bookRepository.Delete(id);
                return Ok($"{id} numaralı kitap sistemden silindi.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}