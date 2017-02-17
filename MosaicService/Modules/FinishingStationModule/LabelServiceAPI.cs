using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace MosaicSample.FinishingStationModule
{
    public class Field
    {
        public Field(string fieldId, string fieldValue, string fieldType)
        {
            id = fieldId;
            value = fieldValue;
            type = fieldType;
        }

        public string id { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class LabelServiceApi
    {
        public string template { get; set; }
        public List<Field> fields { get; set; }
        public int widthInches { get; set; }
        public int heightInches { get; set; }
        public string missingFieldValuePolicy { get; set; }
        public List<string> renderingHints { get; set; }
        public string mediaType { get; set; }
    }

    public class LabelResponse
    {
        public Label label { get; set; }
    }

    public class Label
    {
        public string data { get; set; }
        public float widthInches { get; set; }
        public float heightInches { get; set; }
        public string mediatype { get; set; }
    }

    public static class LabelService
    {

        private const string _apiKey = "3fe6a27c-dc8b-40e5-b2d1-463f0c9b663a";

        public static string NewRequest(string template, List<Field> fields, ILogger logger)
        {
            var url = "https://stg.labelservice.shipping.cimpress.io/api/v1/labels";

            var jsonObj = new LabelServiceApi();
            jsonObj.template = template;
            jsonObj.fields = fields;
            jsonObj.heightInches = 5;
            jsonObj.widthInches = 3;
            jsonObj.missingFieldValuePolicy = "Error";
            jsonObj.mediaType = "application/x-zpl";

            var response= "";

            try
            {
                var json = JsonConvert.SerializeObject(jsonObj);

                using (var client = new WebClient())
                {
                    client.Headers.Add("x-api-key", _apiKey);
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    response = client.UploadString(url, "POST", json);
                }

            }
            catch (Exception e)
            {
                logger.Error($"Labelrequest failed: {e.Message}");
            }

            var result = JsonConvert.DeserializeObject<LabelResponse>(response);
            return result.label.data;
        }
    }
}
