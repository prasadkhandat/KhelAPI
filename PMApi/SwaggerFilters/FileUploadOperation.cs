using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace PMApi.SwaggerFilters
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.operationId.ToLower() == "upload_post")
            {
                operation.consumes.Add("application/form-data");
                if (operation.parameters != null && operation.parameters.Count > 0)
                {
                    operation.parameters.Add(new Parameter
                    {
                        name = "file",
                        @in = "formData",
                        required = true,
                        type = "file"
                    });
                }
                else
                {
                    operation.parameters = new[]
                    {
                        new Parameter
                        {
                            name = "file",
                            @in = "formData",
                            required = true,
                            type = "file"
                        }
                    };
                }
            }
        }
    }
}