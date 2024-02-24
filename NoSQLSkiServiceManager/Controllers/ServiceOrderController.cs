using Microsoft.AspNetCore.Mvc;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Services;

namespace NoSQLSkiServiceManager.Controllers
{
    /// <summary>
    /// Handles HTTP requests for service orders, extending functionality of a generic controller for <see cref="ServiceOrder"/>.
    /// </summary>
    /// <remarks>
    /// Provides an API endpoint for creating service orders with custom business logic via the <see cref="ServiceOrderService"/>.
    /// </remarks>
    public class ServiceOrderController : GenericController<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto>
    {
        private readonly ServiceOrderService _serviceOrderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOrderController"/> class.
        /// </summary>
        /// <param name="genericService">The generic service for basic CRUD operations.</param>
        /// <param name="serviceOrderService">The service containing business logic for service orders.</param>
        public ServiceOrderController(
            GenericService<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto> genericService,
            ServiceOrderService serviceOrderService) : base(genericService)
        {
            _serviceOrderService = serviceOrderService;
        }

        /// <summary>
        /// Creates a new service order.
        /// </summary>
        /// <param name="createDto">The service order creation data.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the create operation.</returns>
        /// <remarks>
        /// If successful, returns the created service order; otherwise, returns a BadRequest response.
        /// </remarks>
        [HttpPost]
        public override async Task<IActionResult> Create(CreateServiceOrderRequestDto createDto)
        {
            var createdOrder = await _serviceOrderService.CreateAsync(createDto);
            if (createdOrder == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(Get), new { id = createdOrder.Id }, createdOrder);
        }

    }

}
