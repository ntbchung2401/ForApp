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

namespace ForApp.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly UserContext _context;

        public OrderDetailsController(UserContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var orderContext = _context.OrderDetail.Include(o => o.Book).Include(o => o.Order);
            return View(await orderContext.ToListAsync());
        }
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.OrderId == id);
        }
    }
}
