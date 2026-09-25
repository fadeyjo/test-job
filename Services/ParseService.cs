using TestJob.DTOs.Elements;
using TestJob.Repositories;

namespace TestJob.Services;

public class ParseService(IElementsRepository repository) : IParseService
{
    public Task<ParseResponse> CreateElementAsync(ParseRequest body)
    {
        throw new NotImplementedException();
    }
}