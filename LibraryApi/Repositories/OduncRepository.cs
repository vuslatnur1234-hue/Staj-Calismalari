using LibraryApi.Data;
using LibraryApi.DTOs;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace LibraryApi.Repositories
{
    public class OduncRepository : IOduncRepository
    {
        private readonly IDBManager _db;

        public OduncRepository(IDBManager db)
        {
            _db = db;
        }

        public List<OduncDto> GetAll()
        {
            _db.OpenConnection();
            string sorgu = @"
                SELECT o.IslemID, 
                       u.Ad + ' ' + u.Soyad AS UyeAdSoyad,
                       k.KitapAd,
                       o.VerilisTarihi,
                       o.TeslimTarihi,
                       o.Durum
                FROM OduncIslemleri o
                INNER JOIN Uyeler u ON o.UyeID = u.UyeID
                INNER JOIN Kitaplar k ON o.KitapID = k.KitapID";

            DataTable veri = _db.ReadData(sorgu);
            _db.CloseConnection();

            List<OduncDto> islemler = new List<OduncDto>();
            foreach (DataRow satir in veri.Rows)
            {
                islemler.Add(new OduncDto
                {
                    IslemID = (int)satir["IslemID"],
                    UyeAdSoyad = satir["UyeAdSoyad"].ToString(),
                    KitapAd = satir["KitapAd"].ToString(),
                    VerilisTarihi = satir["VerilisTarihi"].ToString(),
                    TeslimTarihi = satir["TeslimTarihi"] == DBNull.Value ? "Henüz teslim edilmedi" : satir["TeslimTarihi"].ToString(),
                    Durum = satir["Durum"].ToString()
                });
            }
            return islemler;
        }

        public void Add(OduncRequestDto dto)
        {
            _db.OpenConnection();
            string sorgu = @"
                INSERT INTO OduncIslemleri (UyeID, KitapID) 
                VALUES (@UyeID, @KitapID)";

            SqlParameter[] parametreler = {
                new SqlParameter("@UyeID", dto.UyeID),
                new SqlParameter("@KitapID", dto.KitapID)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }

        public void TeslimAl(int id)
        {
            _db.OpenConnection();
            string sorgu = @"
                UPDATE OduncIslemleri 
                SET Durum = 'Teslim Edildi', TeslimTarihi = GETDATE()
                WHERE IslemID = @IslemID";

            SqlParameter[] parametreler = {
                new SqlParameter("@IslemID", id)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }
    }
}