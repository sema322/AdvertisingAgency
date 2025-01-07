using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdvertisingAgency.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AdvertisingAgency.Controllers
{

    public class AdvRequestController : Controller
    {
        private readonly AdvertisingAgencyContext _context;

        public AdvRequestController(AdvertisingAgencyContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Index()
        {
            var applications = await _context.Advertisings
                .Where(a => a.IsCorrect == false)
                .Include(a => a.client)
                .Include(a => a.category)
                .ToListAsync();

            return View(applications);
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.categories = await _context.Categories.ToListAsync();

            var defaultCategoryId = categories.FirstOrDefault()?.Id;
            var defaultDateStart = DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.DefaultCategoryId = defaultCategoryId;
            ViewBag.DefaultDateStart = defaultDateStart;

            return View();
        }

        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<IActionResult> Create(Advertising advertising)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userIdClaim.Value);

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserID == userId);
            if (client == null)
            {
                ModelState.AddModelError("ClientId", "Клиент не найден. Обратитесь в службу поддержки.");
                ViewBag.categories = await _context.Categories.ToListAsync();
                return View(advertising);
            }

            if (advertising.DateStart < DateTime.Today)
            {
                ModelState.AddModelError("DateStart", "Дата начала не может быть в прошлом.");
                ViewBag.categories = await _context.Categories.ToListAsync();
                return View(advertising);
            }

            if (advertising.Duration < 1)
            {
                ModelState.AddModelError("Duration", "Количество дней должно быть больше или равно 1.");
                ViewBag.categories = await _context.Categories.ToListAsync();
                return View(advertising);
            }

            advertising.ClientId = client.Id;
            advertising.category = await _context.Categories.FindAsync(advertising.CategoryId);
            advertising.IsActive = false;
            advertising.IsCorrect = false;

            _context.Advertisings.Add(advertising);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var advertising = await _context.Advertisings
                .Include(a => a.client)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (advertising == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(advertising);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Advertising advertising)
        {
            var existingAdvertising = await _context.Advertisings
                .Include(a => a.client)
                .Include(a => a.category)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAdvertising == null)
            {
                return NotFound();
            }

            if (advertising.DateStart < DateTime.Today)
            {
                ModelState.AddModelError("DateStart", "Дата начала не может быть в прошлом.");
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(advertising);
            }

            if (advertising.Duration < 1)
            {
                ModelState.AddModelError("Duration", "Количество дней должно быть больше или равно 1.");
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                return View(advertising);
            }

            existingAdvertising.ProductName = advertising.ProductName;
            existingAdvertising.CategoryId = advertising.CategoryId;
            existingAdvertising.DateStart = advertising.DateStart;
            existingAdvertising.Duration = advertising.Duration;
            existingAdvertising.IsCorrect = true;

            existingAdvertising.category = await _context.Categories.FindAsync(advertising.CategoryId);

            _context.Update(existingAdvertising);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        private bool AdvertisingExists(int id)
        {
            return _context.Advertisings.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var advertising = await _context.Advertisings.FindAsync(id);
            if (advertising == null)
            {
                return NotFound();
            }

            _context.Advertisings.Remove(advertising);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Client")]
        public IActionResult Success()
        {
            return View();
        }

    }
}