# Staj Çalışmaları — C# / .NET 8

Staj sürecimde geliştirdiğim çalışmaları içeren repodur.  
Temel olarak **OOP prensipleri** ve **veritabanı yönetimi** konularını kapsamaktadır.  
Çalışmalar geliştirilmeye devam etmektedir. 

---

## Proje: Kütüphane Yönetim Sistemi

Konsol tabanlı, MS SQL Server bağlantılı bir kütüphane otomasyon uygulamasıdır.  
Kitap, yazar, üye ve ödünç işlemlerini yönetmeye olanak tanır.

### Özellikler

- 📖 Kitap listeleme (yazar ve tür bilgisiyle birlikte)
- ➕ Yeni kitap / yazar / tür ekleme
- ✏️ Kitap adı güncelleme
- 🗑️ Kitap silme
- 🔍 Kitap adına göre arama
- 🪵 Tüm işlemlerin SQL tabanlı log tablosuna yazılması (`IslemLoglari`)

---

## Uygulanan OOP Kavramları

| Kavram | Açıklama |
|--------|----------|
| **Soyutlama** (Abstraction) | `Kitap` abstract sınıfı ve `IDBManager`, `ILoggerService` arayüzleri |
| **Kapsülleme** (Encapsulation) | Private alanlar, public property'ler (`KitapAd`, `UyeAd`) |
| **Kalıtım** (Inheritance) | `OduncIslemi : Kitap` |
| **Çok Biçimlilik** (Polymorphism) | `virtual` / `override` ile `EkranaYazdir()` metodu |
| **Dependency Injection** | `ServiceCollection` ile `ILoggerService` ve `IDBManager` servis kaydı |

---

## Veritabanı Yapısı

**Veritabanı:** MS SQL Server (SQLEXPRESS) — `Kutuphane`

| Tablo | İçerik |
|-------|--------|
| `Kitaplar` | KitapID, KitapAd, YazarID, TurID, SayfaSayisi, BasimYili, StokAdeti, RafNo |
| `Yazarlar` | YazarID, YazarAd |
| `Turler` | TurID, TurAd |
| `Uyeler` | UyeID, Ad, Soyad, Telefon, Email, KayitTarihi |
| `OduncIslemleri` | IslemID, UyeID, KitapID, VerilisTarihi, TeslimTarihi, Durum |
| `IslemLoglari` | Durum, IslemTipi, Mesaj, Tarih |

---

##  Kullanılan Teknolojiler

- **Dil:** C#
- **Framework:** .NET 8
- **Veritabanı:** MS SQL Server (SQLEXPRESS)
- **Kütüphane:** `Microsoft.Data.SqlClient`, `Microsoft.Extensions.DependencyInjection`
- **IDE:** Visual Studio

---

> Staj çalışmaları — 2026
> Veribase
