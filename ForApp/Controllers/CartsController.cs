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
using ForApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ForApp.Controllers
{
    public class CartsController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartsController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Update(Cart cart)
        {

            if (ModelState.IsValid)
            {
                _context.Cart.Update(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }
    
    // GET: Carts
    [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            var cart = _context.Cart
                .Include(x => x.Book)
                .Include(x => x.User)
                .Where(c => c.UId == thisUserId).ToList();
            return View(cart);
        }
    }
}
