using _01_BOL;
using _00_DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace _02_BLL
{
    public static class TeamLeaderLogic
    {

        public static List<Project> GetProjectDeatails(int teamLeaderId)
        {
            string query = $"SELECT * FROM task_managment.projects WHERE team_leader={teamLeaderId}";

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

        public static List<User> GetWorkersDeatails(int teamLeaderId)
        {
            string query = $"SELECT * FROM task_managment.users WHERE manager={teamLeaderId}";

            Func<MySqlDataReader, List<User>> func = (reader) =>
            {
                List<User> workers = new List<User>();
                while (reader.Read())
                {
                    workers.Add(GlobalLogic.InitUser(reader));
                }
                return workers;
            };
            return DBAccess.RunReader(query, func);

        }

        public static List<WorkerHours> getWorkersHours(int projectId)
        {
            string query = $"SELECT name,date,SEC_TO_TIME(TIME_TO_SEC(end) - TIME_TO_SEC(start)) AS Time , allocated_hours" +
        $" FROM users u JOIN user_projects up ON u.user_id=up.user_id LEFT JOIN daily_presence dp ON up.user_project_id= dp.user_project_id" +
       $" WHERE up.project_id= {projectId}" +
        $" GROUP BY name,allocated_hours,date" +
        $" ORDER BY name,Time desc";

            Func<MySqlDataReader, List<WorkerHours>> func = (reader) =>
            {
                List<WorkerHours> unknowns = new List<WorkerHours>();
                while (reader.Read())
                {
                    string s = reader[3].ToString();
                    int.TryParse(s, out int x);
                    string s2;
                    try
                    {
                        TimeSpan t = reader.GetTimeSpan(2);
                        s2 = (t.Hours + t.Days * 24) + ":" + t.Minutes;
                    }
                    catch { s2 = 0 + ":" + 0; };
                    DateTime.TryParse(reader[1].ToString(), out DateTime d);
                    unknowns.Add(new WorkerHours
                    {

                        Name = reader.GetString(0),
                        Date = d,
                        Hours = s2,
                        allocatedHours = x
                    });
                }
                return unknowns;

            };
            return DBAccess.RunReader(query, func);

        }
        public static List<WorkerHours> getWorkerHours(int teamLeaderId, int workerId)
        {
            string query = $"SELECT up.user_project_id, p.name , allocated_hours , SEC_TO_TIME(SUM(TIME_TO_SEC(end) - TIME_TO_SEC(start)))" +
            $" FROM user_projects UP  join projects p on p.project_id=up.project_id" +
            $" LEFT join daily_presence DP on up.user_project_id= dp.user_project_id" +
            $" where p.team_leader={teamLeaderId} and up.user_id= {workerId}" +
            $" group by up.user_project_id, p.name,allocated_hours";

            Func<MySqlDataReader, List<WorkerHours>> func = (reader) =>
            {
                List<WorkerHours> unknowns = new List<WorkerHours>();
                while (reader.Read())
                {
                    string s = reader[2].ToString();
                    int.TryParse(s, out int x);
                    string s2 = reader[3].ToString();
                    unknowns.Add(new WorkerHours
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        allocatedHours = x,
                        Hours = s2
                    });
                }
                return unknowns;

            };
            return DBAccess.RunReader(query, func);

        }

        public static bool UpdateWorkerHours(int projectWorkerId, int numHours)
        {
            string query = $"UPDATE task_managment.user_projects SET allocated_hours={numHours} WHERE user_project_id={projectWorkerId}";
            return DBAccess.RunNonQuery(query) == 1;
        }

        public static string GetHours(int projectId)
        {
            string query = $"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(end) - TIME_TO_SEC(start))) FROM task_managment.daily_presence WHERE user_project_id IN" +
                $"(SELECT user_project_id FROM task_managment.user_projects" +
                $" WHERE project_id={projectId});";
            return DBAccess.RunScalar(query).ToString();
        }

    }
}
