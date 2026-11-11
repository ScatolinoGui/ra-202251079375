using System.Security.Claims;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaApi.Autorizacao;

public class DonoDoEmprestimoRequirement : IAuthorizationRequirement { }

public class DonoDoEmprestimoHandler : AuthorizationHandler<DonoDoEmprestimoRequirement, Emprestimo>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DonoDoEmprestimoRequirement requirement,
        Emprestimo emprestimo)
    {
        if (context.User.IsInRole("Bibliotecario"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var usuarioId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (usuarioId == emprestimo.UsuarioId.ToString())
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
