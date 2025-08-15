using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // Example authentication logic
        if (Email == "admin@example.com" && Password == "pass123")
        {
            return RedirectToPage("/Index");
        }

        ErrorMessage = "Invalid email or password.";
        return Page();
    }
}
