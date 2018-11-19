using _01_BOL;
using _02_BLL;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace UIL.Controllers
{
    public class HomeController : ApiController
    {
        // POST api/<controller>
        //curl -v -X POST -H "Content-type: application/json" -d "{\"UserName\":\"DDD\", \"Password\":\"444444\"}"  http://localhost:59628/api/login
        /// <summary>
        /// check the user name and password to log in
        /// </summary>
        /// <param name="user">the user who loged in</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/login")]
        public HttpResponseMessage Login([FromBody]User user)
        {
            User u = HomeLogic.Login(user.UserName, user.Password);
            if (u != null)
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<User>(u, new JsonMediaTypeFormatter())
                };
            else return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new ObjectContent<String>("Can not log in", new JsonMediaTypeFormatter())
            };
        }
    }
}