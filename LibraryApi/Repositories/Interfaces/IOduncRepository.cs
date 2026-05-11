using LibraryApi.DTOs;
using System.Collections.Generic;

namespace LibraryApi.Repositories.Interfaces
{
    public interface IOduncRepository
    {
        List<OduncDto> GetAll();
        void Add(OduncRequestDto dto);
        void TeslimAl(int id);
        OduncDto GetById(int id);
    }
}