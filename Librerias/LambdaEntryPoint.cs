using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace Librerias
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// Librerias::Librerias.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint :
        // When using an ELB's Application Load Balancer as the event source change 
        // the base class to Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            RegisterResponseContentEncodingForContentType("image/png", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("image/jpeg", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("image/jpg", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("image/gif", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("image/apng", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("image/webp", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("image/x-icon", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("application/octet-stream", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ResponseContentEncoding.Base64);
            RegisterResponseContentEncodingForContentType("multipart/form-data", ResponseContentEncoding.Base64);

            builder
                .UseStartup<Startup>();
        }
    }
}
