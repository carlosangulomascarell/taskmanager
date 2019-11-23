﻿using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TaskManagerAPI.BL.AuthProcess;
using TaskManagerAPI.CQRS.AuthProcess.Commands;
using TaskManagerAPI.Exceptions.Helpers;
using TaskManagerAPI.Filters.Authentication;
using TaskManagerAPI.Models.FE.APIRequests;

namespace TaskManagerAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IErrorResponseCreator _errorResponseCreator;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IErrorResponseCreator errorResponseCreator, IMediator mediator, IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
            _errorResponseCreator = errorResponseCreator;
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Log in the user 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">User Logged correctly</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            _logger.LogInformation($"{request.Email}");

            var result = await _mediator.Send(_mapper.Map<LoginCommand>(request));
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return _errorResponseCreator.CreateResponse(result.Errors);
            }
        }

        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">User Sign up correctly</response>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SignUp([FromBody]SignUpRequest request)
        {
            var result = await _mediator.Send(_mapper.Map<SignUpCommand>(request));
            if (result.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return _errorResponseCreator.CreateResponse(result.Errors);
            }
        }

        /// <summary>
        /// Log off the current user
        /// </summary>
        /// <returns></returns>
        /// <response code="200">User Session ended correctly</response>
        [HttpPost("logoff")]
        [AuthenticationFilter]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogOff()
        {
            var result = await _mediator.Send(new LogOffCommand());
            if (result.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return _errorResponseCreator.CreateResponse(result.Errors);

            }

        }
    }
}
