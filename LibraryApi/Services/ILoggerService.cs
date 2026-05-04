using System;

namespace LibraryApi.Services
{
    public interface ILoggerService
    {
        void LogYaz(string durum, string islemTipi, string mesaj);
    }
}
