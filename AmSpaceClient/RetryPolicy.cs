using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public class RetryPolicy : IRetryPolicy
    {
        public HashSet<HttpStatusCode> ApplyToStatusCode { get; set; }
        public int InitialDelay { get; set; }
        public int Attemts { get; set; }

        public RetryPolicy()
        {
            ApplyToStatusCode = new HashSet<HttpStatusCode>();
        }

        public void AddApplicableStatusCode(HttpStatusCode code)
        {
            ApplyToStatusCode.Add(code);
        }

        public void RemoveApplicableStatusCode(HttpStatusCode code)
        {
            ApplyToStatusCode.Remove(code); ;
        }
    }
}
