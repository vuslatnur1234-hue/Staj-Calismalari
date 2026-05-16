using LibraryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IDBManager _db;

        public LogsController(IDBManager db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllLogs()
        {
            try
            {
                _db.OpenConnection();

                string sorgu = @"
                    SELECT Id, Durum, IslemTipi, Mesaj, Tarih
                    FROM IslemLoglari
                    ORDER BY Id DESC";

                DataTable veri = _db.ReadData(sorgu);
                _db.CloseConnection();

                List<dynamic> logs = new List<dynamic>();
                foreach (DataRow satir in veri.Rows)
                {
                    logs.Add(new
                    {
                        logID = (int)satir["Id"],
                        durum = satir["Durum"].ToString(),
                        islemTipi = satir["IslemTipi"].ToString(),
                        mesaj = satir["Mesaj"].ToString(),
                        tarihZaman = satir["Tarih"].ToString()
                    });
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("by-status/{status}")]
        public IActionResult GetLogsByStatus(string status)
        {
            try
            {
                _db.OpenConnection();
                string sorgu = @"
                    SELECT Id, Durum, IslemTipi, Mesaj, Tarih
                    FROM IslemLoglari
                    WHERE Durum = @Durum
                    ORDER BY Id DESC";

                SqlParameter[] parametreler = {
                    new SqlParameter("@Durum", status)
                };

                DataTable veri = _db.ReadData(sorgu, parametreler);
                _db.CloseConnection();

                List<dynamic> logs = new List<dynamic>();
                foreach (DataRow satir in veri.Rows)
                {
                    logs.Add(new
                    {
                        logID = (int)satir["Id"],
                        durum = satir["Durum"].ToString(),
                        islemTipi = satir["IslemTipi"].ToString(),
                        mesaj = satir["Mesaj"].ToString(),
                        tarihZaman = satir["Tarih"].ToString()
                    });
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public IActionResult ClearAllLogs()
        {
            try
            {
                _db.OpenConnection();
                string sorgu = "DELETE FROM IslemLoglari";

                _db.ExecuteNonQuery(sorgu);
                _db.CloseConnection();

                return Ok(new { message = "Tüm loglar başarıyla silindi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}