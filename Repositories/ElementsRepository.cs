using Dapper;
using TestJob.Data;

namespace TestJob.Repositories;

public class ElementsRepository(IDbConnectionFactory connectionFactory) : IElementsRepository
{
    public async Task<long> CreateElementAsync(string? attrValue, string elHtmlCode)
    {
        const string query = """
                             insert into elements (attr_value, el_html_code)
                             values (@attr_value, @el_html_code)
                             returning id;
                             """;
        
        using var connection = connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<long>(
            query,
            new
            {
                attr_value = attrValue,
                el_html_code = elHtmlCode
            });
    }
}