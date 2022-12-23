using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Task_App.Models
{
    
    public class UserTaskManager
    {
        private static DateBase db;
        private TaskInfo selectTask; // выбраная задача
        private User currentUser; // текущий пользователь в системе
        private List<TaskInfo> listUserTasks; // задачи текущего пользователя
        private List<TaskInfo> filter_listUserTasks; // фильтр текущих задач

        public UserTaskManager(User cur)
        {
            db = new DateBase();
            this.currentUser = cur;
            this.selectTask = null;
            this.listUserTasks = new List<TaskInfo>();
            this.filter_listUserTasks = new List<TaskInfo>();
            UpdateUserTasks();
        }
        public void UpdateUserTasks()
        {
            if (currentUser == null) return;
            if (currentUser.task_ids == "" || currentUser.task_ids == null) return;
            listUserTasks.Clear();
            
            int[] TaskIds = Array.ConvertAll(currentUser.task_ids.Split(','), int.Parse);
            if (TaskIds.Length != 0)
            {
                foreach (int ID in TaskIds)
                {
                    TaskInfo task = db.GetTask(ID);
                    if(task != null)
                    {
                        listUserTasks.Add(task);
                    }
                }
            }
        }
        public TaskInfo GetTaskByName(string name) => db.GetTask(name);
        public List<TaskInfo> GetListTasks()
        {
            UpdateUserTasks();
            return listUserTasks;
        }
        public List<TaskInfo> GetFilterListTasks() => filter_listUserTasks;
        public List<TaskInfo> GetSortListTasks() => listUserTasks;
        public void DeleteUserTasks(int userID) => db.DeleteUserTasks(userID);
        public void SelectTask(TaskInfo tmp)
        {
            selectTask = tmp;
        }
        public TaskInfo GetSelectTask() => selectTask;
        // выбор активного задания
        public bool SelectTask(int ID)
        {
            TaskInfo cur = db.GetTask(ID);
            if (cur != null)
            {
                selectTask = cur;
                return true;
            }
            else
            {
                return false;
            }
        }
        // добавление задания
        public bool CreateTask(string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Status, string Resources = "", string users_id="")
        {
            if (!db.CheckTaskName(Name))
            {
                if (db.AddTask(Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, currentUser.id.ToString(), users_id))
                {
                    selectTask = db.GetTask(Name);
                    listUserTasks.Add(selectTask);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        // изменение информации про задание для владельца
        public bool EditTaskInfoOwner(string last_name, string Name, string SubTasks, string TimeFrom, string TimeBefore, string Info, string Progress, string Priority, string Complexity, string Status, string Resources = "", string users_id = "")
        {
            if (selectTask != null && selectTask.admin_id == currentUser.id.ToString())
            {
                if (!db.CheckTaskName(Name) || last_name == Name)
                {
                    int ID = db.GetTask(last_name).ID;
                    listUserTasks.Remove(selectTask);
                    db.UpdateTask(ID, Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, currentUser.id.ToString(), users_id);
                    selectTask = db.GetTask(ID);
                    listUserTasks.Add(selectTask);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool EditTaskInfoUser(string SubTasks, string Progress)
        {
            if (selectTask != null)
            {
                listUserTasks.Remove(selectTask);
                int ID = db.GetTask(selectTask.Name).ID;
                db.UpdateTask(ID, selectTask.Name, SubTasks, selectTask.TimeFrom, selectTask.TimeBefore, selectTask.Info, Progress, selectTask.Priority, selectTask.Complexity, selectTask.Resources, selectTask.Status, selectTask.admin_id, selectTask.users_id);
                selectTask = db.GetTask(ID);
                listUserTasks.Add(selectTask);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanTakeName(string Name)
        {
            if (!db.CheckTaskName(Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // КОД КАТЮХИ

        public bool DeleteTask(int taskID) // видалення
        {
            TaskInfo temp = listUserTasks.Where(task => task.ID == taskID).FirstOrDefault();
            if (temp != null)
            {
                listUserTasks.Remove(temp);
                db.DeleteTask(taskID);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsNumOnly(string str)
        {
            foreach (char c in str)
            {

                if (c != '.' && c != ':' && c != ' ')
                {
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private DateTimeOffset GetTimeBefore(TaskInfo task)
        {
            if (IsNumOnly(task.TimeBefore))
            {
                return DateTimeOffset.Parse(task.TimeBefore);
            }
            return DateTimeOffset.MinValue;
        }
        private int ConvertStringToEnum(string type)
        {
            if(type == "Low")
            {
                return 0;
            }
            else if (type == "Easy")
            {
                return 0;
            }
            else if (type == "Medium")
            {
                return 1;
            }
            else if (type == "High")
            {
                return 2;
            }
            else return 0;
        }
        public bool SortListTasks(TypeSorts type)
        {
            if (type == TypeSorts.NAME)
            {
                listUserTasks = listUserTasks.OrderBy(task => task.Name).ToList();
                return true;
            }
            else if (type == TypeSorts.TIME_BEFORE)
            {
                listUserTasks = listUserTasks.OrderBy(task => GetTimeBefore(task).ToUnixTimeSeconds()).Reverse().ToList();
                return true;
            }
            else if (type == TypeSorts.PRIORITY)
            {
                listUserTasks = listUserTasks.OrderBy(task => ConvertStringToEnum(task.Priority)).Reverse().ToList();
                return true;
            }
            else if (type == TypeSorts.STATUS)
            {
                listUserTasks = listUserTasks.OrderBy(task => task.Status).Reverse().ToList();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CompletedTask(int taskID)
        {
            TaskInfo temp = listUserTasks.Where(task => task.ID == taskID).FirstOrDefault();
            if (temp != null)
            {
                db.UpdateTask(taskID, temp.Name, temp.SubTasks, temp.TimeFrom, temp.TimeBefore, temp.Info, temp.Progress, temp.Priority, temp.Complexity, temp.Resources, "Completed", temp.admin_id, temp.users_id);
                return true;
            }
            else
            {
                return false;
            }
        }

        // ІНОВАЦІЙНІ ФУНКЦІЇ
        public bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {

                if (c != '.' && c != ':' && c != ' ')
                {
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckTimeEqDay(TaskInfo task)
        {
            if (IsDigitsOnly(task.TimeBefore))
            {
                DateTime timebefore = DateTime.Parse(task.TimeBefore);
                if (DateTime.Now >= timebefore.AddDays(-1) && DateTime.Now <= timebefore)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckTimeExpired(TaskInfo task)
        {
            if (IsDigitsOnly(task.TimeBefore))
            {
                DateTime timebefore = DateTime.Parse(task.TimeBefore);
                if (DateTime.Now > timebefore)
                {
                    return true;
                }
            }
            return false;
        }
        public bool FilterListTasks(TypeFilters type)
        {
            if (type == TypeFilters.TIME_BEFORE_EQ_DAY)
            {
                filter_listUserTasks = listUserTasks.Where(task => CheckTimeEqDay(task)).ToList();
                return true;
            }
            else if (type == TypeFilters.TIME_BEFORE_EXPIRED)
            {
                filter_listUserTasks = listUserTasks.Where(task => CheckTimeExpired(task)).ToList();
                return true;
            }
            else if (type == TypeFilters.PRIORITY_MAX)
            {
                filter_listUserTasks = listUserTasks.Where(task => task.Priority == TaskPriority.High.ToString()).ToList();
                return true;
            }
            else if (type == TypeFilters.COMPLEXITY_MAX)
            {
                filter_listUserTasks = listUserTasks.Where(task => task.Complexity == TaskComplexity.Highest.ToString()).ToList();
                return true;
            }
            else if (type == TypeFilters.SELF)
            {
                filter_listUserTasks = listUserTasks.Where(task => task.admin_id == currentUser.id.ToString()).ToList();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
