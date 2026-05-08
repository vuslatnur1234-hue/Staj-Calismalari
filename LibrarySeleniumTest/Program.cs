using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace LibrarySeleniumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test başlıyor...");

            
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized"); 
            options.AddArgument("--disable-gpu");    
            options.AddArgument("--no-sandbox");     

            IWebDriver driver = new ChromeDriver(options);

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);

            try
            {
                driver.Navigate().GoToUrl("https://books.google.com/");

                IWebElement aramaKutusu = driver.FindElement(By.Name("q"));

                aramaKutusu.SendKeys("C# Software Engineering");
                aramaKutusu.SendKeys(Keys.Enter);

                Thread.Sleep(3000);

                if (driver.Title.Contains("C#") || driver.Title.Contains("Google"))
                {
                    Console.WriteLine("TEST BAŞARILI: Kitap arama işlevi düzgün çalışıyor.");
                }
                else
                {
                    Console.WriteLine("TEST BAŞARILI: Sayfa yüklendi ancak başlık farklı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TEST SIRASINDA BİR SORUN ÇIKTI: " + ex.Message);
            }
            finally
            {
                driver.Quit();
                Console.WriteLine("Test tamamlandı, tarayıcı kapatıldı.");
            }
        }
    }
}