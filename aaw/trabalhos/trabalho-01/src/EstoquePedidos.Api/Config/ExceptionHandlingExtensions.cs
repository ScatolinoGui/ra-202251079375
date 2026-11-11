using EstoquePedidos.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace EstoquePedidos.Api.Config;

public static class ExceptionHandlingExtensions
{
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                var (statusCode, title, type, detail) = exception switch
                {
                    ArgumentException ex => (
                        StatusCodes.Status400BadRequest,
                        "Dados de entrada invalidos",
                        "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        ex.Message),
                    UnauthorizedAccessException ex => (
                        StatusCodes.Status401Unauthorized,
                        "Nao autenticado",
                        "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                        string.IsNullOrWhiteSpace(ex.Message)
                            ? "Credenciais invalidas ou ausentes."
                            : ex.Message),
                    NaoEncontradoException ex => (
                        StatusCodes.Status404NotFound,
                        "Recurso nao encontrado",
                        "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                        ex.Message),
                    ConflitoException ex => (
                        StatusCodes.Status409Conflict,
                        "Conflito de regra de negocio",
                        "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                        ex.Message),
                    _ => (
                        StatusCodes.Status500InternalServerError,
                        "Erro interno no servidor",
                        "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                        "Ocorreu um erro inesperado ao processar a requisicao.")
                };

                await Results.Problem(
                    title: title,
                    detail: detail,
                    statusCode: statusCode,
                    type: type,
                    instance: context.Request.Path)
                    .ExecuteAsync(context);
            });
        });
    }
}
