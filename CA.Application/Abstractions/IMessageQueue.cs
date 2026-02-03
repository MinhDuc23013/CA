using CA.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Abstractions
{
    public interface IMessageQueue
    {
        Task PublishAsync<T>(
            string topic,
            T message,
            CancellationToken cancellationToken = default)
            where T : class;
    }


}
