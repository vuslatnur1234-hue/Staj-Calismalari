using LibraryApi.DTOs;
using System.Collections.Generic;

namespace LibraryApi.Repositories.Interfaces
{
    public interface IBookRepository
    {
        List<BookDto> GetAll();
        void Add(BookRequestDto dto);
        void Update(int id, BookRequestDto dto);
        void Delete(int id);
        void Patch(int id, BookRequestDto dto);
        BookDto GetById(int id);
    }
}