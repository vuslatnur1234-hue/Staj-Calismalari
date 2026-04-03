OduncIslemi islem = new OduncIslemi();

islem.KitapAd = "Fahrenheit";
islem.UyeAd = "Ali Şahin";

islem.EkranaYazdir();

// Kalıtım
// OduncIslemi sınıfı Kitap sınıfından miras alıyor. KitapAd ve EkranaYazdir metodunu kullanabiliyor.
class Kitap
{
    public string KitapAd;

    public virtual void EkranaYazdir()
    {
        Console.WriteLine("Kitap Adı: " + KitapAd);
    }
}
// Çok biçimlilik 
// Kitap sınıfındaki virtual metod OduncIslemi sınıfında override edildi. 
// Böylece aynı metod farklı davranış sergileyebiliyor.
class OduncIslemi : Kitap
{
    public string UyeAd;

    public override void EkranaYazdir()
    {
        Console.WriteLine(UyeAd + " adlı üye " + KitapAd + " kitabını ödünç aldı.");
    }
}

