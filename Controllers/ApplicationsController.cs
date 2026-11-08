using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.Controllers;

public class ApplicationsController: Controller {
    private readonly ApplicationDbContext _context;

    public ApplicationsController(ApplicationDbContext context){
        _context = context;
    }

    public async Task<IActionResult> Index() {
        var Applications = await _context.Applications.Include(a => a.Company).ToListAsync();   
        return View(Applications);

    }

    public async Task<IActionResult> Details(int? id) {
        if (id == null) return NotFound();

        var application = await _context.Applications.Include(a => a.Company).FirstOrDefaultAsync(m => m.Id == id);

        if (application == null) return NotFound();

        return View(application);
    }

    [HttpGet]
    public IActionResult Create(){
        ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Nome");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobApplication application) {
        if (ModelState.IsValid) {
            _context.Add(application);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Nome", application.CompanyId);
        return View(application);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id) {
        
        if (id == null) return NotFound();

        var application = await _context.Applications.FindAsync(id);
        if (application == null) return NotFound();

        ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Nome");

        return View(application);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, JobApplication application) {
        if (id != application.Id) return NotFound();

        if(ModelState.IsValid){
            _context.Update(application);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(application);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id){
        var application = await _context.Applications.FindAsync(id);
        if (application != null){
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    
    

}