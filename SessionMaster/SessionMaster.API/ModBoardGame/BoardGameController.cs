using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SessionMaster.API.ModBoardGame.ViewModels;
using SessionMaster.BLL.Core;
using SessionMaster.Common.Exceptions;
using SessionMaster.Common.Helpers;
using SessionMaster.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SessionMaster.API.ModBoardGame
{
    [Produces("application/json")]
    [Route("api/[controller]")]
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
        /// Get board games by a name filter
        /// </summary>
        /// <param name="name">The name of the boardgame</param>
        /// <returns>Filtered board games</returns>
        /// <response code="200">Returns all boardgames found by name (max. 25)</response>
        /// <response code="400">An error occured requesting the thirdparty boardgame api</response>
        /// <response code="401">Valid JWT token needed</response>
        //[Authorize]
        [HttpGet("search/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GeByName(string name)
        {
            try
            {
                var nameFilter = BoardGameAtlasFilterHelper.ByName(name);
                var boardGames = await _unitOfWork.BoardGames.GetAll(nameFilter, _appSettings.BgaClientId);
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
        /// <param name="id">The id of the board gamer</param>
        /// <returns>The requested board game details</returns>
        /// <response code="200">Returns the specific board game</response>
        /// <response code="400">An error occured requesting the thirdparty boardgame api</response>
        /// <response code="401">Valid JWT token needed</response>
        /// <response code="404">Board game with the given id not found</response>
        //[Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var boardGame = await _unitOfWork.BoardGames.GetById(id, _appSettings.BgaClientId);
                var model = _mapper.Map<BoardGameModel>(boardGame);
                return Ok(boardGame);
            }
            catch(NotFoundException ex)
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
