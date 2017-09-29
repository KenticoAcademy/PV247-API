using System;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Messaging.Api
{
    /// <summary>
    /// Replaces automatically generated fields for all of file properties with a file upload input.
    /// </summary>
    public class FileOperation : IOperationFilter
    {
        /// <summary>
        /// Applies the filter on selected Swagger UI request.
        /// </summary>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.Equals("apifilepost", StringComparison.OrdinalIgnoreCase))
            {
                operation.Parameters.Clear();
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Files",
                    In = "formData",
                    Description = "Upload Image",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("application/form-data");
            }
        }
    }
}
