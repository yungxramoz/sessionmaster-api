using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionMaster.BLL.Core;
using SessionMaster.DAL.Entity;
using System;

namespace SessionMaster.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<TEntity, TRepository> : ControllerBase 
        where TEntity : BaseEntity 
        where TRepository : class, IGenericRepository<TEntity>
    {
        protected TRepository _repository;
        protected IUnitOfWork _unitOfWork;

        public GenericController(IUnitOfWork unitOfWork, TRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var entities = _repository.GetAll();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var entity = _repository.GetById(id);
            return Ok(entity);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TEntity entity)
        {
            try
            {
                _repository.Add(entity);
                _unitOfWork.Complete();
                return Ok();
            }
            //TODO catch propper exception
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] TEntity entity)
        {
            entity.Id = id;
            try
            {
                _repository.Update(entity);
                _unitOfWork.Complete();
                return Ok();
            }
            //TODO catch propper exception
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _repository.Remove(id);
            _unitOfWork.Complete();
            return Ok();
        }
    }
}
