using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Infrastructure.Core
{
    public interface IProcessor
    {
        void Start();
        void Stop();
    }
}
