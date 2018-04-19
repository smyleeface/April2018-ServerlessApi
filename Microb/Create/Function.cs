using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Microb.Create {

    public class CreateItem
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    class Function: MicrobFunction {
        
        //--- Methods ---
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public APIGatewayProxyResponse LambdaHandler(APIGatewayProxyRequest request) {
            LambdaLogger.Log(JsonConvert.SerializeObject(request));
            try
            {
                var requestJson = JsonConvert.DeserializeObject<CreateItem>(request.Body);                
                var id = CreateItem(requestJson.Title, requestJson.Content).Result;
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = id
                };
            }
            catch (Exception e) {
                LambdaLogger.Log($"*** ERROR: {e}");
                return new APIGatewayProxyResponse {
                    Body = e.Message,
                    StatusCode = 500
                };
            }
        }

        private async Task<string> CreateItem(string title, string content) {
            var id = Guid.NewGuid().ToString();
            var now = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
            var item = new Document();
            item["Id"] = id;
            item["Title"] = title;
            item["Content"] = content;
            item["DateCreated"] = now;
            await _table.PutItemAsync(item);
            LambdaLogger.Log($"*** INFO: Created item {id}");
            return id;
        }
    }
}
