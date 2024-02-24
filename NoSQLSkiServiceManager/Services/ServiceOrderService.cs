using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using System;
using System.Threading.Tasks;

namespace NoSQLSkiServiceManager.Services
{
    /// <summary>
    /// Provides service operations for service orders including creation with business logic validations.
    /// Inherits from the generic service for common CRUD operations.
    /// </summary>
    public class ServiceOrderService : GenericService<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto>
    {
        private readonly IMongoCollection<ServiceType> _serviceTypes;
        private readonly IMongoCollection<ServicePriority> _servicePriorities;

        /// <summary>
        /// Initializes a new instance of the ServiceOrderService with the necessary database collections.
        /// </summary>
        /// <param name="database">The Mongo database connection.</param>
        /// <param name="mapper">The AutoMapper instance for mapping between DTOs and database models.</param>
        public ServiceOrderService(IMongoDatabase database, IMapper mapper)
            : base(database, mapper, "serviceOrders")
        {
            _serviceTypes = database.GetCollection<ServiceType>("serviceTypes");
            _servicePriorities = database.GetCollection<ServicePriority>("servicePriorities");
        }

        /// <summary>
        /// Creates a service order with additional business logic to validate service types and priorities.
        /// </summary>
        /// <param name="createDto">The service order creation DTO from the client request.</param>
        /// <returns>The created service order response DTO.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a required service type or priority is not found.</exception>
        public override async Task<OrderResponseDto> CreateAsync(CreateServiceOrderRequestDto createDto)
        {
            var serviceType = await _serviceTypes
                                .Find(st => st.Id == createDto.ServiceTypeId)
                                .FirstOrDefaultAsync();

            if (serviceType == null)
            {
                throw new InvalidOperationException("ServiceType not found");
            }

            var servicePriority = await _servicePriorities
                                     .Find(sp => sp.Id == createDto.PriorityId)
                                     .FirstOrDefaultAsync();

            if (servicePriority == null)
            {
                throw new InvalidOperationException("ServicePriority not found");
            }

            decimal priceAdjustmentFactor = 1.0m;
            if (servicePriority.PriorityName == "Express")
            {
                priceAdjustmentFactor = 1.2m;
            }
            else if (servicePriority.PriorityName == "Low")
            {
                priceAdjustmentFactor = 0.8m;
            }
            serviceType.Cost *= priceAdjustmentFactor;

            var serviceOrder = _mapper.Map<ServiceOrder>(createDto);
            serviceOrder.ServiceType = serviceType;
            serviceOrder.Priority = servicePriority;
            serviceOrder.CreationDate = DateTime.UtcNow;
            var basePickupDate = serviceOrder.CreationDate.AddDays(7 + servicePriority.DayCount);
            var desiredPickupDate = new DateTime(basePickupDate.Year, basePickupDate.Month, basePickupDate.Day);
            serviceOrder.DesiredPickupDate = desiredPickupDate;
            serviceOrder.Status = OrderStatus.Offen;

            await _collection.InsertOneAsync(serviceOrder);
            return _mapper.Map<OrderResponseDto>(serviceOrder);
        }

    }
}
