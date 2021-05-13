using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionMaster.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionMaster.UnitTests.TestHelper
{
    public static class HttpContextTestHelper
    {
        public static Guid SetAuthorizedUser(this ControllerBase controller, Guid userId)
        {
            var user = new User
            {
                Id = userId
            };

            controller.SetDefaultHttpContext();
            controller.ControllerContext.HttpContext.Items.Add("User", user);

            return user.Id;
        }

        public static void SetDefaultHttpContext(this ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }
    }
}