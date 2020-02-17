using Lana.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lana.Core.Interfaces
{
    public interface IPredictionRaportingService
    {
        ObservableCollection<Prediction> PredictionsList { get; }
        void AddPredictions(IEnumerable<Prediction> predictions);
    }
}
