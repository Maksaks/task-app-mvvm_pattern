using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Task_App.Models;
using Task_App.Views;
using System.Windows;
using System.Windows.Input;

namespace Task_App.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private static Timer timer;
        private TaskControllerSystem controller;
        private string _CurrentUser;
        private TaskInfo _SelectedTask;
        private string _Time;
        private string _Date;


        public TaskInfo SelectedTask
        {
            get => _SelectedTask;
            set
            {
                _SelectedTask = value;
                InfoVis = true;
                OnPropertyChanged();
                EditTaskCommand?.RaiseCanExecuteChanged();
                DeleteTaskCommand?.RaiseCanExecuteChanged();
                AddUserToTaskCommand?.RaiseCanExecuteChanged();
                CompletedTaskCommand?.RaiseCanExecuteChanged();
                controller.taskManager.SelectTask(value);
            }
        }
        public string CurrentUser
        {
            get => _CurrentUser;
            set
            {
                _CurrentUser = value;
                OnPropertyChanged();
            }
        }
        public string Time
        {
            get => _Time;
            set
            {
                _Time = value;
                OnPropertyChanged();
            }
        }
        public string Date
        {
            get => _Date;
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }
        // VISIBILITY
        private bool _InfoVis;
        public bool InfoVis {
            get => _InfoVis;
            set
            {
                _InfoVis = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TaskInfo> listTasks { get; set; }

        // COMMAND

        public ICommand AddTaskCommand { get; }

        public DelegateCommand EditTaskCommand { get; }
        public DelegateCommand DeleteTaskCommand { get; }
        public DelegateCommand AddUserToTaskCommand { get; }
        public DelegateCommand CompletedTaskCommand { get; }
        public ICommand EditUserInfoCommand { get; }
        public ICommand SortListCommand { get; }
        public ICommand FilterListCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand DeleteUserCommand { get; }
        

        public MainViewModel()
        {
            // LOGIN
            User us = new User();

            LogInVM logIn = new LogInVM(us);

            controller = new TaskControllerSystem(us);
            if (us.login == null) Application.Current.Shutdown();
            CurrentUser = us.login;

            InfoVis = false;
            listTasks = new ObservableCollection<TaskInfo>();
            
            
            // timer
            timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            

            // COMMAND INIT
            AddTaskCommand = new DelegateCommand(CreateTask);
            EditTaskCommand = new DelegateCommand(EditTask, CanEdit);
            EditUserInfoCommand = new DelegateCommand(EditUserInfo);
            DeleteTaskCommand = new DelegateCommand(DeleteTask, CanDelete);
            AddUserToTaskCommand = new DelegateCommand(AddUser, CanAddUser);
            CompletedTaskCommand = new DelegateCommand(CompletedTask, CanCompleted);
            SortListCommand = new DelegateCommand(SortList);
            FilterListCommand = new DelegateCommand(FilterList);
            ReloadCommand = new DelegateCommand(ReloadList);
            LogOutCommand = new DelegateCommand(LogOut);
            DeleteUserCommand = new DelegateCommand(DeleteUser);
            // MAKE LIST TASKS
            MakeListTasks();
            
        }

        private bool CanCompleted(object obj)
        {
            if (SelectedTask != null)
            {
                if (controller.currentUser.id.ToString() == SelectedTask.admin_id)
                {
                    return true;
                }
            }
            return false;
        }

        private void CompletedTask(object obj)
        {
            controller.CompletedTask(SelectedTask.ID);
            MakeListTasks();
        }

        private void DeleteUser(object obj)
        {
            if(controller.currentUser.task_ids.Length != 0)
            {
                controller.DeleteUserTasks(controller.currentUser.id);
            }
            controller.DeleteUser(controller.currentUser.login, controller.currentUser.password);
            LogOut(obj);
        }

        private void LogOut(object obj)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void ReloadList(object obj)
        {
            MakeListTasks();
        }

        private void FilterList(object obj)
        {
            string type = obj as string;
            if (type == "Меньш ніж один день")
            {
                controller.taskManager.FilterListTasks(TypeFilters.TIME_BEFORE_EQ_DAY);
                listTasks.Clear();
                foreach(TaskInfo task in controller.taskManager.GetFilterListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "Закінчився час")
            {
                controller.taskManager.FilterListTasks(TypeFilters.TIME_BEFORE_EXPIRED);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetFilterListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "Найбільш пріоритетні")
            {
                controller.taskManager.FilterListTasks(TypeFilters.PRIORITY_MAX);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetFilterListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "Найважчі")
            {
                controller.taskManager.FilterListTasks(TypeFilters.COMPLEXITY_MAX);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetFilterListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "Власні")
            {
                controller.taskManager.FilterListTasks(TypeFilters.SELF);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetFilterListTasks())
                {
                    listTasks.Add(task);
                }
            }
        }

        private void SortList(object obj)
        {
            string type = obj as string;
            if (type == "За ім'ям")
            {
                controller.taskManager.SortListTasks(TypeSorts.NAME);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetSortListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "За часом")
            {
                controller.taskManager.SortListTasks(TypeSorts.TIME_BEFORE);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetSortListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "За пріоритетом")
            {
                controller.taskManager.SortListTasks(TypeSorts.PRIORITY);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetSortListTasks())
                {
                    listTasks.Add(task);
                }
            }
            else if (type == "За статусом")
            {
                controller.taskManager.SortListTasks(TypeSorts.STATUS);
                listTasks.Clear();
                foreach (TaskInfo task in controller.taskManager.GetSortListTasks())
                {
                    listTasks.Add(task);
                }
            }
        }

        private bool CanAddUser(object obj)
        {
            if (SelectedTask != null)
            {
                if (controller.currentUser.id.ToString() == SelectedTask.admin_id)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddUser(object obj)
        {
            AddUserToTaskVM vm = new AddUserToTaskVM(controller);
            MakeListTasks();

            InfoVis = false;
        }

        private bool CanDelete(object obj)
        {
            if(SelectedTask != null)
            {
                if(controller.currentUser.id.ToString() == SelectedTask.admin_id)
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteTask(object obj)
        {
            if (controller.DeleteTask(SelectedTask.ID))
            {
                MakeListTasks();
            }
            InfoVis = false;
        }

        private void MakeListTasks()
        {
            
            listTasks.Clear();
            foreach (TaskInfo tmp in controller.GetListTasksUser())
            {
                listTasks.Add(tmp);
            }
            InfoVis = false;
        }

        private void EditUserInfo(object obj)
        {
            EditUserInfoVM edit = new EditUserInfoVM(controller, controller.currentUser);
            CurrentUser = controller.currentUser.login;
            InfoVis = false;
        }

        private bool CanEdit(object obj)
        {
            return SelectedTask != null;
        }

        private void EditTask(object obj)
        {
            if(controller.currentUser.id.ToString() == SelectedTask.admin_id)
            {
                EditTaskVM editTask = new EditTaskVM(controller, SelectedTask);
            }
            else
            {
                EditTaskUserVM editTask = new EditTaskUserVM(controller, SelectedTask);
            }
            MakeListTasks();
            InfoVis = false;
        }

        private void CreateTask(object obj)
        {
            CreateTaskVM createTask = new CreateTaskVM(controller);
            MakeListTasks();
            InfoVis = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Time = e.SignalTime.Hour.ToString() + ":";
            if(e.SignalTime.Minute.ToString().Length == 1)
            {
                Time += "0" + e.SignalTime.Minute.ToString();
            }
            else
            {
                Time += e.SignalTime.Minute.ToString();
            }
            
            Date = e.SignalTime.Date.ToString("dddd, dd MMMM yyyy");
        }
    }
}
