using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace DotNetCoreAngularHMR_Spa.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; private set; } = "Hello!";

        public void OnGet()
        {
            Message += $" Server time is { DateTime.Now }";
        }
    }
}