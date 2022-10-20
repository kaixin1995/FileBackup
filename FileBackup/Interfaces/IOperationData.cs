using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Interfaces
{
    public interface IOperationData
    {
        public void Update();
        public void Empty();
        public void InitThe();
        public bool Delete();
        public bool Add();


    }
}
