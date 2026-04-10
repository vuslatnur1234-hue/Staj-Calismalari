using DbServiceApp;
using Microsoft.Extensions.DependencyInjection;
using System;

// Servis kaydı
var servisler = new ServiceCollection()
    .AddSingleton<IDBManager, DBManager>()
    .BuildServiceProvider();

var db = servisler.GetService<IDBManager>();

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
Console.ReadKey();