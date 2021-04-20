using Microsoft.AspNetCore.Mvc;
using SessionMaster.BLL.Core;
using SessionMaster.BLL.ModUser;
using SessionMaster.DAL.Entities;

namespace SessionMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : GenericController<User, IUserRepository>
    {
        public UserController(IUnitOfWork unitOfWork, IUserRepository repository)
            : base(unitOfWork, repository)
        {
        }
    }
}
