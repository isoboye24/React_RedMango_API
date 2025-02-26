using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using System.Net;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly AppDBContext _db;
        public ShoppingCartController(AppDBContext db)
        {
            _db = db;
            _response = new();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantityBy)
        {
            // Shopping cart will have one entry per user id, even if a user has many items in cart.
            // Cart items will have all the items in shopping cart for a user
            // updatequantityby will have count by with an items quantity needs to be updated
            // if it is -1 that means we have lower a count if it is 5 it means we have to add 5 count to existing count.
            // if updatequantityby by is 0, item will be removed


            // when a user adds a new item to a new shopping cart for the first time
            // when a user adds a new item to an existing shopping cart (basically user has other items in cart)
            // when a user updates an existing item count
            // when a user removes an existing item

            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(x=>x.CartItems).FirstOrDefault(x => x.UserId == userId);
            MenuItem menuItem = _db.MenuItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
            if (menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            if (shoppingCart == null && updateQuantityBy > 0)
            {
                // create a shopping cart and add item to cart
                ShoppingCart newCart = new()
                {
                    UserId = userId
                };
                _db.ShoppingCarts.Add(newCart);
                _db.SaveChanges();

                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                    ShoppingCartId = newCart.ShoppingCartId,
                    MenuItem = null
                };
                _db.CartItems.Add(newCartItem);
                _db.SaveChanges();
            }
            else
            {
                // Shopping cart exists
                CartItem cartItemInCart = shoppingCart.CartItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
                if (cartItemInCart == null) 
                {
                    // item does not exist in cart
                    CartItem newCartItem = new()
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantityBy,
                        ShoppingCartId = shoppingCart.ShoppingCartId,
                        MenuItem = null
                    };
                    _db.CartItems.Add(newCartItem);
                    _db.SaveChanges();
                }
                else
                {
                    // item exists in cart
                    int newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                    if (updateQuantityBy == 0 || newQuantity <= 0)
                    {
                        // remove item from cart and if it is the last item in cart, remove cart as well
                        _db.CartItems.Remove(cartItemInCart);
                        if (shoppingCart.CartItems.Count() == 1)
                        {
                            _db.CartItems.Remove(cartItemInCart);
                        }
                        _db.SaveChanges();
                    }
                    else
                    {
                        cartItemInCart.Quantity = newQuantity;
                        _db.SaveChanges();
                    }
                }
            }
            return Ok(_response);
        }
    }
}
