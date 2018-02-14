using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace PMApi.SwaggerFilters
{
    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation == null) return;

            var filterPipeline = apiDescription.ActionDescriptor.GetFilterPipeline();
            var isAuthorized = filterPipeline.Select(x => x.Instance).Any(y => y is AuthorizeAttribute);
            //var allowAnonymous = filterPipeline.Select(fi => fi).Any(filter => filter.Instance is IAllowAnonymousFilter);

            if (isAuthorized)
            {
                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }

                var parameter = new Parameter
                {
                    description = "The authorization token",
                    @in = "header",
                    name = "Authorization",
                    @default = "bearer ",
                    required = true,
                    type = "string"
                };

                operation.parameters.Add(parameter);
            }
        }
    }
}