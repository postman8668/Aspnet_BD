namespace KursDB.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? LibraryId { get; set; }
        public string LibraryName { get; set; }
        public IList<string> Roles { get; set; }
    }
}