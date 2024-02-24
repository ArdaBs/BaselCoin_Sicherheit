using MongoDB.Bson;
using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoSQLSkiServiceManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NoSQLSkiServiceManager.Services
{
    /// <summary>
    /// Provides a generic service layer for CRUD operations on MongoDB collections.
    /// </summary>
    /// <typeparam name="TModel">The entity model type.</typeparam>
    /// <typeparam name="TCreateDto">The DTO used for create operations.</typeparam>
    /// <typeparam name="TUpdateDto">The DTO used for update operations.</typeparam>
    /// <typeparam name="TResponseDto">The DTO returned by the service methods.</typeparam>
    public class GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto>
    where TModel : class, IEntity
    where TCreateDto : class
    where TUpdateDto : class
    where TResponseDto : class, IResponseDto
    {
        protected readonly IMongoCollection<TModel> _collection;
        protected readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the GenericService class.
        /// </summary>
        /// <param name="database">The Mongo database connection.</param>
        /// <param name="mapper">The class used for object mapping.</param>
        /// <param name="collectionName">The name of the MongoDB collection.</param>
        public GenericService(IMongoDatabase database, IMapper mapper, string collectionName)
        {
            _collection = database.GetCollection<TModel>(collectionName);
            _mapper = mapper;

            if (_collection == null)
            {
                throw new ArgumentNullException(nameof(_collection), "MongoDB collection cannot be null.");
            }
        }

        /// <summary>
        /// Creates a new entity in the collection.
        /// </summary>
        /// <param name="createDto">The DTO from which to create the entity.</param>
        /// <returns>The created entity mapped to a response DTO.</returns>
        public virtual async Task<TResponseDto> CreateAsync(TCreateDto createDto)
        {
            var model = _mapper.Map<TModel>(createDto);
            await _collection.InsertOneAsync(model);
            return _mapper.Map<TResponseDto>(model);
        }

        /// <summary>
        /// Asynchronously retrieves all entities in the collection.
        /// </summary>
        /// <returns>A list of response DTOs representing all the entities in the collection.</returns>
        public async Task<IEnumerable<TResponseDto>> GetAllAsync()
        {
            var items = await _collection.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<TResponseDto>>(items);
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>The response DTO representing the entity, if found.</returns>
        public async Task<TResponseDto> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var item = await _collection.Find<TModel>(x => x.Id == objectId).FirstOrDefaultAsync();
            return _mapper.Map<TResponseDto>(item);
        }

        /// <summary>
        /// Asynchronously updates an entity in the collection.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="updateDto">The DTO containing update data.</param>
        /// <returns>True if the update is successful, false otherwise.</returns>
        public async Task<bool> UpdateAsync(string id, TUpdateDto updateDto)
        {
            var objectId = new ObjectId(id);
            var existingItem = await _collection.Find(x => x.Id == objectId).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                return false;
            }

            _mapper.Map(updateDto, existingItem);
            var result = await _collection.ReplaceOneAsync(x => x.Id == objectId, existingItem);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Asynchronously deletes an entity from the collection.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <returns>True if the deletion is successful, false otherwise.</returns>
        public async Task<bool> DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var result = await _collection.DeleteOneAsync(x => x.Id == objectId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

    }

}
