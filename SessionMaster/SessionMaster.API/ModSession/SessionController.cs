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
                var session = _unitOfWork.Sessions.GetById(id);

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
                var currentUser = HttpContext.Items["User"];
                if (currentUser != null)
                {
                    var userId = ((User)currentUser).Id;
                    session = _unitOfWork.Sessions.Register(userId, id);
                }
                else
                {
                    if (userResponse == null)
                    {
                        return BadRequest("Provide a name to register");
                    }

                    var anonymousUser = GetAnonymousUserBySession(id, userResponse.Name);

                    if (anonymousUser == null)
                    {
                        anonymousUser = _unitOfWork.AnonymousUsers.Add(new AnonymousUser
                        {
                            Name = userResponse.Name
                        });
                    }

                    session = _unitOfWork.Sessions.RegisterAnonymous(anonymousUser.Id, id);
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
                var currentUser = HttpContext.Items["User"];
                if (currentUser != null)
                {
                    var userId = ((User)currentUser).Id;
                    session = _unitOfWork.Sessions.Cancel(userId, id);
                }
                else
                {
                    if (userResponse == null)
                    {
                        return BadRequest("Provide a name to cancel");
                    }

                    var anonymousUser = GetAnonymousUserBySession(id, userResponse.Name);

                    if (anonymousUser == null)
                    {
                        return NotFound("This user does not exist in this sessionplan");
                    }

                    session = _unitOfWork.Sessions.CancelAnonymous(anonymousUser.Id, id);

                    //cleanup the anonymous user if the last session has been canceled to prevent "Leichen" in the DB
                    if (!anonymousUser.SessionAnonymousUsers.Any(s => s.SessionId != id))
                    {
                        _unitOfWork.AnonymousUsers.Remove(anonymousUser.Id);
                    }
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

        /// <summary>
        /// Gets the anonymous user of a sessionplan based on a session and its name if any exists
        /// </summary>
        /// <param name="sessionId">The session id to check the plan</param>
        /// <param name="userName">The name of the anonymous user</param>
        /// <returns>The anonymous user if exists</returns>
        private AnonymousUser GetAnonymousUserBySession(Guid sessionId, string userName)
        {
            var plan = _unitOfWork.Sessionplans.Get(sp => sp.Sessions.Any(s => s.Id == sessionId),
                        include: e => e.Include(s => s.Sessions).ThenInclude(a => a.SessionAnonymousUsers).ThenInclude(sa => sa.AnonymousUser));

            var sessions = plan?.FirstOrDefault()?.Sessions?.FirstOrDefault(s => s.SessionAnonymousUsers.Any(sa => sa.AnonymousUser.Name == userName));

            return sessions?.SessionAnonymousUsers.FirstOrDefault(su => su.AnonymousUser.Name == userName)?.AnonymousUser;
        }
    }
}