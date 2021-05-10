using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionMaster.API.Core.Attributes;
using SessionMaster.API.ModSessionplan.ViewModels;
using SessionMaster.API.ModUser.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;

namespace SessionMaster.API.ModUser
{
    [Produces("application/json")]
    [Route("api/sessionplans")]
    [ApiController]
    public class SessionplanController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public SessionplanController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all sessionplans of the current logged in user
        /// </summary>
        /// <returns>Get all personal sessionplans</returns>
        /// <response code="200">Succefully retrieved sessionplans</response>
        /// <response code="401">Valid JWT token needed</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Get()
        {
            var currentUser = (User)HttpContext.Items["User"];
            var sessionplans = _unitOfWork.Sessionplans.Get(u => u.UserId == currentUser.Id);
            var model = _mapper.Map<IList<SessionplanOverviewModel>>(sessionplans);
            return Ok(model);
        }

        /// <summary>
        /// Get the details of a specific sessionplan
        /// </summary>
        /// <param name="id">The id of the sessionplan</param>
        /// <returns>The requested sessionplan</returns>
        /// <response code="200">Successfully retrieved sessionplan details</response>
        /// <response code="404">Sessionplan does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            try
            {
                var sessionplan = _unitOfWork.Sessionplans.GetById(id);
                var model = _mapper.Map<SessionplanDetailModel>(sessionplan);
                return Ok(model);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Create a new sessionplan
        /// </summary>
        /// <param name="sessionplan">the name of the sessionplan</param>
        /// <returns>Newly created sessionplan details</returns>
        /// <response code="200">Successfully created a sessionplan</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Post([FromBody] AddSessionplanModel sessionplan)
        {
            try
            {
                var sessionplanEntity = _mapper.Map<Sessionplan>(sessionplan);

                var currentUser = HttpContext.Items["User"];
                if (currentUser != null)
                {
                    sessionplanEntity.UserId = ((User)currentUser).Id;
                }

                var addedSessionplan = _unitOfWork.Sessionplans.Add(sessionplanEntity);
                _unitOfWork.Complete();

                return Get(addedSessionplan.Id);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InfoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a specific sessionplan
        /// </summary>
        /// <param name="id">Id of the sessionplan to update</param>
        /// <param name="model">Updated data of the sessionplan</param>
        /// <returns>Updated sessionplan details</returns>
        /// <response code="200">Successfully updated the sessionplan</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">Sessionplan does not exist</response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(Guid id, [FromBody] UpdateSessionplanModel model)
        {
            var sessionplan = _mapper.Map<Sessionplan>(model);
            var currentUser = (User)HttpContext.Items["User"];

            sessionplan.Id = id;
            sessionplan.UserId = currentUser.Id;

            try
            {
                var sessionplanUpdated = _unitOfWork.Sessionplans.Update(sessionplan);
                _unitOfWork.Complete();

                var returnModel = _mapper.Map<SessionplanDetailModel>(sessionplanUpdated);
                return Ok(returnModel);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete a sepcific sessionplan
        /// </summary>
        /// <param name="id">Id of the sessionplan to delete</param>
        /// <returns>Status if sessionplan has been successfully deleted</returns>
        /// <response code="200">Successfully deleted the sessionplan</response>
        /// <response code="404">Sessionplan does not exist</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _unitOfWork.Sessionplans.Remove(id);
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