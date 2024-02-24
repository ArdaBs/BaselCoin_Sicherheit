using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Interfaces;
using NoSQLSkiServiceManager.Services;
using Microsoft.AspNetCore.Authorization;

namespace NoSQLSkiServiceManager.Controllers
{
    /// <summary>
    /// Provides a generic API controller for CRUD operations on models.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TCreateDto">The type of the create DTO.</typeparam>
    /// <typeparam name="TUpdateDto">The type of the update DTO.</typeparam>
    /// <typeparam name="TResponseDto">The type of the response DTO.</typeparam>
    /// <remarks>
    /// This controller abstracts common CRUD operations to reduce redundancy across specific controllers.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<TModel, TCreateDto, TUpdateDto, TResponseDto> : ControllerBase
        where TModel : class, IEntity
        where TCreateDto : class
        where TUpdateDto : class
        where TResponseDto : class, IResponseDto
    {
        private readonly GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto> _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericController{TModel, TCreateDto, TUpdateDto, TResponseDto}"/> class.
        /// </summary>
        /// <param name="service">The service to be used for CRUD operations.</param>
        public GenericController(GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto> service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all instances of the model.
        /// </summary>
        /// <returns>A list of all instances.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a single instance of the model by ID.
        /// </summary>
        /// <param name="id">The ID of the instance to retrieve.</param>
        /// <returns>The requested instance if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            return item != null ? Ok(item) : NotFound();
        }

        /// <summary>
        /// Creates a new instance of the model.
        /// </summary>
        /// <param name="createDto">The data transfer object containing the creation data.</param>
        /// <returns>A response indicating the result of the creation operation.</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Create(TCreateDto createDto)
        {
            var createdItem = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        /// <summary>
        /// Updates an existing instance of the model.
        /// </summary>
        /// <param name="id">The ID of the instance to update.</param>
        /// <param name="updateDto">The data transfer object containing the update data.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, TUpdateDto updateDto)
        {
            var updated = await _service.UpdateAsync(id, updateDto);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes an existing instance of the model.
        /// </summary>
        /// <param name="id">The ID of the instance to delete.</param>
        /// <returns>A response indicating the result of the deletion operation.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
