using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.BLL.Core;
using System;
using System.Collections.Generic;

namespace SessionMaster.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Get all users</returns>
        /// <response code="201">Returns all the users</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Get()
        {
            var users = _unitOfWork.Users.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The requested user</returns>
        /// <response code="201">Returns the specific user</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Get(Guid id)
        {
            var entity = _unitOfWork.Users.GetById(id);
            var user = _mapper.Map<UserModel>(entity);
            return Ok(user);
        }
    }
}
