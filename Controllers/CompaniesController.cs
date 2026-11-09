using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;

namespace CRM.Controllers;

public class CompaniesController: Controller {
    private readonly ApplicationDbContext _context;

    public CompaniesController(ApplicationDbContext context){
        _context = context;
    }

    public async Task<IActionResult> Index() {
        var Companies = await _context.Companies.ToListAsync();    
        return View(Companies);

    }

    public async Task<IActionResult> Details(int? id) {
        if (id == null) return NotFound();

        var company = await _context.Companies.FirstOrDefaultAsync(m => m.Id == id);

        if (company == null) return NotFound();

        return View(company);
    }

    [HttpGet]
    public IActionResult Create(){
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Company Company) {
        if (ModelState.IsValid) {
            _context.Add(Company);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Empresa criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        return View(Company);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id) {
        if (id == null) return NotFound();

        var company = await _context.Companies.FindAsync(id);
        if (company == null) return NotFound();

        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Company company) {
        if (id != company.Id) return NotFound();

        if(ModelState.IsValid){
            _context.Update(company);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Empresa atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        return View(company);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id){
        var company = await _context.Companies.FindAsync(id);
        if (company != null){
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    
    

}