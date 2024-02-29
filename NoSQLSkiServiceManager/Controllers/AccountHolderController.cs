using Microsoft.AspNetCore.Mvc;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace NoSQLSkiServiceManager.Controllers
{
    /// <summary>
    /// Manages employee-related operations such as authentication and account unlocking.
    /// </summary>
    public class AccountHolderController : GenericController<AccountHolder, AccountHolderCreateDto, AccountBalanceUpdateDto, AccountHolderResponseDto>
    {
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the EmployeeController class.
        /// </summary>
        /// <param name="genericService">Provides generic CRUD operations.</param>
        /// <param name="employeeService">Provides employee-specific services.</param>
        /// <param name="tokenService">Provides JWT token services.</param>
        /// <param name="mapper">Provides object mapping services.</param>
        public AccountHolderController(GenericService<AccountHolder, AccountHolderCreateDto, AccountBalanceUpdateDto, AccountHolderResponseDto> genericService,
                                  EmployeeService employeeService,
                                  TokenService tokenService,
                                  IMapper mapper) : base(genericService)
        {
            _employeeService = employeeService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public override async Task<IActionResult> Create(AccountHolderCreateDto createDto)
        {
            return await base.Create(createDto);
        }

        /// <summary>
        /// Authenticates an employee and generates a JWT token if successful.
        /// </summary>
        /// <param name="loginDto">The login details of the employee.</param>
        /// <returns>A JWT token if authentication is successful; otherwise, an unauthorized result.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountHolderLoginDto loginDto)
        {
            var authResult = await _employeeService.AuthenticateEmployeeAsync(loginDto);

            if (authResult.IsAuthenticated)
            {
                var token = _tokenService.CreateToken(authResult.Employee.Username, authResult.Employee.Role, authResult.Employee.Id.ToString());

                Log.Information("Event: Login Success, User: {Username}, Action: User logged in", loginDto.Username);

                return Ok(new { Token = token });
            }
            else
            {
                Log.Information("Event: Login Failed, User: {Username}, Action: Failed login attempt, Reason: {Reason}", loginDto.Username, authResult.Message);

                return Unauthorized(authResult.Message);
            }
        }


        /// <summary>
        /// Unlocks an employee's locked account.
        /// </summary>
        /// <param name="username">The username of the employee whose account is to be unlocked.</param>
        /// <returns>A success result if the account is unlocked; otherwise, a not found result.</returns>
        [HttpPost("unlock/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlockAccount(string username)
        {
            var success = await _employeeService.UnlockEmployeeAccount(username);

            if (!success)
            {
                return NotFound("Benutzerkonto nicht gefunden oder nicht gesperrt.");
            }

            return Ok("Benutzerkonto erfolgreich entsperrt.");
        }

    }

}