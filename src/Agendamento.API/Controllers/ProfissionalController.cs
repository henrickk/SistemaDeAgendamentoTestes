using Microsoft.AspNetCore.Mvc;

namespace Agendamento.API.Controllers;
public class ProfissionalController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
