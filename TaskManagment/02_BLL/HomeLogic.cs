using _00_DAL;
using _01_BOL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Timers;

namespace _02_BLL
{
    public static class HomeLogic
    {
          
        public static User Login(string userName, string password)
        {
            string query = $"SELECT * FROM task_managment.users WHERE user_name='{userName}' and password='{password}' ";
            Func<MySqlDataReader, List<User>> func = (reader) => {
                List<User> userList = new List<User>();
                while (reader.Read())
                {
                    userList.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        UserName = reader.GetString(2),
                        Password = reader.GetString(3),
                        EMail = reader.GetString(4),
                        StatusId = reader.GetInt32(5),
                        ManagerId = reader[6] as int?
                    });
                }
                return userList;
            };
            List<User> Users= DBAccess.RunReader(query, func);
            if (Users != null && Users.Count > 0)
                return Users[0];
            return null;
        }
        public static bool sendEmail(string sub, string body, string email)
        {
            
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("rachel.novak@seldatinc.com");
            msg.To.Add(new MailAddress(email));
            msg.Subject = sub;
            msg.Body = body;
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential("rachel.novak@seldatinc.com", "0556774766");
            client.Timeout = 20000;
            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                var e = ex;
                return false;
            }
            finally
            {
                msg.Dispose();
            }
        }
        public static void OnStart(object sender, ElapsedEventArgs args)
        {

            string query = $"SELECT u.email, p.name ,p.end_date" +
            $" FROM users u JOIN user_projects up ON u.user_id = up.user_id JOIN projects p" +
            $" ON p.project_id = up.project_id" +
            $" WHERE p.end_date <= NOW() + INTERVAL 1 DAY" +
             $" UNION " +
             $" SELECT u.email, p.name ,p.end_date" +
            $" FROM users u JOIN projects p ON p.team_leader = u.user_id" +
             $" WHERE p.end_date <= NOW() + INTERVAL 1 DAY";

            Func<MySqlDataReader, List<object>> func = (reader) => {
                List<object> objects = new List<object>();
                while (reader.Read())
                {
                    var o = new
                    {
                        email = reader.GetString(0),
                        name = reader.GetString(1),
                        end_date = Convert.ToDateTime(reader.GetString(2)),
                    };
                    sendEmail("Notification do not end task", $"the project {o.name} end in {o.end_date}!!", o.email);
                }
                return objects;
            };
            List<object> objects2 = DBAccess.RunReader(query, func);

        }
        public static void Notifications()
        {
            OnStart(null, null);
            Timer timer = new Timer();
            timer.Interval = 60000 * 60 * 24; // once a day
            timer.Elapsed += new ElapsedEventHandler(OnStart);
            timer.Start();
        }

    }
}
