using Lana.Domain.Predictions;
using Lana.Domain.Predictions.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lana.App.Predictions
{
    public class WPFPredictionRaportingService : IPredictionRaportingService
    {
        private readonly SynchronizationContext _synchronizationContext;

        public WPFPredictionRaportingService(SynchronizationContext synchronizationContext)
        {
            this._synchronizationContext = synchronizationContext;
        }

        public ObservableCollection<Prediction> PredictionsList { get; } = new ObservableCollection<Prediction>();

        public void AddPredictions(IEnumerable<Prediction> predictions)
        {
            if (predictions == null) throw new ArgumentNullException(nameof(predictions));

            foreach (var prediction in predictions)
                _synchronizationContext.Send(x => PredictionsList.Add(prediction), null);
        }
    }
}
