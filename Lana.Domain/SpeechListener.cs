using Lana.Domain.Predictions;
using Lana.Domain.Predictions.Models;
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
        private readonly IPredictionRaportingService _predictionRaportingService;

        public SpeechListener(IPredictionRaportingService predictionRaportingService)
        {
            this._predictionRaportingService = predictionRaportingService;
        }

        //I need cancellationTokenSource, because I might want to cancel listening by command.
        public void Run(CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null) throw new ArgumentNullException(nameof(cancellationTokenSource));

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

        private static IEnumerable<Grammar> CreateGrammars()
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
            var predictions = new List<Prediction>();

            predictions.Add(new Prediction("Windows", e.Result.Text));

            foreach (var predictionAlternative in e.Result.Alternates)
                predictions.Add(new Prediction("Windows", predictionAlternative.Text));

            WitAIHandle(e, predictions);

            this._predictionRaportingService.AddPredictions(predictions);
        }

        private static void WitAIHandle(SpeechRecognizedEventArgs e, List<Prediction> predictions)
        {
            //So we won't send every sound to Wit.AI and disrupt results
            if (!predictions.Any(x => x.Text.Contains("Lana")))
                return;

            using (var waveStream = new MemoryStream())
            {
                e.Result.Audio.WriteToWaveStream(waveStream);
                waveStream.Flush();

                var converter = new WitAiParser();
                var witaiPrediction = converter.Parse(waveStream.ToArray()).GetAwaiter().GetResult();
                predictions.Add(new Prediction("Wit.AI", witaiPrediction));
            }
        }
    }
}
