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

            //TODO: parse amspace erros as json to view [property_name: value]
            //the problems is, that sometime amspace send erros like objects and sometime like arrays
            //maybe we need to create different error formaters for different responses and specify it when calling ResponseValidator
            //below realization for array
            //
            //
            //var errorDescriptionBuilder = new StringBuilder();
            //var resultObject = JObject.Parse(resultContent);

            //foreach(JProperty jProperty in resultObject.Properties())
            //{
            //    errorDescriptionBuilder
            //        .Append(jProperty.Name)
            //        .Append(": ");

            //    var valueBuilder = new StringBuilder();
            //    foreach (JValue v in (JArray)jProperty.Value)
            //        valueBuilder
            //            .Append(v.Value)
            //            .AppendLine();
            //    errorDescriptionBuilder.Append(valueBuilder);
            //}

            throw new ArgumentException(
                    error.ErrorDescription ??
                    error.Details ??
                    error.MissingFields ??
                    JToken.Parse(resultContent).ToString());
        }


    }
}
