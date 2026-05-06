using LibraryApi.Data;
using LibraryApi.DTOs;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace LibraryApi.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IDBManager _db;

        public BookRepository(IDBManager db)
        {
            _db = db;
        }

        public List<BookDto> GetAll()
        {
            _db.OpenConnection();
            string sorgu = @"
                SELECT k.KitapID, k.KitapAd, y.YazarAd, t.TurAd, k.RafNo
                FROM Kitaplar k
                INNER JOIN Yazarlar y ON k.YazarID = y.YazarID
                INNER JOIN Turler t ON k.TurID = t.TurID";

            DataTable veri = _db.ReadData(sorgu);
            _db.CloseConnection();

            List<BookDto> kitaplar = new List<BookDto>();
            foreach (DataRow satir in veri.Rows)
            {
                kitaplar.Add(new BookDto
                {
                    KitapID = (int)satir["KitapID"],
                    KitapAd = satir["KitapAd"].ToString(),
                    YazarAd = satir["YazarAd"].ToString(),
                    TurAd = satir["TurAd"].ToString(),
                    RafNo = satir["RafNo"].ToString()
                });
            }
            return kitaplar;
        }

        public void Add(BookRequestDto dto)
        {
            _db.OpenConnection();
            string sorgu = @"
                INSERT INTO Kitaplar (KitapAd, YazarID, TurID, SayfaSayisi, BasimYili, StokAdeti, RafNo) 
                VALUES (@KitapAd, @YazarID, @TurID, @SayfaSayisi, @BasimYili, @StokAdeti, @RafNo)";

            SqlParameter[] parametreler = {
                new SqlParameter("@KitapAd", dto.KitapAd),
                new SqlParameter("@YazarID", dto.YazarID),
                new SqlParameter("@TurID", dto.TurID),
                new SqlParameter("@SayfaSayisi", dto.SayfaSayisi),
                new SqlParameter("@BasimYili", dto.BasimYili),
                new SqlParameter("@StokAdeti", dto.StokAdeti),
                new SqlParameter("@RafNo", dto.RafNo)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }

        public void Update(int id, BookRequestDto dto)
        {
            _db.OpenConnection();
            string sorgu = @"
                UPDATE Kitaplar 
                SET KitapAd = @KitapAd, StokAdeti = @StokAdeti, RafNo = @RafNo 
                WHERE KitapID = @KitapID";

            SqlParameter[] parametreler = {
                new SqlParameter("@KitapAd", dto.KitapAd),
                new SqlParameter("@StokAdeti", dto.StokAdeti),
                new SqlParameter("@RafNo", dto.RafNo),
                new SqlParameter("@KitapID", id)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }

        public void Delete(int id)
        {
            _db.OpenConnection();
            string sorgu = "DELETE FROM Kitaplar WHERE KitapID = @KitapID";

            SqlParameter[] parametreler = {
                new SqlParameter("@KitapID", id)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }
    }
}