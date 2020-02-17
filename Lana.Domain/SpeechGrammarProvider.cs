using Lana.Core.Interfaces;
using Lana.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Domain
{
    public class SpeechGrammarProvider : ISpeechGrammarProvider
    {
        private readonly IEnumerable<ILanaAction> _actions;

        public SpeechGrammarProvider(IEnumerable<ILanaAction> actions)
        {
            this._actions = actions;
        }

        public IEnumerable<Grammar> ProvideGrammars()
        {
            var result = new List<Grammar>()
            {
                CreateDefaultGrammar(),
                CreateLanaGrammar()
            };

            foreach (var action in this._actions)
                result.Add(CreateActionGrammar(action));

            return result;
        }

        private static Grammar CreateActionGrammar(ILanaAction action)
        {
            var lanaGrammarBuilder = new GrammarBuilder();
            lanaGrammarBuilder.Append("Lana");
            lanaGrammarBuilder.Append(action.Phrase);
            lanaGrammarBuilder.AppendDictation();
            var lanaGrammar = new Grammar(lanaGrammarBuilder);
            lanaGrammar.Name = action.Phrase;
            return lanaGrammar;
        }

        private static Grammar CreateDefaultGrammar()
        {
            var defaultGrammarBuilder = new GrammarBuilder();
            defaultGrammarBuilder.AppendDictation();
            var defaultGrammar = new Grammar(defaultGrammarBuilder);
            defaultGrammar.Name = "Default";
            return defaultGrammar;
        }

        private static Grammar CreateLanaGrammar()
        {
            var lanaGrammarBuilder = new GrammarBuilder();
            lanaGrammarBuilder.Append("Lana");
            lanaGrammarBuilder.AppendDictation();
            var lanaGrammar = new Grammar(lanaGrammarBuilder);
            lanaGrammar.Name = "Lana";
            return lanaGrammar;
        }


    }
}
