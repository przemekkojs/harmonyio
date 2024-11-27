using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Main.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private const string DEFAULT_ERROR_MESSAGE = "Podczas przetwarzania twojego zapytania wystąpił błąd. Strona do której się odwołujesz nie istnieje lub nie masz do niej dostępu.";

        public string ErrorMessage { get; set; } = null!;

        public string? RequestId { get; set; }

        public void OnGet(string? errorMessage)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            ErrorMessage = errorMessage ?? DEFAULT_ERROR_MESSAGE;
        }
    }

}
