OduncIslemi islem = new OduncIslemi();

islem.KitapAd = "Fahrenheit";
islem.UyeAd = "Ali Şahin";

islem.EkranaYazdir();

abstract class Kitap // Soyutlama (Abstraction)
{
    private string kitapAd; // Kapsülleme (Encapsulation)

    public string KitapAd
    {
        get { return kitapAd; }
        set { kitapAd = value; }
    }

    public abstract void EkranaYazdir(); // Soyut metod
}

class OduncIslemi : Kitap // Kalıtım (Inheritance)
{
    private string uyeAd; // Kapsülleme

    public string UyeAd
    {
        get { return uyeAd; }
        set { uyeAd = value; }
    }

    public override void EkranaYazdir() // Çok biçimlilik (Polymorphism) - Override
    {
        Console.WriteLine(UyeAd + " adlı üye " + KitapAd + " kitabını ödünç aldı.");
    }
}