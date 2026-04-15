using System;

namespace DbServiceApp
{
    public interface ILoggerService
    {
        void LogYaz(string durum, string islemTipi, string mesaj);
    }
}
