using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using Stripe;
using System.Net;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDBContext _db;
        private ApiResponse _response;
        private readonly IConfiguration _configuration;
        public PaymentController(AppDBContext db, IConfiguration configuration)
        {
            _db = db;
            _response = new();
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userId)
        {
            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(x => x.CartItems).ThenInclude(x => x.MenuItem).FirstOrDefault(x => x.UserId == userId);
            if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count() == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;               
                return Ok(_response);
            }
            else
            {
                #region Create Payment Intent

                StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
                shoppingCart.CartTotal = shoppingCart.CartItems.Sum(x => x.MenuItem.Price * x.Quantity);

                PaymentIntentCreateOptions options = new()
                {
                    Amount = (int)(shoppingCart.CartTotal * 100),
                    Currency = "eur",
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                };
                PaymentIntentService service = new();
                PaymentIntent response = service.Create(options);
                shoppingCart.StripePaymentIntentId = response.Id;
                shoppingCart.ClientSecret = response.ClientSecret;
                #endregion

                _response.Result = shoppingCart;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }
    }
}
