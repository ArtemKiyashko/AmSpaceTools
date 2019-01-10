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
            var resultContent = response.Content != null ? await response.Content.ReadAsStringAsync() : "";
            if (!response.IsSuccessStatusCode)
                await response.ConverToExceptionAndThrow(resultContent);
            return JsonConvert.DeserializeObject<TOutput>(resultContent);
        }

        public static async Task<bool> ValidateAsync(this HttpResponseMessage response)
        {
            var resultContent = response.Content != null ? await response.Content.ReadAsStringAsync() : "";
            if (!response.IsSuccessStatusCode)
                await response.ConverToExceptionAndThrow(resultContent);
            return true;
        }

        private async static Task ConverToExceptionAndThrow(this HttpResponseMessage response, string resultContent)
        {
            var errorDescriptionBuilder = new StringBuilder();
            try
            {
                var resultObject = JRaw.Parse(resultContent);

                foreach (var jProperty in resultObject.Children())
                {
                    errorDescriptionBuilder
                        .Append(GetJTokenContent(jProperty));
                }
            }
            catch
            {
                errorDescriptionBuilder.Append($"{response.ReasonPhrase}.")
                                        .AppendLine()
                                        .Append(response.Content != null ? await response.Content.ReadAsStringAsync() : "");
            }
            throw new ArgumentException(errorDescriptionBuilder.ToString().TrimEnd());
        }
        
        private static string GetJTokenContent(JToken token)
        {
            if (token == null)
                return "";
            var valueBuilder = new StringBuilder();
            switch (token)
            {
                case JProperty property:
                    valueBuilder.Append($"{property.Name}: {GetJTokenContent(property.Value)}");
                    break;
                case JArray array:
                    valueBuilder.Append(Environment.NewLine);
                    foreach (var value in array)
                    {
                        valueBuilder.Append($"{GetJTokenContent(value)}");
                    }
                    break;
                case JObject obj:
                    foreach (var jProperty in obj.Children())
                    {
                        valueBuilder.Append($"{Environment.NewLine}{GetJTokenContent(jProperty)}");
                    }
                    break;
                default:
                    valueBuilder.Append($"{token.ToString()}{Environment.NewLine}");
                    break;
            }
            return valueBuilder.ToString();
        }

    }
}
