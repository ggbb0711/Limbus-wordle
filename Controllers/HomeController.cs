

using Microsoft.AspNetCore.Mvc;
 

public class HomeController():Controller
{
    public async Task<IActionResult> Index()
    {
        return View();
    }
}