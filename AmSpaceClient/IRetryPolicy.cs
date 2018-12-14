using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public interface IRetryPolicy
    {
        HashSet<HttpStatusCode> ApplyToStatusCode { get; set; }
        int InitialDelay { get; set; }
        int Attemts { get; set; }
        void AddApplicableStatusCode(HttpStatusCode code);
        void RemoveApplicableStatusCode(HttpStatusCode code);
    }
}
