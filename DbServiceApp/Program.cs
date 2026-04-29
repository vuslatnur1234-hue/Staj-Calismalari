using DbServiceApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.Logging.ClearProviders();

    // Servisler
    builder.Services.AddSingleton<ILoggerService, SqlLogger>();
    builder.Services.AddTransient<IDBManager, DBManager>();
    builder.Services.AddHttpClient();
    builder.Services.AddHostedService<LibraryBackgroundService>();

    var host = builder.Build();
    var db = host.Services.GetRequiredService<IDBManager>();

    await host.StartAsync();

    bool devam = true;

    while (devam)
    {
        Console.Clear();
        Console.WriteLine("KÜTÜPHANE YÖNETİM SİSTEMİ");
        Console.WriteLine("1 - Kitapları Listele");
        Console.WriteLine("2 - Kitap Ekle");
        Console.WriteLine("3 - Kitap Güncelle");
        Console.WriteLine("4 - Kitap Sil");
        Console.WriteLine("5 - Kitap Ara");
        Console.WriteLine("0 - Çıkış");
        Console.Write("\nSeçiminiz: ");

        string secim = Console.ReadLine();

        if (secim == "0")
        {
            devam = false;
            break;
        }

        try
        {
            db.OpenConnection();

            switch (secim)
            {
                case "1":
                    Console.WriteLine("\nKİTAPLAR");
                    string sorguListele = @"
                        SELECT 
                            k.KitapID, 
                            k.KitapAd, 
                            y.YazarAd, 
                            t.TurAd, 
                            k.RafNo
                        FROM Kitaplar k
                        INNER JOIN Yazarlar y ON k.YazarID = y.YazarID
                        INNER JOIN Turler t ON k.TurID = t.TurID";

                    db.ReadData(sorguListele);
                    break;

                case "2":
                    Console.Write("Kitap Adı: ");
                    string adGirdi = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(adGirdi)) { Console.WriteLine("İşlem iptal edildi."); break; }
                    string ad = adGirdi.Replace("'", "''");

                    Console.Write("Yazar Adı: ");
                    string yazar = Console.ReadLine()?.Replace("'", "''");

                    Console.Write("Kitap Türü: ");
                    string tur = Console.ReadLine()?.Replace("'", "''");

                    int sayfa = SayiAl("Sayfa Sayısı: ");
                    if (sayfa == -1) { Console.WriteLine("İşlem iptal edildi."); break; }

                    Console.Write("Basım Yılı: ");
                    string yil = Console.ReadLine()?.Replace("'", "''");

                    int stok = SayiAl("Stok Adeti: ");
                    if (stok == -1) { Console.WriteLine("İşlem iptal edildi."); break; }

                    Console.Write("Raf No: ");
                    string raf = Console.ReadLine()?.Replace("'", "''");

                    string sorguEkle = $@"
                        IF NOT EXISTS (SELECT 1 FROM Yazarlar WHERE YazarAd = '{yazar}')
                        INSERT INTO Yazarlar (YazarAd) VALUES ('{yazar}');

                        IF NOT EXISTS (SELECT 1 FROM Turler WHERE TurAd = '{tur}')
                        INSERT INTO Turler (TurAd) VALUES ('{tur}');

                        DECLARE @YazarID INT = (SELECT TOP 1 YazarID FROM Yazarlar WHERE YazarAd = '{yazar}');
                        DECLARE @TurID INT = (SELECT TOP 1 TurID FROM Turler WHERE TurAd = '{tur}');

                        INSERT INTO Kitaplar (KitapAd, YazarID, TurID, SayfaSayisi, BasimYili, StokAdeti, RafNo)
                        VALUES ('{ad}', @YazarID, @TurID, {sayfa}, '{yil}', {stok}, '{raf}');";

                    db.ExecuteNonQuery(sorguEkle);

                    Console.WriteLine("Kayıt başarıyla eklendi");
                    break;

                case "3":
                    Console.WriteLine("\nKitap Güncelle:");

                    Console.Write("Eski Ad: ");
                    string eskiGirdi = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(eskiGirdi)) { Console.WriteLine("İşlem iptal edildi."); break; }
                    string eski = eskiGirdi.Replace("'", "''");

                    Console.Write("Yeni Ad: ");
                    string yeni = Console.ReadLine()?.Replace("'", "''");

                    db.ExecuteNonQuery($@"
                        UPDATE Kitaplar 
                        SET KitapAd = '{yeni}' 
                        WHERE KitapAd = '{eski}'");

                    Console.WriteLine("Güncellendi.");
                    break;

                case "4":
                    Console.WriteLine("\nKitap Sil:");

                    Console.Write("Silinecek Ad: ");
                    string silGirdi = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(silGirdi)) { Console.WriteLine("İşlem iptal edildi."); break; }
                    string sil = silGirdi.Replace("'", "''");

                    db.ExecuteNonQuery($@"
                        DELETE FROM Kitaplar 
                        WHERE KitapAd = '{sil}'");

                    Console.WriteLine("Silindi.");
                    break;

                case "5":
                    Console.WriteLine("\nKitap Ara:");

                    Console.Write("Aranacak Kelime: ");
                    string araGirdi = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(araGirdi)) { Console.WriteLine("İşlem iptal edildi."); break; }
                    string ara = araGirdi.Replace("'", "''");

                    string sorguAra = $@"
                        SELECT 
                            k.KitapID,
                            k.KitapAd, 
                            y.YazarAd, 
                            t.TurAd,
                            k.RafNo 
                        FROM Kitaplar k
                        INNER JOIN Yazarlar y ON k.YazarID = y.YazarID
                        INNER JOIN Turler t ON k.TurID = t.TurID
                        WHERE k.KitapAd LIKE '%{ara}%'";

                    db.ReadData(sorguAra);
                    break;

                default:
                    Console.WriteLine("\nGeçersiz seçim!");
                    break;
            }

            db.CloseConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nHata: " + ex.Message);
            db.CloseConnection();
        }

        Console.WriteLine("\nDevam etmek için bir tuşa basın...");
        Console.ReadKey();
    }

    int SayiAl(string mesaj)
    {
        Console.Write(mesaj);
        while (true)
        {
            string girdi = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(girdi)) return -1;

            if (int.TryParse(girdi, out int sayi)) return sayi;

            Console.Write("Geçerli sayı gir: ");
        }
    }

    await host.StopAsync();
}
catch (Exception ex)
{
    Console.WriteLine("Sistem Başlatılamadı. Kritik Hata: " + ex.Message);
    Console.ReadLine();
}