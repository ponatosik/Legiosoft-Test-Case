using System.Data;

namespace Legiosoft_test_case.Data;

public interface IDbConnectionFactory
{
	public IDbConnection CreateConnection();
}
