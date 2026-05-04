using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApi.Data
{
    public interface IDBManager
    {
        void OpenConnection();
        void CloseConnection();
        void ExecuteNonQuery(string query);
        DataTable ReadData(string query);
    }
}
