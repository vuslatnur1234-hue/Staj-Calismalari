using LibraryApi.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace LibraryApi.Data
{
    public class DBManager : IDBManager
    {
        private readonly ILoggerService _logger;
        private readonly string _baglantiAdresi;
        private SqlConnection baglanti;

        public DBManager(ILoggerService logger, IConfiguration configuration)
        {
            _logger = logger;
            _baglantiAdresi = configuration.GetConnectionString("DefaultConnection");
        }
        public void OpenConnection()
        {
            baglanti = new SqlConnection(_baglantiAdresi);
            baglanti.Open();
            Console.WriteLine("Bağlantı açıldı.");
        }

        public void CloseConnection()
        {
            if (baglanti != null)
            {
                baglanti.Close();
                Console.WriteLine("Bağlantı kapatıldı.");
            }
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                SqlCommand komut = new SqlCommand(query, baglanti);
                int sonuc = komut.ExecuteNonQuery();

                // Başarılı işlem logu
                _logger.LogYaz("SUCCESS", "ExecuteNonQuery", $"Sorgu başarıyla çalıştırıldı: {query}");

                Console.WriteLine(sonuc + " satır etkilendi.");
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                _logger.LogYaz("FAIL", "ExecuteNonQuery", $"Hata Oluştu: {ex.Message} | Sorgu: {query}");
                throw;
            }
        }

        public DataTable ReadData(string query)
        {
            try
            {
                SqlCommand komut = new SqlCommand(query, baglanti);
                SqlDataReader oku = komut.ExecuteReader();

                DataTable tablo = new DataTable();
                tablo.Load(oku); 

                oku.Close();
                return tablo; 
            }
            catch (Exception ex)
            {
                _logger.LogYaz("FAIL", "ReadData", $"Hata: {ex.Message}");
                throw;
            }
        }
    }
}