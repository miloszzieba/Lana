using Lana.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Core.Interfaces
{
    public interface ILanaActionService
    {
        IEnumerable<ILanaAction> GetAll();
    }
}
