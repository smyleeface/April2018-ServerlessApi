﻿using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace Microb.Read {
    
    class Function: MicrobFunction {
        
        //--- Methods ---
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public APIGatewayProxyResponse LambdaHandler(APIGatewayProxyRequest request) {
            LambdaLogger.Log(JsonConvert.SerializeObject(request));
            try
            {
                var id = request.PathParameters["id"];
                return new APIGatewayProxyResponse {
                    StatusCode = 200,
                    Body = JsonConvert.SerializeObject(GetItem(id).Result)
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

        private async Task<MicrobItem> GetItem(string id) {
            var response = await _table.GetItemAsync(id);
            return new MicrobItem {
                id = response["Id"],
                title = response["Title"],
                content = response["Content"],
                date = response["DateCreated"]
            };
        }
    }
}
