namespace Legiosoft_test_case.Services.Interfaces;

public interface ICsvReader<T>
{
	IEnumerable<T> Read(Stream stream);
}
