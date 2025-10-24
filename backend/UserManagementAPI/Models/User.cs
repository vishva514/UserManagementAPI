namespace UserManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Designation { get; set; } 
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

}
