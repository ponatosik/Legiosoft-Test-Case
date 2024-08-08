namespace Legiosoft_test_case.Models;

public class TransactionDTO
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public decimal Amount { get; set; }
	public Coordinates ClientLocation { get; set; }
	public DateTime LocalTime { get; set; }

	public TransactionDTO(
		string id, 
		string name, 
		string email, 
		decimal amount, 
		Coordinates clientLocation, 
		DateTime localTime)
	{
		Id = id;
		Name = name;
		Email = email;
		Amount = amount;
		ClientLocation = clientLocation;
		LocalTime = localTime;
	}

	public static TransactionDTO From (Transaction transaction)
	{
		return new TransactionDTO(
			transaction.Id,
			transaction.Name,
			transaction.Email,
			transaction.Amount,
			transaction.ClientLocation,
			transaction.LocalTime);
	}
}
