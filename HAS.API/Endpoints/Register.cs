using HAS.Application.Common.Interfaces;
using HAS.Application.User.Auth;

namespace HAS.API.Endpoints;

public static class Register
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterCommand command, IMediator mediator) =>
        {
            try
            {
                var result = await mediator.Send(command);
                return Results.Created($"/users/{result.UserId}", result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithTags("Auth")
        .AllowAnonymous();

        return app;
    }
}
