using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Microb.Update {
    
    public class UpdateItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
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
            try {
                var requestJson = JsonConvert.DeserializeObject<UpdateItem>(request.Body);  
                UpdateItem(request.PathParameters["id"], requestJson.Title, requestJson.Content).Wait();
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = true.ToString()
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

        private async Task UpdateItem(string id, string title, string content) {
            var item = new Document();
            item["Id"] = id;
            item["Title"] = title;
            item["Content"] = content;
            await _table.UpdateItemAsync(item);
        }
    }
}
