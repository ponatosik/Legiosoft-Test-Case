namespace Legiosoft_test_case.Services.Interfaces;

public interface IExcelWriter<T>
{
	Task WriteStream(IEnumerable<T> data, ref Stream stream);
	Task<byte[]> WriteBytes(IEnumerable<T> data); 
}
