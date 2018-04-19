using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Microb.List {
    
    class Function: MicrobFunction {
        
        //--- Methods ---
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public APIGatewayProxyResponse LambdaHandler(APIGatewayProxyRequest request) {
            LambdaLogger.Log(JsonConvert.SerializeObject(request));
            try {
                return new APIGatewayProxyResponse {
                    Body = JsonConvert.SerializeObject(GetItems().Result),
                    StatusCode = 200,
                    Headers = new Dictionary<string, string>()
                    {
                        { "Access-Control-Allow-Origin", "*" },
                        { "Access-Control-Allow-Credentials", "true" }
                    }
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

        private async Task<IEnumerable<MicrobItem>> GetItems() {
            var request = new ScanRequest {
                TableName = _tableName
            };
            var response = await _dynamoClient.ScanAsync(request);
            return response.Items.Select(x => new MicrobItem {
                id = x["Id"].S,
                title = x["Title"].S,
                content = x["Content"].S,
                date = x["DateCreated"].S
            });
        }
    }
}
