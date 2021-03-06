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
    public class BooksController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly int _recordsPerPage = 20;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BooksController(UserContext context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Books
        //list book we can sort
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> List(string sortOrder, string searchString)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;
            var userContext = from s in _context.Book.Include(b => b.Store)
                              select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                userContext = userContext.Where(s => s.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "title_desc":
                    userContext = userContext.OrderByDescending(s => s.Title);
                    break;
                case "Price":
                    userContext = userContext.OrderByDescending(s => s.Price);
                    break;
                default:
                    userContext = userContext.OrderBy(s => s.Title);
                    break;
            }
            return View(await userContext.AsNoTracking().ToListAsync()); ;
        }


        // books available in your store
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> StoreBook(Book book, string sortOrder, string searchString)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;
            var thisUserId = _userManager.GetUserId(HttpContext.User);
            Store thisStore = await _context.Store.FirstOrDefaultAsync(s => s.UId == thisUserId);
            book.StoreId = thisStore.Id;
            var userContext = from s in _context.Book.Include(b => b.Store)
                              select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                userContext = userContext.Where(s => s.Title.Contains(searchString));
                return View(await userContext.AsNoTracking().ToListAsync());
            }
            switch (sortOrder)
            {
                case "title_desc":
                    userContext = userContext.OrderByDescending(s => s.Title);
                    break;
                case "price_desc":
                    userContext = userContext.OrderByDescending(s => s.Price);
                    break;
                default:
                    userContext = userContext.OrderBy(s => s.Title);
                    break;
            }
            return View(await _context.Book.Where(c => c.StoreId == book.StoreId).ToListAsync());
        }
        // list book we can add to cart
        public async Task<IActionResult> Index(string sortOrder, string searchString, int id = 0)
        {
            var userContext = from s in _context.Book.Include(b => b.Store)
                              select s; ;
            int numberOfRecords = await _context.Book.CountAsync();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _recordsPerPage);
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            switch (sortOrder)
            {
                case "price_desc":
                    userContext = userContext.OrderByDescending(s => s.Price);
                    return View(await userContext.AsNoTracking().ToListAsync());
                    break;
                default:
                    userContext = userContext.OrderBy(s => s.Price);
                    return View(await userContext.AsNoTracking().ToListAsync());
                    break;
            }
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            ViewData["CurrentFilter"] = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                userContext = userContext.Where(s => s.Title.Contains(searchString));
                return View(await userContext.AsNoTracking().ToListAsync());
            }
            List<Book> books = await _context.Book
              .Skip(id * _recordsPerPage)  //Offset SQL
              .Take(_recordsPerPage)//Top SQL
              .ToListAsync();
            return View(books);
            /*return View(await userContext.ToListAsync());*/
        }


        // GET: Books/Details/5
        // see the detail of your book
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Seller")]
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,Category,Price,Desc")] Book book, IFormFile image)
        {
            if (image != null)
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = "/img/" + imgName;

                var thisUserId = _userManager.GetUserId(HttpContext.User);
                Store thisStore = await _context.Store.FirstOrDefaultAsync(s => s.UId == thisUserId);
                book.StoreId = thisStore.Id;
            }
            else
            {
                return View(book);
            }
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Pages,Author,Category,Price,Desc,ImgUrl")] Book book, IFormFile image)
        {
            if (id != book.Isbn)
            {
                return NotFound();
            }
            var thisUserId = _userManager.GetUserId(HttpContext.User);
            Store thisStore = await _context.Store.FirstOrDefaultAsync(s => s.UId == thisUserId);
            book.StoreId = thisStore.Id;
            if (ModelState.IsValid)
            {
                try
                {
                    {
                        if (image != null)
                        {
                            string imgName = book.Isbn + Path.GetExtension(image.FileName);
                            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                            using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }
                            book.ImgUrl = "/img/" + imgName;
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("StoreBook", "Books");
            }

            return View(book);
        }
        // GET: Books/Delete/5
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Book.FindAsync(id);

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("StoreBook", "Books");
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.Isbn == id);
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToCart(string isbn)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            Cart myCart = new Cart() { UId = thisUserId, BookIsbn = isbn, Quantity = 1 };
            Cart fromDb = _context.Cart.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
            //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
            if (fromDb == null)
            {
                _context.Add(myCart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");


        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = myDetailsInCart.Select(c => c.Book.Price*c.Quantity)
                        .Aggregate((c1, c2) => c1 + c2);
                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = item.Quantity,
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _context.Cart.RemoveRange(myDetailsInCart);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }
            return RedirectToAction("OrderHistory", "Orders");
        }
    }
    }
