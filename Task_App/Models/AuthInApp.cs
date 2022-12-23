using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Task_App.Models
{
    public class AuthInApp
    {
        private static DateBase db;

        public AuthInApp()
        {
            db = new DateBase();
        }
        // Регистрация пользователя без входа в систему, после регистрации выполнение входа
        public bool CheckUserLogin(string login) => db.CheckUserLogin(login);
        public User GetUser(string login, string password) => db.GetUser(login, password);
        public string SignUp(string login, string password, string email, string task_ids="")
        {
            if (!db.CheckUserLogin(login)) // если нету
            {
                if (password.Length >= 8) // валидация пароля - 8+ and number one plus
                {
                    bool pas_cor = false;
                    foreach(char ch in password)
                    {
                        if (Convert.ToInt32(ch) >= 48 && Convert.ToInt32(ch) <= 57)
                        {
                            pas_cor = true;
                            break;
                        }
                    }
                    if (pas_cor){
                        db.AddUser(login, password, email, task_ids);
                        return "Вдала реєстрація!";
                    }
                    else
                    {
                        return "Пароль має містити хоча б одну цифру";
                    }
                }
                else
                {
                    return "Пароль має містити понад 8 символів";
                }
            }
            else
            {
                try
                {
                    throw new InCorrectDate();
                }
                catch (TaskAppException e)
                {
                    MessageBox.Show(e.msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return $"Користувач з логіном {login} вже зареєстрован у системі";
            }
        }
        // вход в систему, возвращает или данные пользователя, или сообщение ошибки
        public object LogIn(string login, string password)
        {
            if (db.CheckUserLogin(login))
            {
                User temp = db.GetUser(login, password);
                if (temp != null)
                {
                    return temp;
                }
                else
                {
                    return "Невірний пароль!";
                }
            }
            else
            {
                try
                {
                    throw new InCorrectDate();
                }
                catch (TaskAppException e)
                {
                    MessageBox.Show(e.msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return $"Користувач з логіном {login} не зареєстрован у системі";
            }
        }
    }
}
