using Lana.App.Predictions;
using Lana.Core.Interfaces;
using Lana.Domain;
using Lana.Domain.Actions;
using Lana.Domain.Predictions.Listeners;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lana.App
{
    public partial class MainWindow
    {
        private CancellationTokenSource _cts = null;
        private IPredictionRaportingService _predictionRaportingService = null;
        private ILanaActionService _lanaActionService = null;
        private Task _speechListenerTask = null;

        private void InitializeServices()
        {
            _predictionRaportingService = InitializePredictionRaportingService();
            _lanaActionService = InitializeLanaActionService();

            _cts = new CancellationTokenSource();
            _speechListenerTask = InitializeSpeechListener(
                this._predictionRaportingService,
                this._lanaActionService,
                this._cts);

            DataContext = _predictionRaportingService.PredictionsList;
        }

        private static IPredictionRaportingService InitializePredictionRaportingService()
        {
            var synchronizationContext = SynchronizationContext.Current;
            return new WPFPredictionRaportingService(synchronizationContext);
        }

        private static ILanaActionService InitializeLanaActionService()
        {
            return new LanaActionService();
        }

        private static Task InitializeSpeechListener(
            IPredictionRaportingService predictionRaportingService,
            ILanaActionService lanaActionService,
            CancellationTokenSource cts)
        {
            var predictionListeners = InitializePredictionListeners();
            var actions = lanaActionService.GetAll();
            var speechGrammarProvider = new SpeechGrammarProvider(actions);
            var _speechListener = new SpeechListener(
                predictionRaportingService,
                predictionListeners,
                speechGrammarProvider
            );
            return Task.Run(() => _speechListener.Run(cts));
        }

        private static IEnumerable<IPredictionListener> InitializePredictionListeners()
        {
            var result = new List<IPredictionListener>();

            //WIT.AI
            var witaiAuthToken = ConfigurationManager.AppSettings["WitAiAuthToken"];
            if (!witaiAuthToken.IsNullOrEmpty())
                result.Add(new WitAiPredictionListener(witaiAuthToken));

            return result;
        }
    }
}
