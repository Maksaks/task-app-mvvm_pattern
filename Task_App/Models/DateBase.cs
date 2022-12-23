using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Windows;
using System.IO;

namespace Task_App.Models
{
    /*
     USERS
    id - int
    login - text
    password - text
    email - text
    task_ids - text - format: 1,2,3,4,5,6,7
    */
    public class User {
        public int id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string task_ids { get; set; }

        public User(){}
        public User(int id, string login, string password, string email, string task_ids) {
            this.id = id;
            this.login = login;
            this.password = password;
            this.email = email;
            this.task_ids = task_ids;
        }
    }

    /*
    TASKINFO
    ID - int
    Name - text
    SubTasks - text - Format: Task1-true/false;Task2-true/false;
    TimeFrom - int
    TimeBefore - int
    Info - text
    Progress - int
    Priority - int
    Complexity - int
    Resources - text
    Status - int
    admin_id - int
    users_id - text
     */

    public class TaskInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string SubTasks { get; set; }
        public string TimeFrom { get; set; }
        public string TimeBefore { get; set; }
        public string Info { get; set; }
        public string Progress { get; set; }
        public string Priority { get; set; }
        public string Complexity { get; set; }
        public string Resources { get; set; }
        public string Status { get; set; }
        public string admin_id { get; set; }
        public string users_id { get; set; }

        public TaskInfo() { }

        public TaskInfo(string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Resources, string Status, string admin_id, string users_id) {
            this.Name = Name;
            this.SubTasks = SubTasks;
            this.TimeFrom = TimeFrom;
            this.TimeBefore = TimeBefore;
            this.Info = Info;
            this.Progress = Progress;
            this.Priority = Priority;
            this.Complexity = Complexity;
            this.Resources = Resources;
            this.Status = Status;
            this.admin_id = admin_id;
            this.users_id = users_id;
        }

    }



    public class DateBase
    {
        private SQLiteConnection db;
        public DateBase()
        {
            try
            {
                if(!File.Exists(@"C:\Users\Gyrocopter_UA\source\repos\Task_App\Task_App\DataBase.db")) throw new NoConnectionWithDB();
                db = new SQLiteConnection(@"Data Source=C:\Users\Gyrocopter_UA\source\repos\Task_App\Task_App\DataBase.db;Version=3;");
                db.Open();
            }
            catch (TaskAppException e)
            {
                MessageBox.Show(e.msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        ~DateBase()
        {
            try
            {
                
                db.Close();
            }
            catch(Exception e)
            {

            }
        }

        //Проверка логина в базе(для регистрации или проверки наличия - неверности пароля)
        public bool CheckUserLogin(string login)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM Users WHERE login='{login}'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        //Проверка наличия и получения данных в базе(Вход)
        public User GetUser(string login, string password)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM Users WHERE login='{login}' AND password='{password}'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User tmp = new User(Convert.ToInt32(reader["id"]), reader["login"].ToString(), reader["password"].ToString(), reader["email"].ToString(), reader["task_ids"].ToString());
                    reader.Close();
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetUser(string login)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM Users WHERE login='{login}'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User tmp = new User(Convert.ToInt32(reader["id"]), reader["login"].ToString(), reader["password"].ToString(), reader["email"].ToString(), reader["task_ids"].ToString());
                    reader.Close();
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        //Получение по айди
        public User GetUser(int ID)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM Users WHERE id = {ID}";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    User tmp = new User(Convert.ToInt32(reader["id"]), reader["login"].ToString(), reader["password"].ToString(), reader["email"].ToString(), reader["task_ids"].ToString());
                    reader.Close();
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        //Добавление пользователя(для регистрации)
        public bool AddUser(string login, string password, string email, string task_ids)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"INSERT INTO Users(login, password, email, task_ids) VALUES('{login}', '{password}', '{email}', '{task_ids}')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        // Обновление данных пользователя(структура с названиями полей)
        public bool UpdateUserInfo(string last_login, string login, string password, string email, string task_ids)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"UPDATE Users SET login='{login}', password='{password}', email='{email}', task_ids='{task_ids}' WHERE login = '{last_login}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }


        //Удаление пользователя
        public bool DeleteUser(string login)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"DELETE FROM Users WHERE login = '{login}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }


        //Добавление задачи
        public bool AddTask(string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Resources, string Status, string admin_id, string users_id)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"INSERT INTO TaskInfos(Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, admin_id, users_id) VALUES('{Name}', '{SubTasks}', '{TimeFrom}', '{TimeBefore}', '{Info}', '{Progress}', '{Priority}', '{Complexity}', '{Resources}', '{Status}', '{admin_id}', '{users_id}')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        //Удаление задачи
        public bool DeleteTask(int ID)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"DELETE FROM TaskInfos WHERE ID = '{ID}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
            
        }
        public bool DeleteUserTasks(int userID)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"DELETE FROM TaskInfos WHERE admin_id = '{userID}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
        //Обновление задачи(структура с названиями полей)


        public bool UpdateTask(int ID, string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Resources, string Status, string admin_id, string users_id)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"UPDATE TaskInfos SET Name='{Name}', SubTasks='{SubTasks}', TimeFrom='{TimeFrom}', TimeBefore='{TimeBefore}', Info='{Info}', Progress='{Progress}', Priority='{Priority}', Complexity='{Complexity}', Resources='{Resources}', Status='{Status}', admin_id='{admin_id}', users_id='{users_id}' WHERE ID = '{ID}'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        //Получение задачи
        public TaskInfo GetTask(int ID)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM TaskInfos WHERE id = {ID}";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    TaskInfo tmp = new TaskInfo(reader["Name"].ToString(), reader["SubTasks"].ToString(), Convert.ToString(reader["TimeFrom"]), Convert.ToString(reader["TimeBefore"]),
                        reader["Info"].ToString(), Convert.ToString(reader["Progress"]), Convert.ToString(reader["Priority"]), Convert.ToString(reader["Complexity"]),
                        reader["Resources"].ToString(), Convert.ToString(reader["Status"]), Convert.ToString(reader["admin_id"]), reader["users_id"].ToString());
                    tmp.ID = Convert.ToInt32(reader["ID"]);
                    reader.Close();
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        // проверка уникальности имени задания
        public bool CheckTaskName(string Name)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM TaskInfos WHERE Name = '{Name}'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                
                if (reader.HasRows)
                {
                    reader.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public TaskInfo GetTask(string Name)
        {
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
                cmd.CommandText = $@"SELECT * FROM TaskInfos WHERE Name = '{Name}'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    TaskInfo tmp = new TaskInfo(reader["Name"].ToString(), reader["SubTasks"].ToString(), Convert.ToString(reader["TimeFrom"]), Convert.ToString(reader["TimeBefore"]),
                        reader["Info"].ToString(), Convert.ToString(reader["Progress"]), Convert.ToString(reader["Priority"]), Convert.ToString(reader["Complexity"]),
                        reader["Resources"].ToString(), Convert.ToString(reader["Status"]), Convert.ToString(reader["admin_id"]), reader["users_id"].ToString());
                    tmp.ID = Convert.ToInt32(reader["ID"]);
                    reader.Close();
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}