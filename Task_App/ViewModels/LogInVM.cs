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
    class LogInVM : BaseViewModel, INotifyDataErrorInfo
    {
        private string _LOGIN;

        public string LOGIN
        {
            get => _LOGIN;
            set
            {
                _LOGIN = value;
                LogInCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private string _PASSWORD;
        public string PASSWORD
        {
            get => _PASSWORD;
            set
            {
                _PASSWORD = value;
                LogInCommand?.RaiseCanExecuteChanged();
            }
        }
        public LogInWindow window;
        public User temp;
        public AuthInApp auth;

       

        public DelegateCommand LogInCommand { get; }
        public ICommand GoSignInCommand { get; }

        

        public LogInVM(User us)
        {
            window = new LogInWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            auth = new AuthInApp();

            LogInCommand = new DelegateCommand(LogIn, CanLogIn);
            GoSignInCommand = new DelegateCommand(GoSignIn);
            temp = us;
            window.ShowDialog();
        }

        private void GoSignIn(object obj)
        {
            window.Close();
            SignInVM vm = new SignInVM(temp, auth);
            
        }

        private bool CanLogIn(object obj)
        {
            if (auth.CheckUserLogin(LOGIN))
            {
                ClearErrors(nameof(LOGIN));
                return true;
            }
            else
            {
                if (LOGIN != null)
                {
                    if (LOGIN.Length != 0 && LOGIN != "")
                    {
                        ClearErrors(nameof(LOGIN));
                        AddError(nameof(LOGIN), $"Користувач з логіном {LOGIN} не зареєстрован у системі");
                    }
                    else ClearErrors(nameof(LOGIN));
                }
                
                return false;
            }
        }

        private void LogIn(object obj)
        {
            object ob = auth.LogIn(LOGIN, PASSWORD);
            if (ob is string msg)
            {
                AddError(nameof(PASSWORD), msg);
            }
            else if (ob is User us)
            {
                temp.id = us.id;
                temp.login = us.login;
                temp.password = us.password;
                temp.email = us.email;
                temp.task_ids = us.task_ids;
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
