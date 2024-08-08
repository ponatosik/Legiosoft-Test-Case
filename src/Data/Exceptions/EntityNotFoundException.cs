namespace Legiosoft_test_case.Data.Exceptions;

public class EntityNotFoundException<T> : Exception
{
	public string EntityKey { get; }

	public EntityNotFoundException(string entityKey)
		: base(GetMessage(entityKey))
	{
		EntityKey = entityKey;
	}

	public EntityNotFoundException(string entityKey, Exception innerException)
		: base(GetMessage(entityKey), innerException)
	{
		EntityKey = entityKey;
	}

	private static string GetMessage(string entityKey) =>
		$"Entity of type {typeof(T).Name} with id {entityKey} was not found.";
}
