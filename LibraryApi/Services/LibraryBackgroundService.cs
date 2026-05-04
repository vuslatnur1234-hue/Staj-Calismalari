using Microsoft.Extensions.Configuration;
using LibraryApi.Data;
using LibraryApi.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryApi.Services
{
    public class LibraryBackgroundService : BackgroundService
    {
        private readonly ILoggerService _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LibraryBackgroundService(ILoggerService logger, IServiceProvider serviceProvider, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            _logger.LogYaz("SUCCESS", "System", "Arka plan servisi aktif edildi.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Random rnd = new Random();
                    int rastgeleSira = rnd.Next(0, 40);

                    string apiKey = _configuration["GoogleBooksApi:ApiKey"];

                    string apiUrl = $"https://www.googleapis.com/books/v1/volumes?q={rastgeleSira}&maxResults=3&startIndex={rastgeleSira}&key={apiKey}";

                    var response = await _httpClient.GetStringAsync(apiUrl, stoppingToken);
                    var apiData = JsonSerializer.Deserialize<GoogleBooksResponse>(response);

                    if (apiData?.Items != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<IDBManager>();
                        db.OpenConnection();

                        foreach (var item in apiData.Items)
                        {
                            var info = item.VolumeInfo;
                            string kitapAd = info.Title?.Replace("'", "''");
                            string yazarAd = info.Authors != null ? info.Authors[0].Replace("'", "''") : "Anonim";
                            string turAd = info.Categories != null ? info.Categories[0].Replace("'", "''") : "Genel";
                            int sayfa = info.PageCount ?? 0;
                            string yil = !string.IsNullOrEmpty(info.PublishedDate) ? info.PublishedDate.Substring(0, 4) : "2024";

                            try
                            {
                                try { db.ExecuteNonQuery($"INSERT INTO Yazarlar (YazarAd) VALUES ('{yazarAd}')"); } catch { }
                                try { db.ExecuteNonQuery($"INSERT INTO Turler (TurAd) VALUES ('{turAd}')"); } catch { }

                                string yId = $"(SELECT TOP 1 YazarID FROM Yazarlar WHERE YazarAd = '{yazarAd}')";
                                string tId = $"(SELECT TOP 1 TurID FROM Turler WHERE TurAd = '{turAd}')";

                                // SQL: "bu KitapAd veritabanında var mı? YOKSA (NOT EXISTS)"
                                string sorguEkle = $@"
                        IF NOT EXISTS (SELECT 1 FROM Kitaplar WHERE KitapAd = '{kitapAd}')
                        BEGIN
                            INSERT INTO Kitaplar (KitapAd, YazarID, TurID, SayfaSayisi, BasimYili, StokAdeti, RafNo) 
                            VALUES ('{kitapAd}', {yId}, {tId}, {sayfa}, '{yil}', 5, 'BG-1')
                        END";

                                db.ExecuteNonQuery(sorguEkle);
                                _logger.LogYaz("SUCCESS", "Auto-Sync", $"İşlem denendi: '{kitapAd}'");
                            }
                            catch (Exception)
                            {
                                // Hata olursa es geç
                            }
                        }
                        db.CloseConnection();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogYaz("FAIL", "Auto-Sync", "Hata: " + ex.Message);
                }

                //await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            //await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}