using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formicae.Templates
{
    public enum RequestState
    {
        Off,
        Idle,
        Requesting,
        Done,
        Error
    };
}