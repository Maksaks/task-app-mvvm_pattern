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
    class SignInVM : BaseViewModel, INotifyDataErrorInfo
    {
        private string _LOGIN;
        public string LOGIN
        {
            get => _LOGIN;
            set
            {
                _LOGIN = value;
                SignInCommand?.RaiseCanExecuteChanged();
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
                SignInCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private string _PASSWORD2;
        public string PASSWORD2
        {
            get => _PASSWORD2;
            set
            {
                _PASSWORD2 = value;
                SignInCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        private string _EMAIL;
        public string EMAIL
        {
            get => _EMAIL;
            set
            {
                _EMAIL = value;
                SignInCommand?.RaiseCanExecuteChanged();
            }
        }
        public SignInWindow window;
        public User tmp;
        public AuthInApp auth;
        public DelegateCommand SignInCommand { get; }
        public SignInVM(User user, AuthInApp ath)
        {
            LOGIN = "";
            PASSWORD = "";
            PASSWORD2 = "";
            EMAIL = "";
            window = new SignInWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            SignInCommand = new DelegateCommand(SignIn, CanSignIn);
            tmp = user;
            auth = ath;

            window.ShowDialog();
        }

        private bool CanSignIn(object obj)
        {
            ClearErrors(nameof(LOGIN));
            if (!auth.CheckUserLogin(LOGIN) && PASSWORD == PASSWORD2)
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
                        ClearErrors(nameof(PASSWORD2));
                        return true;
                    }
                    else
                    {
                        AddError(nameof(PASSWORD), "Пароль має містити хоча б одну цифру");
                        ClearErrors(nameof(PASSWORD2));
                        return false;
                    }
                }
                else
                {
                    if(PASSWORD != "")AddError(nameof(PASSWORD), "Пароль має містити понад 8 символів");
                    return false;
                }
            }
            else
            {
                ClearErrors(nameof(PASSWORD));
                ClearErrors(nameof(PASSWORD2));
                if (PASSWORD != PASSWORD2) AddError(nameof(PASSWORD2), "Паролі не співпадають");
                else AddError(nameof(LOGIN), $"Користувач з логіном {LOGIN} вже зареєстрован у системі");
                return false;
            }
        }

        private void SignIn(object obj)
        {
            string msg = auth.SignUp(LOGIN, PASSWORD, EMAIL);
            if(msg == "Вдала реєстрація!")
            {
                User temp = auth.GetUser(LOGIN, PASSWORD);
                tmp.id = temp.id;
                tmp.login = temp.login;
                tmp.password = temp.password;
                tmp.email = temp.email;
                tmp.task_ids = temp.task_ids;
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
