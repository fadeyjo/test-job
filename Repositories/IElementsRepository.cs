using TestJob.Models;

namespace TestJob.Repositories;

public interface IElementsRepository
{
    public Task<long> CreateElementAsync(string attrValue, string elHtmlCode);
}