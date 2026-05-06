using LibraryApi.DTOs;
using System.Collections.Generic;

namespace LibraryApi.Repositories
{
    public interface IBookRepository
    {
        List<BookDto> GetAll();
        void Add(BookRequestDto dto);
        void Update(int id, BookRequestDto dto);
        void Delete(int id);
    }
}