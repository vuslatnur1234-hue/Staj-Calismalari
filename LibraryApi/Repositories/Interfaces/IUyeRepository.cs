using LibraryApi.DTOs;
using System.Collections.Generic;

namespace LibraryApi.Repositories.Interfaces
{
    public interface IUyeRepository
    {
        List<UyeDto> GetAll();
        void Add(UyeRequestDto dto);
        void Update(int id, UyeRequestDto dto);
        void Delete(int id);
        UyeDto GetById(int id);
        void Patch(int id, UyeRequestDto dto);
    }
}