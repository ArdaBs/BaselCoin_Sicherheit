using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Interfaces;
using NoSQLSkiServiceManager.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using Serilog; // Stellen Sie sicher, dass Sie Serilog importieren

namespace NoSQLSkiServiceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<TModel, TCreateDto, TUpdateDto, TResponseDto> : ControllerBase
        where TModel : class, IEntity
        where TCreateDto : class
        where TUpdateDto : class
        where TResponseDto : class, IResponseDto
    {
        private readonly GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto> _service;

        public GenericController(GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto> service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            Log.Information("Attempting to retrieve all instances of {Model}", typeof(TModel).Name);
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Get()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Log.Information("Attempting to retrieve an instance for User ID: {UserID}", userId);

            if (string.IsNullOrEmpty(userId))
            {
                Log.Warning("Failed to retrieve an instance: User ID is invalid");
                return Unauthorized("Benutzer-ID ist ungültig.");
            }

            var item = await _service.GetByIdAsync(userId);
            if (item == null)
            {
                Log.Warning("Failed to find an instance for User ID: {UserID}", userId);
                return NotFound();
            }

            Log.Information("Successfully retrieved an instance for User ID: {UserID}", userId);
            return Ok(item);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TCreateDto createDto)
        {
            Log.Information("Attempting to create a new instance of {Model}", typeof(TModel).Name);
            var createdItem = await _service.CreateAsync(createDto);
            Log.Information("Successfully created a new instance of {Model} with ID: {ID}", typeof(TModel).Name, createdItem.Id);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, TUpdateDto updateDto)
        {
            Log.Information("Attempting to update an instance of {Model} with ID: {ID}", typeof(TModel).Name, id);
            var updated = await _service.UpdateAsync(id, updateDto);

            if (!updated)
            {
                Log.Warning("Failed to update an instance of {Model} with ID: {ID}", typeof(TModel).Name, id);
                return NotFound();
            }

            Log.Information("Successfully updated an instance of {Model} with ID: {ID}", typeof(TModel).Name, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            Log.Information("Attempting to delete an instance of {Model} with ID: {ID}", typeof(TModel).Name, id);
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                Log.Warning("Failed to delete an instance of {Model} with ID: {ID}", typeof(TModel).Name, id);
                return NotFound();
            }

            Log.Information("Successfully deleted an instance of {Model} with ID: {ID}", typeof(TModel).Name, id);
            return NoContent();
        }
    }
}
