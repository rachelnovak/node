using _01_BOL;
using _02_BLL;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using WebApi.Models;


namespace UIL.Controllers
{
    public class ManagerController : ApiController
    {
        

        /// <summary>
        /// add new project
        /// </summary>
        /// <param name="Project">Project deatails</param>
        /// 
        [HttpPut]
        [Route("api/addProject")]
        public HttpResponseMessage AddProject([FromBody]Project value)
        {
            //curl -v -X POST -H "Content-type: application/json" -d "{\"Name\":\"tryProject\", \"Customer\":\"nnn\",\"TeamLeaderId\":\"11\" , \"DevelopHours\":\"300\",\"QAHours\":\"250\", \"UiUxHours\":\"100\",\"StartDate\":\"2018-02-02\",\"EndDate\":\"2018-07-07\"}"  http://localhost:59628/api/addProject

            if (ModelState.IsValid)
            {
             
                return (ManagerLogic.AddProject(value)) ?
                   new HttpResponseMessage(HttpStatusCode.Created)
                   {
                       Content = new ObjectContent<bool>(true, new JsonMediaTypeFormatter())
                   }:
                   new HttpResponseMessage(HttpStatusCode.BadRequest)
                   {
                       Content = new ObjectContent<String>("Can not add to DB", new JsonMediaTypeFormatter())
                   };
            }
            return Global.ErrorList(ModelState);
         
        }

           /// <summary>
           /// add new worker
           /// </summary>
           /// <param name="Project">Project deatails</param>
           /// 
        [HttpPost]
        [Route("api/addUser")]
        public HttpResponseMessage AddWorker([FromBody]User value)
        {
            //curl -v -X POST -H "Content-type: application/json" -d "{\"Name\":\"Gila\",\"UserName\":\"gggg\",\"Password\":\"gggggg\",\"JobId\":\"4\",\"EMail\":\"safdsa@fdsaf\",\"ManagerId\":\"11\"}"  http://localhost:59628/api/addWorker
            //
            if (ModelState.IsValid)
            {
                return (ManagerLogic.AddUser(value)) ?
                   new HttpResponseMessage(HttpStatusCode.Created)
                   {
                       Content = new ObjectContent<bool>(true, new JsonMediaTypeFormatter())
                   }:
                   new HttpResponseMessage(HttpStatusCode.BadRequest)
                   {
                       Content = new ObjectContent<String>("Can not add to DB", new JsonMediaTypeFormatter())
                   };
            }
            return Global.ErrorList(ModelState);
        }

        /// <summary>
        /// add workers to project
        /// </summary>
        /// <param name="ids">the ids'workers to add</param>
        /// <param name="name">the project name</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/addWorkersToProject/{name}")]
        public HttpResponseMessage addWorkersToProject([FromBody]int[] ids, [FromUri]string name)
        {
            //curl -v -X POST -H "Content-type: application/json" -d "{\"Name\":\"tryProject\", \"Customer\":\"nnn\",\"TeamLeaderId\":\"11\" , \"DevelopHours\":\"300\",\"QAHours\":\"250\", \"UiUxHours\":\"100\",\"StartDate\":\"2018-02-02\",\"EndDate\":\"2018-07-07\"}"  http://localhost:59628/api/addProject
            // value.EndDate.tos
            return (ManagerLogic.addWorkersToProject(ids, name)) ?

                     new HttpResponseMessage(HttpStatusCode.OK) :
                     new HttpResponseMessage(HttpStatusCode.BadRequest)
                     {
                         Content = new ObjectContent<String>("Can not update in DB", new JsonMediaTypeFormatter())
                     };


        }

        /// <summary>
        /// edit worker's details
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/updateUser")]
        public HttpResponseMessage UpdateWorker([FromBody]User value)
        {
            //curl -v -X PUT -H "Content-type: application/json" -d "{\"Id\":\"7\",\"Name\":\"Malki\", \"UserName\":\"ggggg\",\"Password\":\"mmmggg\" , \"JobId\":\"3\",\"EMail\":\"sjafjkl@dfaf\", \"ManagerId\":\"5\"}"  http://localhost:59628/api/UpdateWorker
             if (ModelState.IsValid)
            {
                return (ManagerLogic.UpdateUser(value)) ?
                    new HttpResponseMessage(HttpStatusCode.OK)
                     {
                    Content = new ObjectContent<bool>(true, new JsonMediaTypeFormatter())
                    }:
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new ObjectContent<String>("Can not update in DB", new JsonMediaTypeFormatter())
                    };
            };

           return Global.ErrorList(ModelState);
        }
        /// <summary>
        /// get all workers
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/getAllUsers")]
        public HttpResponseMessage GetAllWorkers()
        {

            //curl -X GET -v http://localhost:59628/api/getAllWorkers
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<List<User>>(ManagerLogic.GetAllUsers(), new JsonMediaTypeFormatter())
            };
        }
        /// <summary>
        /// get all managers
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/getAllManagers")]
        public HttpResponseMessage GetAllManagers()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<List<User>>(ManagerLogic.GetAllManagers(), new JsonMediaTypeFormatter())
            };
        }


        /// <summary>
        /// get all team leaders
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/getAllTeamLeaders")]
        public HttpResponseMessage getAllTeamLeaders()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<List<User>>(ManagerLogic.getAllTeamLeaders(), new JsonMediaTypeFormatter())
            };
        }

        /// <summary>
        /// get all projects
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/getAllProjects")]
        public HttpResponseMessage getAllProjects()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<List<Project>>(ManagerLogic.getAllProjects(), new JsonMediaTypeFormatter())
            };
        }

        /// <summary>
        /// delete worker
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpDelete]
        [Route("api/deleteUser/{id}")]
        public HttpResponseMessage DeleteWorker(int id)
        //curl -X DELETE -v http://localhost:59628/api/deleteWorker/id=8
        {
            return (ManagerLogic.RemoveUser(id)) ?
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ObjectContent<bool>(true, new JsonMediaTypeFormatter())
                    } :
                    new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new ObjectContent<String>("Can not remove from DB", new JsonMediaTypeFormatter())
                    };
        }
    }
}
