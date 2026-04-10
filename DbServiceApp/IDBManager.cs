using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbServiceApp
{
    public interface IDBManager
    {
        void OpenConnection();
        void CloseConnection();
        void ExecuteNonQuery(string query);
        void ReadData(string query); 
    }
}
