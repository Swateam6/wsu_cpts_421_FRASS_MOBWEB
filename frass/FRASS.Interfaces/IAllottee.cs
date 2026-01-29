namespace FRASS.Interfaces
{
	public interface IAllottee
	{
		int ParcelID { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string AllotmentNumber { get; set; }
		int AllotteeNumber { get; set; }
		string ParcelNumber { get; set; }
		string Township { get; set; }
		string Range { get; set; }
		string Section { get; set; }
		decimal Share { get; set; }
		decimal Acres { get; set; }
	}
}
