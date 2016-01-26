using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFInterfaces
{
    public enum ExecutionResult: int 
    {
        ERR_NO_ERRORS = 0,
        ERR_STATE_QUIT =1,

        ERR_UNKNOWN = -1,

        ERR_INVALID_STATE = -101,

        ERR_BADFILE = -10001

    }
}
