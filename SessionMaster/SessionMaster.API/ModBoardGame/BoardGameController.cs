using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SessionMaster.API.Core.Attributes;
using SessionMaster.API.ModBoardGame.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.Common.Models;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SessionMaster.API.ModBoardGame
{
    [Produces("application/json")]
    [Route("api/boardgames")]
    [ApiController]
    public class BoardGameController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public BoardGameController(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Get board games filtered
        /// </summary>
        /// <param name="name">The name of the board game</param>
        /// <param name="playerCount">The number of player that the game should support</param>
        /// <returns>Board games matching the filter</returns>
        /// <response code="200">Successfully retrieved the filtered board games</response>
        /// <response code="400">An error occured requesting the thirdparty boardgame api</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFiltered(string name = "", int? playerCount = null)
        {
            try
            {
                var nameFilter = BoardGameAtlasFilterHelper.ByName(name);
                var minPlayerFilter = BoardGameAtlasFilterHelper.ByPlayerCount(playerCount);
                var filter = nameFilter + minPlayerFilter;

                var boardGames = await _unitOfWork.BoardGames.GetAll(filter, _appSettings.BgaClientId);
                var model = _mapper.Map<IList<BoardGameModel>>(boardGames);
                return Ok(model);
            }
            catch (InfoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a specific board game
        /// </summary>
        /// <param name="id">The id of the board game</param>
        /// <returns>The requested board game details</returns>
        /// <response code="200">Successfully retrieved the board game</response>
        /// <response code="400">An error occured requesting the thirdparty boardgame api</response>
        /// <response code="404">Board game with the given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var boardGame = await _unitOfWork.BoardGames.GetById(id, _appSettings.BgaClientId);
                var model = _mapper.Map<BoardGameModel>(boardGame);
                return Ok(model);
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
        /// Get the board game collection of a specific user
        /// </summary>
        /// <param name="id">The id of the user whose board game collection to retrieve</param>
        /// <returns>Filtered board games</returns>
        /// <response code="200">Successfully retrieved the users board game collection</response>
        /// <response code="400">An error occured requesting the thirdparty boardgame api</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpGet("~/api/users/{id}/boardgames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCollection(Guid id)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(id, u => u.Include(u => u.BoardGames));

                if (user.BoardGames?.Any() != true)
                {
                    return Ok();
                }

                var boardgameIds = user.BoardGames.Select(bg => bg.BoardGameId).ToList();
                var boardGames = await _unitOfWork.BoardGames.GetAll(BoardGameAtlasFilterHelper.ByIds(boardgameIds), _appSettings.BgaClientId);
                var model = _mapper.Map<IList<BoardGameModel>>(boardGames);
                return Ok(model.OrderBy(bg => bg.Name));
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
        /// Get the board game suggestions based on the participants collections
        /// </summary>
        /// <param name="id">The id of the session to get board game suggestion</param>
        /// <returns>Playable board games</returns>
        /// <response code="200">Successfully retrieved the board game suggestion</response>
        /// <response code="400">An error occured requesting the thirdparty boardgame api</response>
        [HttpGet("~/api/sessions/{id}/boardgames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSuggestion(Guid id)
        {
            try
            {
                var session = _unitOfWork.Sessions.GetById(id,
                        q => q.Include(s => s.SessionAnonymousUsers).Include(s => s.SessionUsers).ThenInclude(su => su.User).ThenInclude(u => u.BoardGames));

                var boardgameIds = session.SessionUsers.Select(su => su.User)
                        .SelectMany(u => u.BoardGames, (_, boardgame) => boardgame.BoardGameId).Distinct().ToList();

                if (boardgameIds.Count == 0)
                {
                    return Ok(new List<BoardGameModel>());
                }

                var boardGames = await _unitOfWork.BoardGames.GetAll(BoardGameAtlasFilterHelper.ByIds(boardgameIds), _appSettings.BgaClientId);

                var playerCount = session.SessionUsers.Count + session.SessionAnonymousUsers.Count;
                boardGames = boardGames.Where(bg => bg.MaxPlayers >= playerCount && bg.MinPlayers <= playerCount).ToList();

                var model = _mapper.Map<IList<BoardGameModel>>(boardGames);

                //don't order because response from the api is already ordered by popularity
                return Ok(model);
            }
            catch (InfoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Add a board game to a users collection
        /// </summary>
        /// <param name="id">The id of the user whose board game collection to extend</param>
        /// <param name="boardGame">The id of the board game to add to the collection</param>
        /// <returns>Complete Collection</returns>
        /// <response code="200">Successfully added the board game to the users collection</response>
        /// <response code="400">An error occured either on the tirdparty api or the db action</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">Board game or user not found</response>
        [Authorize]
        [HttpPost("~/api/users/{id}/boardgames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostToCollection(Guid id, [FromBody] AddToCollectionModel boardGame)
        {
            try
            {
                //verify that user and boardgame exists
                var user = _unitOfWork.Users.GetById(id);
                var boardGameEntity = await _unitOfWork.BoardGames.GetById(boardGame.BoardGameId, _appSettings.BgaClientId);

                var addModel = new UserBoardGame
                {
                    UserId = user.Id,
                    BoardGameId = boardGameEntity.Id
                };

                _unitOfWork.BoardGames.Add(addModel);

                try
                {
                    _unitOfWork.Complete();
                }
                catch (DbUpdateException)
                {
                    return BadRequest("The board game is already in the collection");
                }

                return await GetCollection(id);
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
        /// Delete a board game form a users collection
        /// </summary>
        /// <param name="id">The id of the user, whose board game gets removed</param>
        /// <param name="boardGameId">The id of the board game to remove from the collection</param>
        /// <returns>Complete Collection</returns>
        /// <response code="200">Successfully removed the board game from the users collection</response>
        /// <response code="400">An error occured either on the tirdparty api or the db action</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">Board game or user not found</response>
        [Authorize]
        [HttpDelete("~/api/users/{id}/boardgames/{boardGameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFromCollection(Guid id, string boardGameId)
        {
            try
            {
                //verify that user exists
                var user = _unitOfWork.Users.GetById(id, e => e.Include(bg => bg.BoardGames));

                var boardgameToRemove = user.BoardGames.SingleOrDefault(bg => bg.BoardGameId == boardGameId);

                if (boardgameToRemove == null)
                {
                    throw new NotFoundException("Board game is not in the collection");
                }

                _unitOfWork.BoardGames.Remove(boardgameToRemove.Id);

                try
                {
                    _unitOfWork.Complete();
                }
                catch (DbUpdateException)
                {
                    return BadRequest("Not able to remove board game from the collection");
                }

                return await GetCollection(id);
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
    }
}