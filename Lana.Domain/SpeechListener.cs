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
        //I need cancellationTokenSource, because I might want to cancel listening by command.
        public async Task Run(CancellationTokenSource cancellationTokenSource)
        {
            using (var recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US")))
            {
                var grammars = await CreateGrammars();
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

        private async Task<IEnumerable<Grammar>> CreateGrammars()
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
            string command = e.Result.Text;
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Recognized: {command}. Confidence: {e.Result.Confidence}");
            Console.WriteLine("Alternates:");
            foreach (var line in e.Result.Alternates)
                Console.WriteLine($"            {line.Text}. Confidence: {line.Confidence}");
            Console.WriteLine("-----------------------------------");

            if (e.Result.Grammar.Name == "Lana")
            {

                var waveStream = new MemoryStream();
                e.Result.Audio.WriteToWaveStream(waveStream);
                waveStream.Flush();

                var converter = new WitAiParser();
                var text = converter.Parse(waveStream.ToArray()).GetAwaiter().GetResult();

                Console.WriteLine("Wit.AI:");
                Console.WriteLine(text);
                Console.WriteLine("-----------------------------------");

                var synthesizer = new SpeechSynthesizer();
                synthesizer.Volume = 100;  // 0...100
                synthesizer.Rate = -2;     // -10...10

                // Synchronous
                synthesizer.Speak(text);
            }
        }
    }
}
