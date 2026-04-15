using Microsoft.Data.SqlClient;
using System;

namespace DbServiceApp
{
    public class SqlLogger : ILoggerService
    {
        private string _baglantiAdresi = @"Server=.\SQLEXPRESS;Database=Kutuphane;User Id=sa;Password=1;Encrypt=False;TrustServerCertificate=True;";

        public void LogYaz(string durum, string islemTipi, string mesaj)
        {
            using (SqlConnection baglanti = new SqlConnection(_baglantiAdresi))
            {
                baglanti.Open();

                string sorgu = "INSERT INTO IslemLoglari (Durum, IslemTipi, Mesaj) VALUES (@Durum, @IslemTipi, @Mesaj)";

                using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                {
                    // Güvenlik için (SQL Injection önleme) parametreli sorgu yapısı tercih edildi
                    komut.Parameters.AddWithValue("@Durum", durum);
                    komut.Parameters.AddWithValue("@IslemTipi", islemTipi);
                    komut.Parameters.AddWithValue("@Mesaj", mesaj);

                    komut.ExecuteNonQuery();
                }
            }
        }
    }
}