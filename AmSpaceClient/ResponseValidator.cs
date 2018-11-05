using AmSpaceModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public static class ResponseValidator
    {
        public static async Task<TOutput> ValidateAsync<TOutput>(this HttpResponseMessage response) where TOutput : class
        {
            var resultContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                response.ConverToAmSpaceError(resultContent);
            return JsonConvert.DeserializeObject<TOutput>(resultContent);
        }

        public static async Task<bool> ValidateAsync(this HttpResponseMessage response)
        {
            var resultContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                response.ConverToAmSpaceError(resultContent);
            return true;
        }

        private static void ConverToAmSpaceError(this HttpResponseMessage response, string resultContent)
        {
            var error = JsonConvert.DeserializeObject<AmSpaceError>(resultContent);

            throw new ArgumentException(
                    error.ErrorDescription ??
                    error.Details ??
                    error.MissingFields ??
                    JToken.Parse(resultContent).ToString());
        }


    }
}
