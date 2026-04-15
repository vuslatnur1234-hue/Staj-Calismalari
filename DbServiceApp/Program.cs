using DbServiceApp;
using Microsoft.Extensions.DependencyInjection;
using System;

// Servis kaydı
var servisler = new ServiceCollection()
    .AddSingleton<IDBManager, DBManager>()
    .BuildServiceProvider();

var db = servisler.GetService<IDBManager>();
/* 10.04.2026
Console.WriteLine("--- KÜTÜPHANE OTOMASYONU ---");

db.OpenConnection();

Console.WriteLine("\n[1] Mevcut Kitaplar Listeleniyor:");
db.ReadData("SELECT * FROM Kitaplar");

Console.WriteLine("\n[2] Yeni kitap ekleniyor: Böğürtlen Kışı");
db.ExecuteNonQuery("INSERT INTO Kitaplar (KitapAd) VALUES ('Böğürtlen Kışı')");

Console.WriteLine("\n[3] Kitap ismi güncelleniyor...");
db.ExecuteNonQuery("UPDATE Kitaplar SET KitapAd = 'Böğürtlen Kışı (GÜNCELLENDİ)' WHERE KitapAd = 'Böğürtlen Kışı'");

Console.WriteLine("\n[4] Güncelleme sonrası liste:");
db.ReadData("SELECT * FROM Kitaplar");

Console.WriteLine("\n[5] Silme işlemi yapılıyor...");
db.ExecuteNonQuery("DELETE FROM Kitaplar WHERE KitapAd = 'Böğürtlen Kışı (GÜNCELLENDİ)'");

db.CloseConnection();

Console.WriteLine("\nBütün işlemler bitti. Çıkmak için bir tuşa basın.");
Console.ReadKey();*/

/*...........................................................................*/

// Test
var testIscisi = new DbServiceApp.SqlLogger();
testIscisi.LogYaz("success", "test islemi", "çalışıyor");
Console.WriteLine("test logu sql'e gönderildi");

//13.04.2026 Geliştirilmiş vers.
string secim = "";
Console.WriteLine("\nKÜTÜPHANE YÖNETİM SİSTEMİ v2.0 ");

while (secim != "0")
{
    Console.WriteLine("\nİŞLEM MENÜSÜ ");
    Console.WriteLine("\n1-Listele \n2-Ekle \n3-Güncelle \n4-Sil \n5-Ara \n0-Çıkış");
    Console.Write("Seçiminiz: ");
    secim = Console.ReadLine();

    if (secim == "0") break;

    try
    {
        db.OpenConnection();

        switch (secim)
        {
            case "1":
                Console.WriteLine("\nKitap Listesi:");
                string sorguListele = "SELECT k.KitapID, k.KitapAd + ' (Yazar: ' + y.YazarAd + ' - Tür: ' + t.TurAd + ' - Raf: ' + k.RafNo + ')' " +
                                      "FROM Kitaplar k " +
                                      "INNER JOIN Yazarlar y ON k.YazarID = y.YazarID " +
                                      "INNER JOIN Turler t ON k.TurID = t.TurID";
                db.ReadData(sorguListele);
                break;

            case "2":
                Console.WriteLine("\nYeni Kitap Kaydı:");
                Console.Write("Kitap Adı: "); string ad = Console.ReadLine();
                Console.Write("Yazar Adı: "); string yazar = Console.ReadLine();
                Console.Write("Kitap Türü: "); string tur = Console.ReadLine();
                Console.Write("Sayfa Sayısı: "); string sayfa = Console.ReadLine();
                Console.Write("Basım Yılı: "); string yil = Console.ReadLine();
                Console.Write("Stok Adeti: "); string stok = Console.ReadLine();
                Console.Write("Raf No: "); string raf = Console.ReadLine();

                try
                {
                    db.ExecuteNonQuery("INSERT INTO Yazarlar (YazarAd) VALUES ('" + yazar + "')");
                    db.ExecuteNonQuery("INSERT INTO Turler (TurAd) VALUES ('" + tur + "')");
                }
                catch
                {
                }

                string yId = "(SELECT TOP 1 YazarID FROM Yazarlar WHERE YazarAd = '" + yazar + "')";
                string tId = "(SELECT TOP 1 TurID FROM Turler WHERE TurAd = '" + tur + "')";

                string veriler = "'" + ad + "', " + yId + ", " + tId + ", " + sayfa + ", '" + yil + "', " + stok + ", '" + raf + "'";
                string sorguEkle = "INSERT INTO Kitaplar (KitapAd, YazarID, TurID, SayfaSayisi, BasimYili, StokAdeti, RafNo) VALUES (" + veriler + ")";

                db.ExecuteNonQuery(sorguEkle);
                Console.WriteLine("Kayıt başarıyla eklendi ve liste ile eşleşti!");
                break;

            case "3":
                Console.Write("Eski Ad: "); string eski = Console.ReadLine();
                Console.Write("Yeni Ad: "); string yeni = Console.ReadLine();
                db.ExecuteNonQuery("UPDATE Kitaplar SET KitapAd = '" + yeni + "' WHERE KitapAd = '" + eski + "'");
                Console.WriteLine("Güncellendi.");
                break;

            case "4":
                Console.Write("Silinecek Ad: "); string sil = Console.ReadLine();
                db.ExecuteNonQuery("DELETE FROM Kitaplar WHERE KitapAd = '" + sil + "'");
                Console.WriteLine("Silindi.");
                break;

            case "5":
                Console.Write("Aranacak Kelime: "); string ara = Console.ReadLine();
                string sorguAra = "SELECT k.KitapAd, y.YazarAd, k.RafNo FROM Kitaplar k " +
                                  "INNER JOIN Yazarlar y ON k.YazarID = y.YazarID " +
                                  "WHERE k.KitapAd LIKE '%" + ara + "%'";
                db.ReadData(sorguAra);
                break;

            default:
                Console.WriteLine("Geçersiz seçim!");
                break;
        }

        db.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Hata: " + ex.Message);
        db.CloseConnection();
    }

    if (secim != "0")
    {
        Console.WriteLine("\nDevam etmek için bir tuşa basın...");
        Console.ReadKey();
        Console.Clear();
    }
}

