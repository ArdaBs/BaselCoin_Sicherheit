using MongoDB.Bson;
using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NoSQLSkiServiceManager.Services
{
    /// <summary>
    /// Provides service layer functionality for employee operations.
    /// </summary>
    public class EmployeeService : GenericService<AccountHolder, AccountHolderCreateDto, AccountBalanceUpdateDto, AccountHolderResponseDto>
    {
        private const int MaxLoginAttempts = 3;

        /// <summary>
        /// Initializes a new instance of the EmployeeService class.
        /// </summary>
        /// <param name="database">The Mongo database connection.</param>
        /// <param name="mapper">The class used for object mapping.</param>
        public EmployeeService(IMongoDatabase database, IMapper mapper)
            : base(database, mapper, "accountHolders")
        {
        }

        /// <summary>
        /// Authenticates an employee's login attempt.
        /// </summary>
        /// <param name="loginDto">The data transfer object containing login credentials.</param>
        /// <returns>A message indicating the result of the authentication attempt.</returns>
        public async Task<AuthenticationResult> AuthenticateEmployeeAsync(AccountHolderLoginDto loginDto)
        {
            var employee = await _collection.Find(emp => emp.Username == loginDto.Username).FirstOrDefaultAsync();
            if (employee == null)
            {
                return new AuthenticationResult { IsAuthenticated = false, Message = "Benutzername nicht gefunden.", RemainingAttempts = MaxLoginAttempts };
            }

            if (employee.IsLocked)
            {
                return new AuthenticationResult { IsAuthenticated = false, Message = "Benutzerkonto ist gesperrt.", RemainingAttempts = 0 };
            }

            if (employee.Password != loginDto.Password)
            {
                employee.FailedLoginAttempts++;
                int remainingAttempts = MaxLoginAttempts - employee.FailedLoginAttempts;
                if (employee.FailedLoginAttempts >= MaxLoginAttempts)
                {
                    employee.IsLocked = true;
                    await _collection.UpdateOneAsync(
                        Builders<AccountHolder>.Filter.Eq(emp => emp.Id, employee.Id),
                        Builders<AccountHolder>.Update
                            .Set(emp => emp.IsLocked, true)
                            .Set(emp => emp.FailedLoginAttempts, employee.FailedLoginAttempts)
                    );
                    return new AuthenticationResult { IsAuthenticated = false, Message = "Benutzerkonto wurde wegen zu vieler fehlgeschlagener Versuche gesperrt.", RemainingAttempts = 0 };
                }
                else
                {
                    await _collection.UpdateOneAsync(
                        Builders<AccountHolder>.Filter.Eq(emp => emp.Id, employee.Id),
                        Builders<AccountHolder>.Update.Set(emp => emp.FailedLoginAttempts, employee.FailedLoginAttempts)
                    );
                    return new AuthenticationResult { IsAuthenticated = false, Message = $"Falsches Passwort. Verbleibende Versuche: {remainingAttempts}", RemainingAttempts = remainingAttempts };
                }
            }

            await _collection.UpdateOneAsync(
                Builders<AccountHolder>.Filter.Eq(emp => emp.Id, employee.Id),
                Builders<AccountHolder>.Update.Set(emp => emp.FailedLoginAttempts, 0)
            );

            return new AuthenticationResult
            {
                IsAuthenticated = true,
                Employee = employee,
                RemainingAttempts = MaxLoginAttempts
            };
        }

        /// <summary>
        /// Unlocks an employee's account.
        /// </summary>
        /// <param name="username">The username of the employee whose account is to be unlocked.</param>
        /// <returns>A boolean indicating whether the account was successfully unlocked.</returns>
        public async Task<bool> UnlockEmployeeAccount(string username)
        {
            var filter = Builders<AccountHolder>.Filter.Eq(emp => emp.Username, username) & Builders<AccountHolder>.Filter.Eq(emp => emp.IsLocked, true);
            var update = Builders<AccountHolder>.Update.Set(emp => emp.IsLocked, false).Set(emp => emp.FailedLoginAttempts, 0);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
