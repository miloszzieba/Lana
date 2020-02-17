using Lana.Core.Interfaces;
using Lana.Core.Models;
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
        private readonly IEnumerable<IPredictionListener> _predictionListeners;
        private readonly ISpeechGrammarProvider _speechGrammarProvider;

        public SpeechListener(IPredictionRaportingService predictionRaportingService, IEnumerable<IPredictionListener> predictionListeners, ISpeechGrammarProvider speechGrammarProvider)
        {
            this._predictionRaportingService = predictionRaportingService;
            this._predictionListeners = predictionListeners;
            this._speechGrammarProvider = speechGrammarProvider;
        }

        //I need cancellationTokenSource, because I might want to cancel listening by command.
        public void Run(CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null) throw new ArgumentNullException(nameof(cancellationTokenSource));

            using (var recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US")))
            {
                var grammars = this._speechGrammarProvider.ProvideGrammars();
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

        private void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
        {
            var predictions = new List<Prediction>();

            predictions.Add(new Prediction("Windows", e.Result.Text));

            foreach (var predictionAlternative in e.Result.Alternates)
                predictions.Add(new Prediction("Windows", predictionAlternative.Text));

            ProvidePredictions(e, predictions);

            this._predictionRaportingService.AddPredictions(predictions);
        }

        private void ProvidePredictions(SpeechRecognizedEventArgs e, List<Prediction> predictions)
        {
            //So we won't send every sound to clouds and disrupt results
            if (!predictions.Any(x => x.Text.Contains("Lana")))
                return;

            using (var waveStream = new MemoryStream())
            {
                e.Result.Audio.WriteToWaveStream(waveStream);
                waveStream.Flush();

                foreach (var listener in this._predictionListeners)
                {
                    var prediction = listener.Listen(waveStream);
                    predictions.Add(new Prediction(listener.Name, prediction));
                }
            }
        }
    }
}
