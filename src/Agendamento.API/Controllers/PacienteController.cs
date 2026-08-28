using Microsoft.AspNetCore.Mvc;

namespace Agendamento.API.Controllers;
public class PacienteController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
