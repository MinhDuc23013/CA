using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Infrastructures.OutboxEvent
{
    public enum OutboxStatus
    {
        New,
        Processing,
        Published,
        Failed
    }
}
