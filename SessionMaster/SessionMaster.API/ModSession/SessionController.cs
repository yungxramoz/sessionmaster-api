using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SessionMaster.API.ModSession.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SessionMaster.API.ModSession
{
    [Produces("application/json")]
    [Route("api/sessions")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public SessionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the details of a specific session
        /// </summary>
        /// <param name="id">The id of the session</param>
        /// <returns>The requested session</returns>
        /// <response code="200">Successfully retrieved session details</response>
        /// <response code="404">Session does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            try
            {
                var session =
                    _unitOfWork.Sessions.GetById(id,
                        e => e.Include(s => s.SessionUsers).ThenInclude(su => su.User)
                            .Include(s => s.SessionAnonymousUsers).ThenInclude(sau => sau.AnonymousUser)
                    );

                var model = _mapper.Map<SessionModel>(session);
                model.Users = MapSessionUsersToSessionUserModel(session).OrderBy(u => u.Name);

                return Ok(model);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Register to a session
        /// </summary>
        /// <param name="id">The id of the session</param>
        /// <param name="userResponse">This param is used for the anonymous users to pass the name</param>
        /// <returns>Session with the registered users</returns>
        /// <response code="200">Successfully registered user to a session</response>
        /// <response code="400">Validation error on body</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{id}/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Register(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UserResponseSessionModel userResponse = null)
        {
            Session session = null;

            try
            {
                if (userResponse == null)
                {
                    var currentUser = HttpContext.Items["User"];
                    if (currentUser == null)
                    {
                        return BadRequest("Provide a name to register");
                    }

                    var userId = ((User)currentUser).Id;

                    session = _unitOfWork.Sessions.Register(userId, id);
                }
                else
                {
                    //Do stuff for unauth user
                }

                _unitOfWork.Complete();

                var model = _mapper.Map<SessionModel>(session);
                model.Users = MapSessionUsersToSessionUserModel(session).OrderBy(u => u.Name);

                return Ok(model);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Cancel / Sign out from a registered session
        /// </summary>
        /// <param name="id">The id of the session</param>
        /// <param name="userResponse">This param is used for the anonymous users to pass the name</param>
        /// <returns>Session with the registered users</returns>
        /// <response code="200">Successfully canceld a session</response>
        /// <response code="400">Validation error on body</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Cancel(Guid id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UserResponseSessionModel userResponse = null)
        {
            Session session = null;

            try
            {
                if (userResponse == null)
                {
                    var currentUser = HttpContext.Items["User"];
                    if (currentUser == null)
                    {
                        return BadRequest("Provide a name to cancel");
                    }

                    var userId = ((User)currentUser).Id;

                    session = _unitOfWork.Sessions.Cancel(userId, id);
                }
                else
                {
                    //Do stuff for unauth user
                }

                _unitOfWork.Complete();

                var model = _mapper.Map<SessionModel>(session);
                model.Users = MapSessionUsersToSessionUserModel(session).OrderBy(u => u.Name);

                return Ok(model);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        //TODO this is just provisionally because auto mapper didn't workout on complex stuctures
        private List<SessionUserModel> MapSessionUsersToSessionUserModel(Session session)
        {
            var users = session.SessionUsers.Select(s => s.User).Select(u => new SessionUserModel
            {
                Name = $"{u.Firstname} {u.Lastname}"
            }).ToList();

            var anonymousUsers = session.SessionAnonymousUsers.Select(s => s.AnonymousUser).Select(a => new SessionUserModel
            {
                Name = a.Name
            }).ToList();

            users.AddRange(anonymousUsers);

            return users;
        }
    }
}