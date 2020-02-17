using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Core.Models
{
    public class Prediction
    {
        public Prediction(string source, string text)
        {
            this.Source = source;
            this.Text = text;
        }

        public string Source { get; }
        public string Text { get; }
    }
}
