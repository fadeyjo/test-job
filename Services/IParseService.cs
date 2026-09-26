using FluentValidation.Results;
using TestJob.DTOs.Elements;

namespace TestJob.Services;

public interface IParseService
{
    Task<ParseResponse> CreateElementAsync(ParseRequest body, ValidationResult validationResult);
}