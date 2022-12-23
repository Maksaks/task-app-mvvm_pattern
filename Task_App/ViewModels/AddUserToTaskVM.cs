using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Task_App.Models;
using Task_App.Views;

namespace Task_App.ViewModels
{
    class AddUserToTaskVM : BaseViewModel, INotifyDataErrorInfo
    {
        public AddUserToTaskWindow window;
        public TaskControllerSystem controller;
        private string _UserAddName;
        public string UserAddName {
            get => _UserAddName;
            set {
                _UserAddName = value;
                AddUserCommand?.RaiseCanExecuteChanged();
            } 
        }
        public DelegateCommand AddUserCommand { get; }
        public AddUserToTaskVM(TaskControllerSystem system)
        {
            window = new AddUserToTaskWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            AddUserCommand = new DelegateCommand(AddUser);

            controller = system;
            window.ShowDialog();
        }

        private void AddUser(object obj)
        {
            try
            {
                if (controller.GetSelectTask().users_id.Split(';').Length == 10) throw new OutOfLimitUserOnTask();
            }
            catch(TaskAppException e)
            {
                MessageBox.Show(e.msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (controller.CheckUserLogin(UserAddName) && !controller.GetSelectTask().users_id.Contains("-"+UserAddName+";") && controller.GetSelectTask().admin_id != controller.userManager.GetUser(UserAddName).id.ToString())
            {
                TaskInfo tmp = controller.GetSelectTask();
                User us = controller.userManager.GetUser(UserAddName);
                if(us.task_ids.Length == 0)
                {
                    us.task_ids = tmp.ID.ToString();
                }
                else
                {
                    us.task_ids = us.task_ids + "," + tmp.ID.ToString();
                }
                
                if (controller.UpdateInfoUser(us))
                {
                    if (tmp.users_id.Length == 0)
                    {
                        if (controller.EditTaskInfoOwner(tmp.Name, tmp.Name, tmp.SubTasks, tmp.TimeFrom, tmp.TimeBefore, tmp.Info, tmp.Progress, tmp.Priority,
                        tmp.Complexity, tmp.Status, tmp.Resources, tmp.users_id + "-" + UserAddName + ";"))
                        {
                            window.Close();
                        }
                    }
                    else
                    {
                        if (controller.EditTaskInfoOwner(tmp.Name, tmp.Name, tmp.SubTasks, tmp.TimeFrom, tmp.TimeBefore, tmp.Info, tmp.Progress, tmp.Priority,
                                                tmp.Complexity, tmp.Status, tmp.Resources, tmp.users_id + "\n-" + UserAddName + ";"))
                        {
                            window.Close();
                        }
                    }
                    
                }
            }
            else
            {
                ClearErrors(nameof(UserAddName));
                if (!controller.CheckUserLogin(UserAddName))
                {
                    AddError(nameof(UserAddName), $"Користувач з логіном {UserAddName} незареєстрован");
                }
                else if (controller.GetSelectTask().users_id.Contains("-" + UserAddName + ";"))
                {
                    AddError(nameof(UserAddName), $"Користувач з логіном {UserAddName} вже доданий до завдання");
                }
                else if (controller.GetSelectTask().admin_id == controller.userManager.GetUser(UserAddName).id.ToString())
                {
                    AddError(nameof(UserAddName), "Творець завжди має доступ до свого завдання!");
                }
            }
        }
        public bool HasErrors => _errorsByPropertyName.Any();
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
            _errorsByPropertyName[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

    }
}
