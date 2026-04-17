using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DbServiceApp
{
    public class DBManager : IDBManager
    {
        private readonly ILoggerService _logger;
        private string baglantiAdresi = @"Server=.\SQLEXPRESS;Database=Kutuphane;User Id=sa;Password=1;Encrypt=False;TrustServerCertificate=True;";
        private SqlConnection baglanti;
        public DBManager(ILoggerService logger)
        {
            _logger = logger;
        }
        public void OpenConnection()
        {
            baglanti = new SqlConnection(baglantiAdresi);
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

        public void ReadData(string query)
        {
            try
            {
                SqlCommand komut = new SqlCommand(query, baglanti);
                SqlDataReader oku = komut.ExecuteReader();

                Console.WriteLine("Veriler Getiriliyor...");

                int kayitSayisi = 0;
                while (oku.Read())
                {
                    Console.WriteLine("ID: " + oku[0] + " - Kitap Adı: " + oku[1]);
                    kayitSayisi++;
                }
                oku.Close();

                // Veri okuma başarılı log 
                _logger.LogYaz("SUCCESS", "ReadData", $"Veri okuma başarılı. Toplam {kayitSayisi} kayıt getirildi.");
            }
            catch (Exception ex)
            {
                // Veri okuma hatası log
                _logger.LogYaz("FAIL", "ReadData", $"Hata: {ex.Message}");
                throw;
            }
        }
    }
}