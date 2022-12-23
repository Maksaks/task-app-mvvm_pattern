using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Task_App.Models;
using Task_App.Views;

namespace Task_App.ViewModels
{
    class CreateTaskVM : BaseViewModel, INotifyDataErrorInfo
    {
        CreateTaskWindow window;
        TaskControllerSystem controllerSystem;
        private string _Name;
        public string Name {
            get => _Name;
            set { _Name = value;
                ConfirmCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        public string SubTasks { get; set; }
        public string TimeFrom { get; set; }
        private string _TimeBefore;
        public string TimeBefore
        {
            get => _TimeBefore;
            set
            {
                string[] mas = value.Split(' ').First().Split('/');
                _TimeBefore = mas[1] + "." + mas[0] + "." + mas[2];
                OnPropertyChanged();
            }
        }
        public string Info { get; set; }
        public string Progress { get; set; }
        public string Priority { get; set; }
        public string Complexity { get; set; }
        public string Resources { get; set; }
        public string Status { get; set; }
        public string admin_id { get; set; }
        public string users_id { get; set; }

        public DelegateCommand ConfirmCommand { get; }

        public CreateTaskVM(TaskControllerSystem tmp)
        {
            window = new CreateTaskWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            ConfirmCommand = new DelegateCommand(ConfirmForm, CanConfirm);
            controllerSystem = tmp;
            Status = "InWork";
            users_id = "";
            Progress = "0%";
            TimeFrom = DateTimeOffset.UtcNow.ToString("dd/MM/yyyy");
            window.ShowDialog();
        }

        private bool CanConfirm(object obj)
        {
            ClearErrors(nameof(Name));
            if (controllerSystem.taskManager.CanTakeName(Name) && Name != null)
            {
                if (Name.Length != 0)
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
                if (Name != null)
                {
                    if (Name.Length != 0) AddError(nameof(Name), $"Назва {Name} вже зайнята");
                    else AddError(nameof(Name), "Обов'язкове поле");
                }
                return false;
            }
        }

        private void ConfirmForm(object obj)
        {
            ClearErrors(nameof(Name));
            ClearErrors(nameof(TimeBefore));
            try
            {
                if (Name.Length == 0) throw new NoFillInputs();
                if (TimeBefore == null) { AddError(nameof(TimeBefore), "Необхідно заповнити поле!"); return; }
                if (DateTime.Parse(TimeFrom) > DateTime.Parse(TimeBefore)) throw new InCorrectDateInput();
                if (controllerSystem.CreateTask(Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Status, Resources, users_id))
                {
                    window.Close();
                }
            }
            catch (InCorrectDateInput e)
            {
                AddError(nameof(TimeBefore), e.msg + $" {TimeFrom} < {TimeBefore}!!!");
            }
            catch (NoFillInputs e)
            {
                AddError(nameof(Name), e.msg);
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
