using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvertisingAgency.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertisingAgency.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class ClientController : Controller
    {
        private readonly AdvertisingAgencyContext _context;

        public ClientController(AdvertisingAgencyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync(); 
            return View(clients);
        }

    }
}