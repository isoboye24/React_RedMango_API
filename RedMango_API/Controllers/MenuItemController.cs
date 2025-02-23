using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.DTO;
using RedMango_API.Services;
using RedMango_API.Utility;
using System.Net;

namespace RedMango_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly AppDBContext _db;
        private ApiResponse _response;
        private readonly IBlobService _blobService;
        public MenuItemController(AppDBContext db, IBlobService blobService)
        {
            _db = db;
            _blobService = blobService;
            _response = new ApiResponse();
        }
        [HttpGet]
        public async Task<IActionResult>GetmenuItems()
        {
            _response.Result = _db.MenuItems;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        
        [HttpGet("{id:int}", Name ="GetMenuItem")]
        public async Task<IActionResult>GetmenuItem(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            else
            {
                MenuItem menuItem = _db.MenuItems.FirstOrDefault(x => x.MenuItemId == id);
                if (menuItem == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                else
                {                    
                    _response.Result = menuItem;
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
            }            
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm]MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDTO.File.FileName)}";
                        MenuItem menuItemToCreate = new()
                        {
                            Name = menuItemCreateDTO.Name,
                            Price = menuItemCreateDTO.Price,
                            Category = menuItemCreateDTO.Category,
                            SpecialTag = menuItemCreateDTO.SpecialTag,
                            Description = menuItemCreateDTO.Description,
                            Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemCreateDTO.File)
                        };
                        _db.MenuItems.Add(menuItemToCreate);
                        _db.SaveChanges();
                        _response.Result = menuItemToCreate;
                        _response.StatusCode = HttpStatusCode.Created;
                        return CreatedAtAction("GetMenuItem", new { id = menuItemToCreate.MenuItemId }, _response);
                    }
                }
                else
                {
                    _response.IsSuccess = false;
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
