using System.Data;

namespace TestJob.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}