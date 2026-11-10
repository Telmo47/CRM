using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CRM.Models;
using CRM.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers;

public class HomeController : Controller
{

    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context){
        _context = context;
    }

    public async Task<IActionResult> Index() {
        ViewBag.TotalEmpresas = await _context.Companies.CountAsync();
        ViewBag.TotalCandidaturas = await _context.Applications.CountAsync();

        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
