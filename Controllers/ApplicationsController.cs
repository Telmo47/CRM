using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Controllers;

[Authorize]
public class ApplicationsController: Controller {
    private readonly ApplicationDbContext _context;

    public ApplicationsController(ApplicationDbContext context){
        _context = context;
    }

    public async Task<IActionResult> Index(int? companyId, ApplicationStatus? estado, string? searchString) {
        var query = _context.Applications.Include(v => v.Company).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchString))    {
        query = query.Where(a => a.Titulo.ToLower().Contains(searchString.ToLower()));        }

    if (companyId.HasValue)
    {
        query = query.Where(a => a.CompanyId == companyId.Value);
    }

    if (estado.HasValue)
    {
        query = query.Where(a => a.Estado == estado.Value);
    }

        ViewBag.CompanyId = new SelectList(_context.Companies.OrderBy(c => c.Nome), "Id", "Nome", companyId);
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentEstado = estado;
       
        var applications = await query.ToListAsync();
        return View(applications);

    }

    public async Task<IActionResult> Details(int? id) {
        if (id == null) return NotFound();

        var application = await _context.Applications.Include(a => a.Company).FirstOrDefaultAsync(m => m.Id == id);

        if (application == null) return NotFound();

        return View(application);
    }

    [HttpGet]
    public IActionResult Create(){
        ViewBag.CompanyId = new SelectList(_context.Companies.OrderBy(c => c.Nome), "Id", "Nome");
        return View();
    }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(JobApplication application) {
    if (ModelState.IsValid) {
        application.DataCriacao = application.DataCriacao.ToUniversalTime();
        application.DataAtualizacao = application.DataAtualizacao.ToUniversalTime();
        application.DataExpiracao = application.DataExpiracao.ToUniversalTime();

        _context.Add(application);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
        ViewBag.CompanyId = new SelectList(_context.Companies.OrderBy(c => c.Nome), "Id", "Nome", application.CompanyId);
        return View(application);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id) {
        
        if (id == null) return NotFound();

        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        ViewBag.CompanyId = new SelectList(_context.Companies.OrderBy(c => c.Nome), "Id", "Nome");

        return View(application);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, JobApplication application) {
        if (id != application.Id) return NotFound();

        if (ModelState.IsValid) {
            application.DataCriacao = application.DataCriacao.ToUniversalTime();
            application.DataAtualizacao = application.DataAtualizacao.ToUniversalTime();
            application.DataExpiracao = application.DataExpiracao.ToUniversalTime();

            _context.Update(application);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CompanyId = new SelectList(_context.Companies.OrderBy(c => c.Nome), "Id", "Nome", application.CompanyId);
        return View(application);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id){
        var application = await _context.Applications.FindAsync(id);
        if (application != null){
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    
    

}