using LibraryApi.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IDBManager _db;

        public BooksController(IDBManager db)
        {
            _db = db;
        }

        // Listele- GET
        [HttpGet]
        public IActionResult GetBooks()
        {
            try
            {
                _db.OpenConnection();
                string sorguListele = @"
            SELECT k.KitapID, k.KitapAd, y.YazarAd, t.TurAd, k.RafNo
            FROM Kitaplar k
            INNER JOIN Yazarlar y ON k.YazarID = y.YazarID
            INNER JOIN Turler t ON k.TurID = t.TurID";

                DataTable veri = _db.ReadData(sorguListele);

                return Ok(veri);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                _db.CloseConnection();
            }
        } 
    }
}
