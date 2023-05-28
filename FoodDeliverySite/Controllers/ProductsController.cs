using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodDeliverySite.Data;
using FoodDeliverySite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;

namespace FoodDeliverySite.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductsContext _context;
        private readonly ShopCartsContext _spcontext;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(ProductsContext context, ShopCartsContext context1, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _spcontext = context1;
            _userManager = userManager;
        }


//------Отображение Панели с товарами для админа------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
              return _context.Products != null ? 
                          View(await _context.Products.ToListAsync()) :
                          Problem("Entity set 'ProductsContext.Products'  is null.");
        }

//------Отображение панели с товарами для пользователя------------------------------------------------------------------
        public async Task<IActionResult> UserIndex()
        {
            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'ProductsContext.Products'  is null.");
        }

//------Обработка нажатия на кнопку покупки товара----------------------------------------------------------------------
//------Сюда приходит на вход Id пользователя и информация о товаре переносится в БД корзины----------------------------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserIndex(int productId)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == productId);
            var user = await _userManager.GetUserAsync(User);

            if (product != null)
            {

                bool exists = _spcontext.ShopCarts.Any(p => p.ProductId == productId && p.UserId == user.Id);

                // Проверка, существует ли у уже у пользователя этот товар
                if (exists) 
                { 
                    var productcart = _spcontext.ShopCarts.SingleOrDefault(p => p.ProductId == productId && p.UserId == user.Id);
                    productcart.Count++;
                    productcart.Price += productcart.Price;
                    _spcontext.Update(productcart);
                    await _spcontext.SaveChangesAsync();
                }
                else
                {
                    // Создать новый объект для записи во вторую базу данных
                    var targetProduct = new ShopCart
                    {
                        UserId = user.Id,
                        ProductId = product.Id,
                        Name = product.Name,
                        Category = product.Category,
                        Price = product.Price,
                        Count = 1,
                        ImagePath = product.ImagePath
                    };
                    // Добавить объект во вторую базу данных
                    _spcontext.Add(targetProduct);
                    await _spcontext.SaveChangesAsync();
                }
                product.Count--;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }

            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'ProductsContext.Products'  is null.");
        }

//------Отображение страницы с корзиной. Доступно только авторизованным пользователям-----------------------------------
        [Authorize]
        public async Task<IActionResult> ShopCart()
        {
            var user = await _userManager.GetUserAsync(User);
            return _spcontext.ShopCarts != null ?
                        View(await _spcontext.ShopCarts.ToListAsync()) :
                        Problem("Entity set 'ProductsContext.Products'  is null.");
        }

//------Обработка покупки. После нажатия на кнопку. После нажатия, из корзины удаляется данный лот----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int shopcartId)
        {
            var product = await _spcontext.ShopCarts.FindAsync(shopcartId);

            if (product != null)
            {
                _spcontext.ShopCarts.Remove(product);
            }

            await _spcontext.SaveChangesAsync();
            return RedirectToAction(nameof(UserIndex));
        }

//------Обработка покупки. После нажатия на кнопку. После нажатия, из корзины удаляется данный лот----------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyAll(string userId)
        {
            List<ShopCart> product = await _spcontext.ShopCarts.Where(p => p.UserId == userId).ToListAsync();

            int i = 0;
            while (product.Count > 0)
            {
                _spcontext.ShopCarts.Remove(product[0]);
                product.RemoveAt(0);
            }

            await _spcontext.SaveChangesAsync();
            return RedirectToAction(nameof(UserIndex));
        }


//------Отображение страницы с деталями о товаре------------------------------------------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

//------Отображение страницы для добавления товара в меню---------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

//------Обработка нажатия на кнопку создания товара на странице создания товара-----------------------------------------
//------Принимает в себя все поля, заполняет всё по форме Model и добавляет в базу данных товаров----------------------- 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,Price,Count,ImagePath")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

//------Отображение страницы изменения товара в меню--------------------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

//------Обработка нажатия на кнопку изменения товара на странице изменения товара---------------------------------------
//------Вновь принимает в себя все поля, заполняет всё по форме Model и обновляет базу данных товаров-------------------        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Price,Count,ImagePath")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

//------Отображение страницы удаления товара----------------------------------------------------------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

//------Обработка удаления----------------------------------------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ProductsContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
