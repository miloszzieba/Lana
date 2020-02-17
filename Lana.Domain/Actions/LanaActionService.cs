using Lana.Core.Interfaces;
using Lana.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Domain.Actions
{
    public class LanaActionService : ILanaActionService
    {
        public IEnumerable<ILanaAction> GetAll()
        {
            return new List<ILanaAction>();
        }
    }
}
