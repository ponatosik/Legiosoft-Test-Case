namespace Legiosoft_test_case.Data.Exceptions;

public class EntityAllreadyExistException<T> : Exception
{
	public string EntityKey { get; }

	public EntityAllreadyExistException(string entityKey)
		: base(GetMessage(entityKey))
	{
		EntityKey = entityKey;
	}

	public EntityAllreadyExistException(string entityKey, Exception innerException)
		: base(GetMessage(entityKey), innerException)
	{
		EntityKey = entityKey;
	}

	private static string GetMessage(string entityKey) =>
		$"Entity of type {typeof(T).Name} with id {entityKey} allready exists.";
}
