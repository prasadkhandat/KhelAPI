using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace PMApi.SwaggerFilters
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            Dictionary<string, Response> res = new Dictionary<string, Response>();
            res.Add("200", new Response() { description = "Success" });
            res.Add("401", new Response() { description = "Invalid username and password" });
            swaggerDoc.paths.Add("/v1/authenticate", new PathItem
            {

                post = new Operation
                {
                    responses = res,
                    tags = new List<string> { "Auth" },
                    consumes = new List<string>
                {
                    "application/x-www-form-urlencoded"
                },
                    parameters = new List<Parameter> {
                    new Parameter
                    {
                        type = "string",
                        name = "grant_type",
                        required = true,
                        @default="password",
                        @in = "formData"
                    },
                    new Parameter
                    {
                        type = "string",
                        name = "username",
                        required = false,
                        @in = "formData"
                    },
                    new Parameter
                    {
                        format="password",
                        type = "string",
                        name = "password",
                        required = false,
                        @in = "formData"
                    }
                    }
                }

            });
        }
    }
}