OduncIslemi islem = new OduncIslemi();

islem.KitapAd = "Fahrenheit";
islem.UyeAd = "Ali Şahin";

islem.EkranaYazdir();

class Kitap
{
    public string KitapAd;
    public virtual void EkranaYazdir()
    {
        Console.WriteLine("Kitap Adı: " + KitapAd);
    }
}

// 1. Kalıtım
// OduncIslemi sınıfı Kitap sınıfından miras alıyor : Kitap
// KitapAd değişkenini baştan yazmadan kullanabiliyor
class OduncIslemi : Kitap
{
    public string UyeAd;

    // 2. Çok Biçimlilik 
    // Kitap sınıfındaki virtual metod burada override edilerek ezildi 
    // Aynı metod farklı davranış sergileyebiliyor
    public override void EkranaYazdir()
    {
        Console.WriteLine(UyeAd + " adlı üye " + KitapAd + " kitabını ödünç aldı.");
    }
}


