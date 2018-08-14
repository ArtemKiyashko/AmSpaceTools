using AmSpaceModels;
using Newtonsoft.Json;
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
            try
            {
                if (!response.IsSuccessStatusCode)
                    response.TryConverToAmSpaceError(resultContent);
                return JsonConvert.DeserializeObject<TOutput>(resultContent);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<bool> ValidateAsync(this HttpResponseMessage response)
        {
            var resultContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                response.TryConverToAmSpaceError(resultContent);
            return true;
        }

        private static void TryConverToAmSpaceError(this HttpResponseMessage response, string resultContent)
        {
            try
            {
                var error = JsonConvert.DeserializeObject<AmSpaceError>(resultContent);
                throw new Exception(
                        error.ErrorDescription ??
                        error.Details ??
                        error.MissingFields);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
