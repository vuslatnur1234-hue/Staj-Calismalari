using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace DbServiceApp
{
    public class SqlLogger : ILoggerService
    {
        private readonly string _baglantiAdresi;

        public SqlLogger(IConfiguration configuration)
        {
            _baglantiAdresi = configuration.GetConnectionString("DefaultConnection");
        }

        public void LogYaz(string durum, string islemTipi, string mesaj)
        {
            using (SqlConnection baglanti = new SqlConnection(_baglantiAdresi))
            {
                baglanti.Open();

                string sorgu = "INSERT INTO IslemLoglari (Durum, IslemTipi, Mesaj) VALUES (@Durum, @IslemTipi, @Mesaj)";

                using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                {
                    komut.Parameters.AddWithValue("@Durum", durum);
                    komut.Parameters.AddWithValue("@IslemTipi", islemTipi);
                    komut.Parameters.AddWithValue("@Mesaj", mesaj);

                    komut.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"SİSTEM LOGU: {durum} - {mesaj}");
        }
    }
}