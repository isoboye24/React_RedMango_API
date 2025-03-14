﻿using Microsoft.AspNetCore.Http;
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
        //private readonly IBlobService _blobService;
        public MenuItemController(AppDBContext db)
        {
            _db = db;
            //_blobService = blobService;
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
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            else
            {
                MenuItem menuItem = _db.MenuItems.FirstOrDefault(x => x.MenuItemId == id);
                if (menuItem == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
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
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    else
                    {
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDTO.File.FileName)}";
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                        using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await menuItemCreateDTO.File.CopyToAsync(fileStream);
                        }

                        MenuItem menuItemToCreate = new()
                        {
                            Name = menuItemCreateDTO.Name,
                            Price = menuItemCreateDTO.Price,
                            Category = menuItemCreateDTO.Category,
                            SpecialTag = menuItemCreateDTO.SpecialTag,
                            Description = menuItemCreateDTO.Description,
                            //Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemCreateDTO.File) // Azure account needs to be subscribed
                            Image = $"/images/{fileName}"
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
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    else
                    {
                        MenuItem menuItemFromDB = await _db.MenuItems.FindAsync(id);
                        if (menuItemFromDB == null)
                        {
                            _response.StatusCode = HttpStatusCode.BadRequest;
                            _response.IsSuccess = false;
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
                                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                                if (!string.IsNullOrEmpty(menuItemFromDB.Image))
                                {
                                    string oldImagePath = Path.Combine(wwwRootPath, menuItemFromDB.Image.TrimStart('/'));
                                    if (System.IO.File.Exists(oldImagePath))
                                    {
                                        System.IO.File.Delete(oldImagePath);
                                    }
                                }
                                string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemUpdateDTO.File.FileName)}";
                                // Azure account needs to be subscribed
                                //await _blobService.DeleteBlob(menuItemFromDB.Image.Split('/').Last(), SD.SD_Storage_Container);
                                //menuItemFromDB.Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemUpdateDTO.File);

                                string uploadPath = Path.Combine(wwwRootPath, "images", newFileName);
                                using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                                {
                                    await menuItemUpdateDTO.File.CopyToAsync(fileStream);
                                }
                                menuItemFromDB.Image = $"/images/{newFileName}";
                            }
                        }                        
                        _db.MenuItems.Update(menuItemFromDB);
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
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                else
                {
                    MenuItem menuItemFromDB = await _db.MenuItems.FindAsync(id);
                    if (menuItemFromDB == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    else
                    {
                        //await _blobService.DeleteBlob(menuItemFromDB.Image.Split('/').Last(), SD.SD_Storage_Container); // Azure account needs to be subscribed

                        string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                        if (!string.IsNullOrEmpty(menuItemFromDB.Image))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, menuItemFromDB.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
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
