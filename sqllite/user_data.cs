using SQLite;

namespace MOBWEB_TEST.sqllite
{
    [Table("user_data")]
    public class user_data
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        // Notice how clean this is! 
        // If you ever want to add a Cruiser Name, Email, or Password to the user profile later, you will add those columns right here.
    }
}