#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ForApp.Data;
using ForApp.Models;
using Microsoft.AspNetCore.Identity;
using ForApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace ForApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        //list order for seller
        public async Task<IActionResult> Index()
        {
            var orderContext = _context.Order.Include(o => o.User);
            return View(await orderContext.ToListAsync());
        }
        //order history for customer
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> OrderHistory()
        {
            var userContext = _context.Order.Include(o => o.User);
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            return View(_context.Order.Where(c => c.UId == thisUserId));
        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
