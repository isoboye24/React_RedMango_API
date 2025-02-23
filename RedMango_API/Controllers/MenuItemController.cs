using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.DTO;
using RedMango_API.Services;
using RedMango_API.Utility;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

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

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemUpdateDTO == null || id != menuItemUpdateDTO.MenuItemId)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        MenuItem menuItemFromDB = await _db.MenuItems.FindAsync(id);
                        if (menuItemFromDB == null)
                        {
                            return BadRequest();
                        }
                        else
                        {
                            menuItemFromDB.Name = menuItemUpdateDTO.Name;
                            menuItemFromDB.Price = menuItemUpdateDTO.Price;
                            menuItemFromDB.Category = menuItemUpdateDTO.Category;
                            menuItemFromDB.SpecialTag = menuItemUpdateDTO.SpecialTag;
                            menuItemFromDB.Description = menuItemUpdateDTO.Description;
                            if (menuItemUpdateDTO.File != null && menuItemUpdateDTO.File.Length >0)
                            {
                                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemUpdateDTO.File.FileName)}";
                                await _blobService.DeleteBlob(menuItemFromDB.Image.Split('/').Last(), SD.SD_Storage_Container);
                                menuItemFromDB.Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemUpdateDTO.File);
                            }
                        }                        
                        _db.MenuItems.Add(menuItemFromDB);
                        _db.SaveChanges();
                        _response.StatusCode = HttpStatusCode.NoContent;
                        return Ok(_response);
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                else
                {
                    MenuItem menuItemFromDB = await _db.MenuItems.FindAsync(id);
                    if (menuItemFromDB == null)
                    {
                        return BadRequest();
                    }
                    else
                    {                       
                        await _blobService.DeleteBlob(menuItemFromDB.Image.Split('/').Last(), SD.SD_Storage_Container);
                        int miliseconds = 2000;
                        Thread.Sleep(miliseconds);
                    }
                    _db.MenuItems.Remove(menuItemFromDB);
                    _db.SaveChanges();
                    _response.StatusCode = HttpStatusCode.NoContent;
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
