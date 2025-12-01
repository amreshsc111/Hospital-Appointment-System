using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HAS.API.Filters;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the endpoint has [AllowAnonymous] or .AllowAnonymous()
        var allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (allowAnonymous)
        {
            return; // No security requirement for anonymous endpoints
        }

        // Check if endpoint has [Authorize] or .RequireAuthorization()
        var hasAuthorize = context.MethodInfo.GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any();

        // For minimal APIs, check metadata
        if (!hasAuthorize && context.ApiDescription.ActionDescriptor.EndpointMetadata != null)
        {
            hasAuthorize = context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>()
                .Any();
        }

        if (hasAuthorize)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
    }
}
