using PMApi.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PMApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

            config.MessageHandlers.Add(new APIPipeline());
            var jsonFormatter = config.Formatters.JsonFormatter;
            config.Formatters.Remove(jsonFormatter);
            config.Formatters.Insert(0, jsonFormatter);
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
