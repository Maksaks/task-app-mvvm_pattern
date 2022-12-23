using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_App.Models;

namespace Task_App
{
    class TaskControllerSystem
    {
        public UserManager userManager;
        public UserTaskManager taskManager;
        private User _user;
        public User currentUser
        {
            get => _user;
            set
            {
                _user = value;
                userManager = new UserManager(value);
                taskManager = new UserTaskManager(value);
            }
        }


        public TaskControllerSystem(User user)
        {
            userManager = new UserManager(user);
            taskManager = new UserTaskManager(user);
            this.currentUser = user;
        }
        public bool CheckUserLogin(string login) => userManager.CheckUserLogin(login);
        public string UpdateUserInfo(string oldlogin, string login, string password, string email, string task_ids)
        {
            (string, User) date = userManager.UpdateUserInfo(oldlogin, login, password, email, task_ids);
            if (!userManager.EqUsers(date.Item2))
            {
                currentUser = date.Item2;
                userManager = new UserManager(currentUser);
                taskManager = new UserTaskManager(currentUser);
            }
            return date.Item1;
        }
        public bool UpdateInfoUser(User us) => userManager.UpdateInfoUser(us);
        public bool DeleteUser(string login, string password) => userManager.DeleteUser(login, password);
        public void DeleteUserTasks(int userID) => taskManager.DeleteUserTasks(userID);
        public bool SelectTask(int ID) => taskManager.SelectTask(ID);
        
        public TaskInfo GetSelectTask() => taskManager.GetSelectTask();
        public List<TaskInfo> GetListTasksUser() { currentUser = userManager.GetUser(currentUser.id); return taskManager.GetListTasks(); }
        public void UpdateUserTasks() => taskManager.UpdateUserTasks();

        public bool CanTakeTaskName(string Name) => taskManager.CanTakeName(Name);

        public bool CreateTask(string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Status, string Resources = "", string users_id = "")
        {
            if(taskManager.CreateTask(Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Status, Resources, users_id))
            {
                if(currentUser.task_ids.Length == 0)
                {
                    userManager.UpdateUserInfo(currentUser.login, currentUser.login, currentUser.password, currentUser.email, taskManager.GetTaskByName(Name).ID.ToString());
                }
                else
                {
                    userManager.UpdateUserInfo(currentUser.login, currentUser.login, currentUser.password, currentUser.email, currentUser.task_ids + "," + taskManager.GetTaskByName(Name).ID.ToString());
                }
                
                return true;
            }
            
            return false;
        }
        public bool EditTaskInfoOwner(string old_name, string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Status, string Resources = "", string users_id = "")
        {
            return taskManager.EditTaskInfoOwner(old_name, Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Status, Resources, users_id);
        }
        public bool EditTaskInfoUser(string SubTasks, string Progress)
        {
            return taskManager.EditTaskInfoUser(SubTasks, Progress);
        }
        public bool DeleteTask(int TaskID) => taskManager.DeleteTask(TaskID);
        public bool CompletedTask(int taskID) => taskManager.CompletedTask(taskID);
    }
}
