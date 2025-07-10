using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Companies.API.Extensions
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeatures = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeatures != null)
                    {
                        var problemDetailsFactory = app.Services.GetService<ProblemDetailsFactory>();
                        ArgumentNullException.ThrowIfNull(nameof(problemDetailsFactory));

                        var problemDetails = CreateProblemDetails(context, contextFeatures.Error, problemDetailsFactory, app);

                        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                });
            });
        }

        private static ProblemDetails CreateProblemDetails(HttpContext context, Exception error, ProblemDetailsFactory? problemDetailsFactory, WebApplication app)
        {
            return error switch
            {
                TournamentNotFoundException companyNotFoundException => problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status404NotFound,
                    title: companyNotFoundException.Title,
                    detail: companyNotFoundException.Message,
                    instance: context.Request.Path),

                GameNotFoundException gameNotFoundException => problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status404NotFound,
                    title: gameNotFoundException.Title,
                    detail: gameNotFoundException.Message,
                    instance: context.Request.Path),

                GameTitleNotFoundException gameTitleNotFoundException => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status404NotFound,
                title: gameTitleNotFoundException.Title,
                detail: gameTitleNotFoundException.Message,
                instance: context.Request.Path),

                GameBadRequestException gameBadRequestException => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                title: gameBadRequestException.Title,
                detail: gameBadRequestException.Message,
                instance: context.Request.Path),

                TournamentBadRequestException tournamentBadRequestException => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                title: tournamentBadRequestException.Title,
                detail: tournamentBadRequestException.Message,
                instance: context.Request.Path),

                _ => problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error",
                    detail: app.Environment.IsDevelopment() ? error.Message : "Un unexpected error occured."),
            };
        }
    }


}
