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
    class EditUserInfoVM : BaseViewModel, INotifyDataErrorInfo
    {
        public EditUserInfoWindow window;
        public User user { get; set; }
        private string _LOGIN;
        public string LOGIN
        {
            get => _LOGIN;
            set
            {
                _LOGIN = value;
                EdituserInfoCommand?.RaiseCanExecuteChanged();
            }
        }
        private string _PASSWORD;
        public string PASSWORD
        {
            get => _PASSWORD;
            set
            {
                _PASSWORD = value;
                EdituserInfoCommand?.RaiseCanExecuteChanged();
            }
        }
        private string _EMAIL;
        public string EMAIL
        {
            get => _EMAIL;
            set
            {
                _EMAIL = value;
                EdituserInfoCommand?.RaiseCanExecuteChanged();
            }
        }
        public TaskControllerSystem controllerSystem;

        public DelegateCommand EdituserInfoCommand { get; }

        public EditUserInfoVM(TaskControllerSystem contr, User us)
        {
            window = new EditUserInfoWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            LOGIN = us.login;
            PASSWORD = us.password;
            EMAIL = us.email;
            user = us;
            
            controllerSystem = contr;
            EdituserInfoCommand = new DelegateCommand(EditUser, CanEdit);

            window.ShowDialog();
        }

        private bool CanEdit(object obj)
        {
            ClearErrors(nameof(LOGIN));
            if (LOGIN == user.login && PASSWORD == user.password)
            {
                return true;
            }
            if (!controllerSystem.CheckUserLogin(LOGIN) || LOGIN == user.login)
            {
                ClearErrors(nameof(PASSWORD));
                if (PASSWORD.Length >= 8) // валидация пароля - 8+ and number one plus
                {
                    bool pas_cor = false;
                    foreach (char ch in PASSWORD)
                    {
                        if (Convert.ToInt32(ch) >= 48 && Convert.ToInt32(ch) <= 57)
                        {
                            pas_cor = true;
                            break;
                        }
                    }
                    if (pas_cor)
                    {
                        ClearErrors(nameof(LOGIN));
                        ClearErrors(nameof(PASSWORD));
                        return true;
                    }
                    else
                    {
                        AddError(nameof(PASSWORD), "Пароль має містити хоча б одну цифру");
                        return false;
                    }
                }
                else
                {
                    if (PASSWORD != "") AddError(nameof(PASSWORD), "Пароль має містити понад 8 символів");
                    return false;
                }
            }
            else
            {
                if (LOGIN != user.login) AddError(nameof(LOGIN), $"Користувач з логіном {LOGIN} вже зареєстрован у системі");
                return false;
            }
        }

        private void EditUser(object obj)
        {
            if (controllerSystem.UpdateUserInfo(user.login, LOGIN, PASSWORD, EMAIL, user.task_ids) == "Інформацію оновлено!")
            {
                user.login = LOGIN;
                user.password = PASSWORD;
                user.email = EMAIL;
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
