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
        private string baglantiAdresi = @"Server=.\SQLEXPRESS;Database=Kutuphane;User Id=sa;Password=1;Encrypt=False;TrustServerCertificate=True;";
        private SqlConnection baglanti;

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
            // Her işlemde komutu sıfırdan oluşturur
            SqlCommand komut = new SqlCommand(query, baglanti);
            int sonuc = komut.ExecuteNonQuery();
            Console.WriteLine(sonuc + " satır etkilendi.");
        }

        public void ReadData(string query)
        {
            SqlCommand komut = new SqlCommand(query, baglanti);
            SqlDataReader oku = komut.ExecuteReader();

            Console.WriteLine("Veriler Getiriliyor...");

            while (oku.Read())
            {
                Console.WriteLine("ID: " + oku[0] + " - Kitap Adı: " + oku[1]);
            }
            oku.Close(); 
        }
    }
}