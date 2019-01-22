using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UriBuilderExtended;

namespace AmSpaceClient.Extensions
{
    public static partial class UriBuilderExtensions
    {
        public static UriBuilder AddQueryNotNull(this UriBuilder builder, string key, params object[] values)
        {
            var nonEmptyValues = values
                .Where(_ => _ != null)
                .Select(_ => _.ToString())
                .Where(_ => !string.IsNullOrEmpty(_))
                .ToArray();
            return nonEmptyValues.Any() ?
                builder.AddQuery(key, nonEmptyValues) :
                builder;
        }
    }
}
