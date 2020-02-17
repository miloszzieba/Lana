using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Core.Interfaces
{
    public interface ISpeechGrammarProvider
    {
        IEnumerable<Grammar> ProvideGrammars();
    }
}
