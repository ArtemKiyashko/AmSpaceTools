﻿using AmSpaceModels;
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
            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<AmSpaceError>(resultContent);
                throw new Exception(error.ErrorDescription);
            }
            try
            {
                return JsonConvert.DeserializeObject<TOutput>(resultContent);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static async Task<bool> ValidateAsync(this HttpResponseMessage response)
        {
            
            if (!response.IsSuccessStatusCode)
            {
                var resultContent = await response.Content.ReadAsStringAsync();
                var error = new AmSpaceError();
                try
                {
                    error = JsonConvert.DeserializeObject<AmSpaceError>(resultContent);
                }
                finally
                {
                    throw new Exception(
                    error.ErrorDescription ??
                    error.Details ??
                    error.MissingFields
                    );
                }
            }
            return true;
        }
    }
}
