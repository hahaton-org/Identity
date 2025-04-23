using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Pages.Account.Register;

public class ViewModel
{
    public List<SelectListItem> RolesList { get; set; }
}
