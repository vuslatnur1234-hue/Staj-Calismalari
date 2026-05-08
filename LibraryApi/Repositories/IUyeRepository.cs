using LibraryApi.DTOs;
using System.Collections.Generic;

namespace LibraryApi.Repositories
{
    public interface IUyeRepository
    {
        List<UyeDto> GetAll();
        void Add(UyeRequestDto dto);
        void Update(int id, UyeRequestDto dto);
        void Delete(int id);
    }
}