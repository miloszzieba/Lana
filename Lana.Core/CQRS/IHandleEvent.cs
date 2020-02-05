using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lana.Core.CQRS
{
    public interface IHandleEvent<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent eventInstance, CancellationToken cancellationToken);
    }
}
