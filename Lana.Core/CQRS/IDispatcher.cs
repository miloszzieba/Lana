using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lana.Core.CQRS
{
    public interface IDispatcher
    {
        Task HandleAsync<TCommand>(TCommand command, CancellationToken cancellationToken, IProgress<int> progress = null) where TCommand : ICommand;
        Task HandleAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
        Task<TResult> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}
