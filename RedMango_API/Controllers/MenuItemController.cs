﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedMango_API.Data;
using RedMango_API.Models;
using System.Net;

namespace RedMango_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly AppDBContext _db;
        private ApiResponse _response;
        public MenuItemController(AppDBContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }
        [HttpGet]
        public async Task<IActionResult>GetmenuItems()
        {
            _response.Result = _db.MenuItems;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
