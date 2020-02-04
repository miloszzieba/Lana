using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Domain.Models
{
    public class Prediction
    {
        public string Source { get; set; }
        public string Importance { get; set; }
        public string Text { get; set; }
    }
}
