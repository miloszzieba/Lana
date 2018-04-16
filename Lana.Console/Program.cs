using Lana.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lana.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var speechListener = new SpeechListener();
            var cts = new CancellationTokenSource();
            var speechListenerTask = Task.Run(async() => await speechListener.Run(cts));

            System.Console.WriteLine("Press any key to exit program.");
            System.Console.ReadKey();
            cts.Cancel();

            try
            {
                speechListenerTask.Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    System.Console.WriteLine("Exception has been thrown:");
                    System.Console.WriteLine(innerException);
                    System.Console.WriteLine();
                }
                System.Console.ReadKey();
            }
        }
    }
}
