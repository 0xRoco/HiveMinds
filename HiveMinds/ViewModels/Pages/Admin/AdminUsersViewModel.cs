using HiveMinds.Models;

namespace HiveMinds.ViewModels.Pages.Admin;

public class AdminUsersViewModel
{
    public List<Account> Users { get; set; } = new();
}