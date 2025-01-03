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
    [Authorize(Roles = "Admin, Manager")]
    public class AdvertisingController : Controller
    {
        private readonly AdvertisingAgencyContext _context;

        public AdvertisingController(AdvertisingAgencyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applications = await _context.Advertisings
                .Where(a => a.IsCorrect == true)
                .Include(a => a.client)
                .Include(a => a.category)
                .ToListAsync();

            return View(applications);
        }

        private bool AdvertisingExists(int id)
        {
            return _context.Advertisings.Any(e => e.Id == id);
        }

        public async Task UpdateAdvertisingStatuses()
        {
            var currentDate = DateTime.Now;

            // Получаем все заявки
            var advertisings = await _context.Advertisings.ToListAsync();

            foreach (var advertising in advertisings)
            {
                // Проверяем, активна ли заявка
                if (advertising.DateStart <= currentDate && advertising.DateStart.AddDays(advertising.Duration) > currentDate)
                {
                    advertising.IsActive = true;
                }
                else
                {
                    advertising.IsActive = false;
                }

                // Удаляем заявку, если время истекло
                if (advertising.DateStart.AddDays(advertising.Duration) <= currentDate)
                {
                    _context.Advertisings.Remove(advertising);
                }
            }

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();
        }

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


    }
}