using _00_DAL;
using _01_BOL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace _02_BLL
{
    public class ManagerLogic
    {

      
        public static List<User> GetAllManagers()
        {
            string query = $"SELECT * FROM task_managment.users WHERE status=1";
            Func<MySqlDataReader, List<User>> func = (reader) =>
            {
                List<User> managers = new List<User>();
                while (reader.Read())
                {
                    managers.Add(GlobalLogic.InitUser(reader));
                }
                return managers;
            };
            return DBAccess.RunReader(query, func);

        }

        public static bool AddUser(User user)
        {
            string query;
            query = $"INSERT INTO task_managment.users  " +
                $"(name,user_name,password,email,status,manager)" +
                $" VALUES ('{user.Name}','{user.UserName}'," +
                $"'{user.Password}','{user.EMail}',{user.StatusId},{user.ManagerId})";
            return DBAccess.RunNonQuery(query) == 1;

        }

        public static bool UpdateUser(User user)
        {
            string query;
            //User u = WorkerLogic.GetWorkerDetails(user.Id)[0];
            //if (u.IsActive == false)
            //{
            //    query = $"UPDATE taskmanagment.users SET is_active=1 where user_id={user.Id};";
            //}
            //else
                query = $"UPDATE task_managment.users SET name='{user.Name}', user_name='{user.UserName}'" +
                $", password='{user.Password}' , email='{user.EMail}', status={user.StatusId}, manager={user.ManagerId} WHERE user_id={user.Id}";
            return DBAccess.RunNonQuery(query) == 1;
        }

        public static List<User> GetAllUsers()
        {
            string query = $"SELECT * FROM task_managment.users where is_active=1";

            Func<MySqlDataReader, List<User>> func = (reader) =>
            {
                List<User> users = new List<User>();
                while (reader.Read())
                {
                    users.Add(GlobalLogic.InitUser(reader));
                }
                return users;
            };

            return DBAccess.RunReader(query, func);

        }
       
        public static bool RemoveUser(int id)
        {
            string query;
            query = $"UPDATE task_managment.users SET is_active=0 where user_id={id} AND is_active=1;";
            return DBAccess.RunNonQuery(query) == 1;
        }

        public static bool AddProject(Project project)
        {

            string query = $"INSERT INTO task_managment.projects  " +
                $"(name,customer,team_leader,develop_houres,qa_houres,ui_ux_houres,start_date,end_date)" +
                $" VALUES ('{project.Name}','{project.Customer}'," +
                $"'{project.TeamLeaderId}',{project.DevelopHours},{project.QAHours},{project.UiUxHours}," +
                $"'{project.StartDate.Year}-{project.StartDate.Month}-{project.StartDate.Day}','{project.EndDate.Year}-{project.EndDate.Month}-{project.EndDate.Day}')";
            if (DBAccess.RunNonQuery(query) == 1)
            {
                return AddWorkersToProject(project);
            }
            return false;
        }

        private static bool AddWorkersToProject(Project project)
        {
            var query = $"SELECT project_id FROM projects WHERE name = '{project.Name}'";
            int idProject = (int)DBAccess.RunScalar(query);

            query = $"SELECT user_id FROM Users" +
                    $" WHERE manager={project.TeamLeaderId}";
            List<int> usersId = new List<int>();
            Func<MySqlDataReader, List<int>> func = (reader) =>
            {
                List<int> users = new List<int>();

                while (reader.Read())
                {
                    //Add all the teamLeadr's user to the project
                    users.Add(reader.GetInt32(0));
                }
                return users;
            };

            usersId = DBAccess.RunReader(query, func);

            usersId.ForEach(idUser =>
            {
                var q = $"INSERT INTO user_projects (user_id,project_id) VALUES({idUser},{idProject})";
                DBAccess.RunNonQuery(q);
            });
            return true;
        }

        public static bool addWorkersToProject(int[] ids, string name)
        {
            string q = $"SELECT project_id FROM projects WHERE name = '{name}'";
            int idProject = (int)DBAccess.RunScalar(q);
            foreach (var item in ids)
            {
                string query = $"INSERT INTO task_managment.user_projects ({item},{idProject}) VALUES(4,3)";//////???????????
                if (DBAccess.RunNonQuery(query) == 0)
                    return false;
            }
            return true;
        }

        public static List<User> getAllTeamLeaders()
        {
            string query = $"SELECT * FROM task_managment.users WHERE status=2";
            Func<MySqlDataReader, List<User>> func = (reader) =>
            {
                List<User> teamLeaders = new List<User>();
                while (reader.Read())
                {
                    teamLeaders.Add(GlobalLogic.InitUser(reader));
                }
                return teamLeaders;
            };
            return DBAccess.RunReader(query, func);
        }


        public static List<Project> getAllProjects()
        {
            string query = $"SELECT * FROM task_managment.projects";
            Func<MySqlDataReader, List<Project>> func = (reader) =>
            {
                List<Project> projects = new List<Project>();
                while (reader.Read())
                {
                    projects.Add(GlobalLogic.InitProject(reader));
                }
                return projects;
            };
            return DBAccess.RunReader(query, func);
        }
    }
}
