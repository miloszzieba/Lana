using Lana.Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lana.Domain
{
    public class SpeechListener
    {
        private readonly SynchronizationContext _synchronizationContext;
        private readonly ICollection<Prediction> _predictionsList;

        public SpeechListener(ICollection<Prediction> predictionsList, SynchronizationContext synchronizationContext)
        {
            this._predictionsList = predictionsList;
            this._synchronizationContext = synchronizationContext;
        }

        //I need cancellationTokenSource, because I might want to cancel listening by command.
        public void Run(CancellationTokenSource cancellationTokenSource)
        {
            var context = SynchronizationContext.Current;
            using (var recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US")))
            {
                var grammars = CreateGrammars();
                foreach (var grammar in grammars)
                    recognizer.LoadGrammarAsync(grammar);

                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(
                  SpeechRecognizedHandler);

                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                cancellationTokenSource.Token.WaitHandle.WaitOne();
                recognizer.RecognizeAsyncCancel();
            }
        }

        private IEnumerable<Grammar> CreateGrammars()
        {
            var defaultGrammarBuilder = new GrammarBuilder();
            defaultGrammarBuilder.AppendDictation();
            var defaultGrammar = new Grammar(defaultGrammarBuilder);
            defaultGrammar.Name = "Default";

            var lanaGrammarBuilder = new GrammarBuilder();
            lanaGrammarBuilder.Append("Lana");
            lanaGrammarBuilder.AppendDictation();
            var lanaGrammar = new Grammar(lanaGrammarBuilder);
            lanaGrammar.Name = "Lana";

            return new List<Grammar>()
            {
                defaultGrammar,
                lanaGrammar
            };
        }

        private void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
        {
            string prediction = e.Result.Text;
            //REFACTOR
            _synchronizationContext.Send(x =>
                _predictionsList.Add(new Prediction()
                {
                    Source = "Windows",
                    Importance = "Main",
                    Text = prediction
                }), null);

            foreach (var predictionAlternative in e.Result.Alternates)
                //REFACTOR
                _synchronizationContext.Send(x =>
                    _predictionsList.Add(new Prediction()
                    {
                        Source = "Windows",
                        Importance = "Alternative",
                        Text = predictionAlternative.Text
                    }), null);

            //REFACTOR
            if (e.Result.Grammar.Name == "Lana"
                || prediction.Contains("Lana")
                || e.Result.Alternates.Any(x => x.Text.Contains("Lana")))
            {
                var waveStream = new MemoryStream();
                e.Result.Audio.WriteToWaveStream(waveStream);
                waveStream.Flush();

                var converter = new WitAiParser();
                var witaiPrediction = converter.Parse(waveStream.ToArray()).GetAwaiter().GetResult();
                //REFACTOR
                _synchronizationContext.Send(x =>
                    _predictionsList.Add(new Prediction()
                    {
                        Source = "Wit.AI",
                        Importance = "Alternative",
                        Text = witaiPrediction
                    }), null);
            }
        }
    }
}
