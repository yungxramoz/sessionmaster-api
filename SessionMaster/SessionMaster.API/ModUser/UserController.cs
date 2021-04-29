using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SessionMaster.API.Core.Attributes;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.Common.Models;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;

namespace SessionMaster.API.ModUser
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Get all users</returns>
        /// <response code="200">Returns all the users</response>
        /// <response code="401">Valid JWT token needed</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            var users = _unitOfWork.Users.Get();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The requested user</returns>
        /// <response code="200">Returns the specific user</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">User does not exist</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(id);
                var model = _mapper.Map<UserModel>(user);
                return Ok(model);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Authenticate a user with the credentials
        /// </summary>
        /// <param name="model">The credentials of a user</param>
        /// <returns>User with token on successful authenticate</returns>
        /// <response code="200">Successfully authenticated and returns token</response>
        /// <response code="400">Invalid credentials</response>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            try
            {
                User user = _unitOfWork.Users.Authenticate(model.Username, model.Password);

                var token = JwtTokenHelper.GenerateJwtToken(user.Id.ToString(), _appSettings.Secret);

                //Create return model with token
                var returnModel = _mapper.Map<UserModel>(user);
                returnModel.Token = token;
                return Ok(returnModel);
            }
            catch (InfoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new account for a new user
        /// </summary>
        /// <param name="model">All required data for a new user</param>
        /// <returns>Status if user has been successfully created</returns>
        /// <response code="200">Successfully registered the new user</response>
        /// <response code="400">Invalid registration data</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Register([FromBody] RegistrationModel model)
        {
            var user = _mapper.Map<User>(model);

            try
            {
                _unitOfWork.Users.Add(user, model.Password);
                _unitOfWork.Complete();
                return Ok();
            }
            catch (InfoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a specific User
        /// </summary>
        /// <param name="id">Id of the user to update</param>
        /// <param name="model">Updated data of the user</param>
        /// <returns>Status if user has been successfully updated</returns>
        /// <response code="200">Successfully updated the user</response>
        /// <response code="400">Invalid update data as duplicate username</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">User does not exist</response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] UpdateUserModel model)
        {
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                var userUpdated = _unitOfWork.Users.Update(user, model.Password);
                _unitOfWork.Complete();

                var returnModel = _mapper.Map<UserModel>(userUpdated);
                return Ok(returnModel);
            }
            catch (InfoException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete a sepcific user
        /// </summary>
        /// <param name="id">Id of the user to delete</param>
        /// <returns>Status if user has been successfully deleted</returns>
        /// <response code="200">Successfully deleted the user</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">User does not exist</response>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _unitOfWork.Users.Remove(id);
                _unitOfWork.Complete();
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}