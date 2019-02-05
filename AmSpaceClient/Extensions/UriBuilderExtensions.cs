using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

        /// <summary>
        /// Add query without converting its values to unicode as AmSpace search API does not allow to use unicode entities
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static UriBuilder AddQuery(this UriBuilder builder, string key, params string[] values)
        {
            NameValueCollection queryValues = HttpUtility.ParseQueryString(builder.Query);

            // Add missing values
            foreach (string value in values)
            {
                if (queryValues.GetValues(key) == null || !queryValues.GetValues(key).Contains(value))
                {
                    queryValues.Add(key, value);
                }
            }

            // Set query
            builder.Query = Uri.EscapeUriString(HttpUtility.UrlDecode(queryValues.ToString()));
            return builder;
        }
    }
}
