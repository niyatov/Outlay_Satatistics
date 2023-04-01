namespace outlay_mvc.Models;

public class GetShowUsersVM
{
    public bool IsAdmin { get; set; }
    public int CategoryId { get; set; }
    public List<GetShowUserVM> UsersVM { get; set; }
}


