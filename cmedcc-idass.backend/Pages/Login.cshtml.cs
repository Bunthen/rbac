using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
namespace cmedcc_idass.backend.Pages;


public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine("Call end");
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please enter valid data.";
            return Page();
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("http://localhost:5161/cmedcc-dass-backend/idass/auth/login", new
        {
            email = Email,
            password = Password
        });
        Console.WriteLine(response.IsSuccessStatusCode);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result != null && result.IsSuccess)
            {
                // Save token in session or cookie
                HttpContext.Session.SetString("AuthToken", result.Token);
                return RedirectToPage("/");
            }
            else
            {
                ErrorMessage = "Invalid email or password";
                return Page();
            }
        }

        ErrorMessage = "Server error, try again later.";
        return Page();
    }

    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
    }
}
