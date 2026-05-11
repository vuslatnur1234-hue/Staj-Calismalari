using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace LibraryApi.Repositories
{
    public class UyeRepository : IUyeRepository
    {
        private readonly IDBManager _db;

        public UyeRepository(IDBManager db)
        {
            _db = db;
        }

        public List<UyeDto> GetAll()
        {
            _db.OpenConnection();
            string sorgu = "SELECT UyeID, Ad, Soyad, Telefon, Email, KayitTarihi FROM Uyeler";

            DataTable veri = _db.ReadData(sorgu);
            _db.CloseConnection();

            List<UyeDto> uyeler = new List<UyeDto>();
            foreach (DataRow satir in veri.Rows)
            {
                uyeler.Add(new UyeDto
                {
                    UyeID = (int)satir["UyeID"],
                    Ad = satir["Ad"].ToString(),
                    Soyad = satir["Soyad"].ToString(),
                    Telefon = satir["Telefon"].ToString(),
                    Email = satir["Email"].ToString(),
                    KayitTarihi = satir["KayitTarihi"].ToString()
                });
            }
            return uyeler;
        }

        public void Add(UyeRequestDto dto)
        {
            _db.OpenConnection();
            string sorgu = @"
                INSERT INTO Uyeler (Ad, Soyad, Telefon, Email) 
                VALUES (@Ad, @Soyad, @Telefon, @Email)";

            SqlParameter[] parametreler = {
                new SqlParameter("@Ad", dto.Ad),
                new SqlParameter("@Soyad", dto.Soyad),
                new SqlParameter("@Telefon", dto.Telefon),
                new SqlParameter("@Email", dto.Email)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }

        public void Update(int id, UyeRequestDto dto)
        {
            _db.OpenConnection();
            string sorgu = @"
                UPDATE Uyeler 
                SET Ad = @Ad, Soyad = @Soyad, Telefon = @Telefon, Email = @Email
                WHERE UyeID = @UyeID";

            SqlParameter[] parametreler = {
                new SqlParameter("@Ad", dto.Ad),
                new SqlParameter("@Soyad", dto.Soyad),
                new SqlParameter("@Telefon", dto.Telefon),
                new SqlParameter("@Email", dto.Email),
                new SqlParameter("@UyeID", id)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }

        public void Delete(int id)
        {
            _db.OpenConnection();
            string sorgu = "DELETE FROM Uyeler WHERE UyeID = @UyeID";

            SqlParameter[] parametreler = {
                new SqlParameter("@UyeID", id)
            };

            _db.ExecuteNonQuery(sorgu, parametreler);
            _db.CloseConnection();
        }

        public UyeDto GetById(int id)
        {
            _db.OpenConnection();
            string sorgu = "SELECT UyeID, Ad, Soyad, Telefon, Email, KayitTarihi FROM Uyeler WHERE UyeID = @UyeID";

            SqlParameter[] parametreler = {
        new SqlParameter("@UyeID", id)
    };

            DataTable veri = _db.ReadData(sorgu, parametreler);
            _db.CloseConnection();

            if (veri.Rows.Count == 0) return null;

            DataRow satir = veri.Rows[0];
            return new UyeDto
            {
                UyeID = (int)satir["UyeID"],
                Ad = satir["Ad"].ToString(),
                Soyad = satir["Soyad"].ToString(),
                Telefon = satir["Telefon"].ToString(),
                Email = satir["Email"].ToString(),
                KayitTarihi = satir["KayitTarihi"].ToString()
            };
        }

        public void Patch(int id, UyeRequestDto dto)
        {
            _db.OpenConnection();
            var satirlar = new List<string>();
            var parametreler = new List<SqlParameter>();

            if (dto.Ad != null)
            {
                satirlar.Add("Ad = @Ad");
                parametreler.Add(new SqlParameter("@Ad", dto.Ad));
            }
            if (dto.Soyad != null)
            {
                satirlar.Add("Soyad = @Soyad");
                parametreler.Add(new SqlParameter("@Soyad", dto.Soyad));
            }
            if (dto.Telefon != null)
            {
                satirlar.Add("Telefon = @Telefon");
                parametreler.Add(new SqlParameter("@Telefon", dto.Telefon));
            }
            if (dto.Email != null)
            {
                satirlar.Add("Email = @Email");
                parametreler.Add(new SqlParameter("@Email", dto.Email));
            }

            parametreler.Add(new SqlParameter("@UyeID", id));
            string sorgu = $"UPDATE Uyeler SET {string.Join(", ", satirlar)} WHERE UyeID = @UyeID";
            _db.ExecuteNonQuery(sorgu, parametreler.ToArray());
            _db.CloseConnection();
        }
    }
}