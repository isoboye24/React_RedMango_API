using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.DTO;
using RedMango_API.Services;
using RedMango_API.Utility;
using System.Net;

namespace RedMango_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDBContext _db;
        private ApiResponse _response;
        public OrderController(AppDBContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string? userId)
        {
            try
            {
                var orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetails).ThenInclude(x => x.MenuItem).OrderByDescending(x => x.OrderHeaderId);
                if (!string.IsNullOrEmpty(userId))
                {
                    _response.Result = orderHeaders.Where(x => x.ApplicationUserId == userId);
                }
                else
                {
                    _response.Result = orderHeaders;
                }
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {                
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse>> GetOrders(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                else
                {
                    var orderHeaders = _db.OrderHeaders.Include(x => x.OrderDetails).ThenInclude(x => x.MenuItem).Where(x => x.OrderHeaderId == id);
                    if (orderHeaders == null)
                    {
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(_response);
                    }
                    else
                    {
                        _response.Result = orderHeaders;
                        _response.StatusCode = HttpStatusCode.OK;
                        return Ok(_response);
                    }                    
                }               
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            try
            {
                OrderHeader order = new()
                {
                    ApplicationUserId = orderHeaderDTO.ApplicationUserId,
                    PickUpEmail = orderHeaderDTO.PickUpEmail,
                    PickUpName = orderHeaderDTO.PickUpName,
                    PickUpPhoneNumber = orderHeaderDTO.PickUpPhoneNumber,
                    OrderTotal = orderHeaderDTO.OrderTotal,
                    OrderDate = DateTime.Now,
                    StripePaymentIntendId = orderHeaderDTO.StripePaymentIntendId,
                    TotalItems = orderHeaderDTO.TotalItems,
                    Status = String.IsNullOrEmpty(orderHeaderDTO.Status)? SD.status_pending : orderHeaderDTO.Status,
                };
                if (ModelState.IsValid)
                {
                    _db.OrderHeaders.Add(order);
                    _db.SaveChanges();
                    foreach (var orderDetailDTO in orderHeaderDTO.OrderDetailDTO)
                    {
                        OrderDetail orderDetail = new()
                        {
                            OrderHeaderId = order.OrderHeaderId,
                            ItemName = orderDetailDTO.ItemName,
                            MenuItemId = orderDetailDTO.MenuItemId,
                            Price = orderDetailDTO.Price,
                            Quantity = orderDetailDTO.Quantity,
                        };
                        _db.OrderDetails.Add(orderDetail);
                    }
                    _db.SaveChanges();
                    _response.Result = order;
                    order.OrderDetails = null;
                    _response.StatusCode = HttpStatusCode.Created;
                    return Ok(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrderHeader(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            try
            {
                if (orderHeaderUpdateDTO == null || id != orderHeaderUpdateDTO.OrderHeaderId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                OrderHeader orderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == id);
                if (orderFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                else
                {
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickUpName))
                    {
                        orderFromDb.PickUpName = orderHeaderUpdateDTO.PickUpName;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickUpPhoneNumber))
                    {
                        orderFromDb.PickUpPhoneNumber = orderHeaderUpdateDTO.PickUpPhoneNumber;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickUpEmail))
                    {
                        orderFromDb.PickUpEmail = orderHeaderUpdateDTO.PickUpEmail;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.Status))
                    {
                        orderFromDb.Status = orderHeaderUpdateDTO.Status;
                    }
                    if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.StripePaymentIntendId))
                    {
                        orderFromDb.StripePaymentIntendId = orderHeaderUpdateDTO.StripePaymentIntendId;
                    }
                    _db.SaveChanges();
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
    }
}
