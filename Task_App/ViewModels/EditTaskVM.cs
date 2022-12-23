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
    class EditTaskVM : BaseViewModel, INotifyDataErrorInfo
    {
        public EditTaskWindow window;
        public string OldName;
        public TaskControllerSystem controllerSystem;
        public string NAME
        {
            get => tmp.Name;
            set
            {
                tmp.Name = value;
                EditTaskCommand?.RaiseCanExecuteChanged();
            }
        }
        public string PROGRESS
        {
            get => tmp.Progress.TrimEnd('%');
            set
            {
                tmp.Progress = value.Split('.').First() + "%";
            }
        }
        public string DATE
        {
            get => tmp.TimeBefore;
            set{
                string[] mas = value.Split(' ').First().Split('/');
                tmp.TimeBefore = mas[1] + "." + mas[0] + "." + mas[2];
                OnPropertyChanged();
            } 
        }
        public TaskInfo tmp { get; set; }
        public DelegateCommand EditTaskCommand { get; }
        public EditTaskVM(TaskControllerSystem contr, TaskInfo task)
        {
            window = new EditTaskWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            OldName = task.Name;
            controllerSystem = contr;
            tmp = task;

            EditTaskCommand = new DelegateCommand(EditTask, CanEdit);

            window.ShowDialog();
        }
        private bool CanEdit(object obj)
        {
            ClearErrors(nameof(NAME));
            if (controllerSystem.taskManager.CanTakeName(NAME) || OldName == NAME)
            {
                if (NAME.Length != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (tmp.Name.Length != 0) AddError(nameof(NAME), $"Назва {NAME} вже зайнята");
                else AddError(nameof(NAME), "Обов'язкове поле");
                
                return false;
            }
        }

        private void EditTask(object obj)
        {
            ClearErrors(nameof(DATE));
            ClearErrors(nameof(NAME));
            if (DateTime.Parse(tmp.TimeFrom) > DateTime.Parse(tmp.TimeBefore))
            {
                AddError(nameof(DATE), $"Некоректне введення дати! {tmp.TimeFrom} < {tmp.TimeBefore}!!!");
                return;
            }
            if (controllerSystem.EditTaskInfoOwner(OldName, tmp.Name, tmp.SubTasks, tmp.TimeFrom, tmp.TimeBefore, tmp.Info, tmp.Progress, tmp.Priority, tmp.Complexity, tmp.Status, tmp.Resources, tmp.users_id))
            {
                window.Close();
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
