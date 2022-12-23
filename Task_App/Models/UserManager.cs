using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_App.Models
{
    public class UserManager
    {
        private static DateBase db;
        private User currentUser;

        public UserManager(User cur)
        {
            this.currentUser = cur;
            db = new DateBase();
        }
        public bool CheckUserLogin(string login)
        {
            return db.CheckUserLogin(login);
        }
        public User GetUser(int ID) => db.GetUser(ID);
        public User GetUser(string login) => db.GetUser(login);
        public (string, User) UpdateUserInfo(string oldlogin, string login, string password, string email, string task_ids)
        {
            if (!db.CheckUserLogin(login) || oldlogin == login && currentUser.password != password) // если не занят ник
            {
                if (password.Length >= 8) // валидация пароля - 8+ and number one plus
                {
                    bool pas_cor = false;
                    foreach (char ch in password)
                    {
                        if (Convert.ToInt32(ch) >= 48 && Convert.ToInt32(ch) <= 57)
                        {
                            pas_cor = true;
                            break;
                        }
                    }
                    if (pas_cor)
                    {
                        db.UpdateUserInfo(oldlogin, login, password, email, task_ids);
                        currentUser = db.GetUser(login, password);
                        return ("Інформацію оновлено!", currentUser);
                    }
                    else
                    {
                        return ("Пароль має містити хоча б одну цифру", currentUser);
                    }
                }
                else
                {
                    return ("Пароль має містити понад 8 символів", currentUser);
                }
            }
            else if (currentUser.login == login && currentUser.password == password) {
                db.UpdateUserInfo(currentUser.login, login, password, email, task_ids);
                currentUser = db.GetUser(login, password);
                return ("Інформацію оновлено!", currentUser);
            }
            else
            {
                return ($"Користувач з логіном {login} вже зареєстрован у системі", currentUser);
            }
        }

        public bool UpdateInfoUser(User user) => db.UpdateUserInfo(user.login, user.login, user.password, user.email, user.task_ids);

        public bool DeleteUser(string login, string password)
        {
            if (db.GetUser(login, password) != null)
            {
                db.DeleteUser(login);
                currentUser = null;
                return true;
            }
            else
            {
                return false;
            }
        }
        // сравнение пользователей
        public bool EqUsers(User user2)
        {
            if (currentUser.login == user2.login && currentUser.email == user2.email && currentUser.task_ids == user2.task_ids)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
