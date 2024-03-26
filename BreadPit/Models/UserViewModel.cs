namespace BreadPit.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public string SelectedRole { get; set; }
    }
}
