using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Core.Interfaces
{
    public interface IPredictionListener
    {
        string Name { get; }
        string Listen(MemoryStream audioStream);
    }
}
