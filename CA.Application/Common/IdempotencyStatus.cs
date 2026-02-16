using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.Common
{
    public enum IdempotencyStatus
    {
        PROCESSING,
        SUCCESS,
        FAILED
    }
}
