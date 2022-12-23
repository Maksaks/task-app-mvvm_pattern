using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Task_App;
using Task_App.Models;
using System.Data.Entity;

namespace BankTests
{
    /*[TestClass]
    public class BankAccountTests
    {
        DateBase db = new Task_App.Models.DateBase();

        [TestMethod]
        public void AddUserTest()
        {
            string login = "Maksaksss", password = "12345678", email = "maksym.zhmutskyigmail.com", task_ids = "1,2";
            Assert.AreEqual(true, db.AddUser(login, password, email, task_ids));
        }

        [TestMethod]
        public void CheckUserLoginTest()
        {
            string login = "Maksaksss";
            Assert.AreEqual(true, db.CheckUserLogin(login));
        }

        [TestMethod]
        public void GetUserTest()
        {
            string login = "Maksakss", password = "12345678";
            Assert.AreEqual(login, db.GetUser(login, password).login);
        }

        [TestMethod]
        public void UpdateUserInfoTest()
        {
            string last_login = "Maksakss", login = "Maksaks3", password = "12345678", email = "maksym.zhmutskyigmail.com", task_ids = "1,2";
            Assert.AreEqual(true, db.UpdateUserInfo(last_login, login, password, email, task_ids));
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            string login = "Maksaks3";
            Assert.AreEqual(true, db.DeleteUser(login));
        }

        [TestMethod]
        public void AddTaskTest()
        {
            string Name = "TEST4", SubTasks = "TEST", TimeFrom = "TEST", TimeBefore = "TEST", Info = "TEST", Progress = "TEST", Priority = "TEST", Complexity = "TEST", Resources = "TEST", Status = "TEST", admin_id = "TEST", users_id = "TEST";
            Assert.AreEqual(true, db.AddTask(Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, admin_id, users_id));
        }

        [TestMethod]
        public void DeleteTaskTest()
        {
            int ID = 1;
            Assert.AreEqual(true, db.DeleteTask(ID));
        }

        [TestMethod]
        public void UpdateTaskTest()
        {
            int ID = 2;
            string Name = "NEW2", SubTasks = "TEST", TimeFrom = "TEST", TimeBefore = "TEST", Info = "TEST", Progress = "TEST", Priority = "TEST", Complexity = "TEST", Resources = "TEST", Status = "TEST", admin_id = "TEST", users_id = "TEST";
            Assert.AreEqual(true, db.UpdateTask(ID, Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, admin_id, users_id));
        }
    }

    [TestClass]
    public class AuthInAppTest
    {
        public AuthInApp authInApp = new AuthInApp();
        [TestMethod]
        public void SignUpTest()
        {
            string login = "TESTSIGNUP", password = "12345678", email = "EMAIL", task_ids = "";
            Assert.AreEqual("Вдала реєстрація!", authInApp.SignUp(login, password, email, task_ids));
        }
        [TestMethod]
        public void LogInTest()
        {
            string login = "TESTSIGNUP", password = "1234568"; // некоректний пароль
            Assert.AreEqual("Невірний пароль!", authInApp.LogIn(login, password));
        }
    }
    [TestClass]
    public class UserManagerTest
    {
        public UserManager userManager = new UserManager(new User(4, "Maksaksss", "12345678", "maksym.zhmutskyigmail.com", "1,2"));
        [TestMethod]
        public void UpdateUserInfoTest()
        {
            string login = "TESTMANAGER", password = "12345678", email = "EMAIL", task_ids = "";
            Assert.AreEqual("Інформацію оновлено!", userManager.UpdateUserInfo(login, password, email, task_ids).Item1);
        }
        [TestMethod]
        public void DeleteUserTest()
        {
            string login = "TESTSIGNUP", password = "12345678";
            Assert.AreEqual(true, userManager.DeleteUser(login, password));
        }
    }*/
    /*[TestClass]
    public class UserTaskManagerTest
    {
        public UserTaskManager userManager = new UserTaskManager(new User(7, "TESTSIGNUP", "12345678", "EMAIl", "2"));
        [TestMethod]
        public void SelectTaskTest()
        {
            int ID = 2;
            Assert.AreEqual(true, userManager.SelectTask(ID));
        }
        [TestMethod]
        public void CreateTaskTest()
        {
            string Name = "TESTCREATE", SubTasks = "TEST", TimeFrom = "TEST", TimeBefore = "TEST", Info = "TEST", Progress = "TEST", Priority = "TEST", Complexity = "TEST", Resources = "TEST", Status = "TEST", users_id = "TEST";
            Assert.AreEqual(true, userManager.CreateTask(Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, users_id));
        }
        [TestMethod]
        public void EditTaskInfoOwnerTest()
        {
            userManager.SelectTask(5);
            string Name = "TESTEDITT", SubTasks = "TEST", TimeFrom = "TEST", TimeBefore = "TEST", Info = "TEST", Progress = "TEST", Priority = "TEST", Complexity = "TEST", Resources = "TEST", Status = "TEST", users_id = "TEST";
            Assert.AreEqual(true, userManager.EditTaskInfoOwner(userManager.GetSelectTask().Name,Name, SubTasks, TimeFrom, TimeBefore, Info, Progress, Priority, Complexity, Resources, Status, users_id));
        }
        [TestMethod]
        public void EditTaskInfoUserTest()
        {
            userManager.SelectTask(2);
            string SubTasks = "TESTEDIT", Progress = "421";
            Assert.AreEqual(true, userManager.EditTaskInfoUser(SubTasks, Progress));
        }
    }*/
    
    /*[TestClass]
    public class FilterTest
    {
        public static AuthInApp auth = new AuthInApp();
        public static User user = auth.LogIn("Maksik", "12345678") as User;
        public UserTaskManager userManager = new UserTaskManager(user);
        [TestMethod]
        public void Filter_TIME_BEFORE_EQ_DAY()
        {
            userManager.FilterListTasks(TypeFilters.TIME_BEFORE_EQ_DAY);
            Assert.AreEqual("TEST", userManager.GetFilterListTasks()[0].Name);
        }
        [TestMethod]
        public void Filter_TIME_BEFORE_EXPIRED()
        {
            userManager.FilterListTasks(TypeFilters.TIME_BEFORE_EXPIRED);
            Assert.AreEqual("MAINTASK", userManager.GetFilterListTasks()[0].Name);
        }
        [TestMethod]
        public void Filter_PRIORITY_MAX()
        {
            userManager.FilterListTasks(TypeFilters.PRIORITY_MAX);
            Assert.AreEqual("EXCEPTIONTEST", userManager.GetFilterListTasks()[0].Name);
        }
        [TestMethod]
        public void Filter_COMPLEXITY_MAX()
        {
            userManager.FilterListTasks(TypeFilters.COMPLEXITY_MAX);
            Assert.AreEqual("MAINTASK", userManager.GetFilterListTasks()[0].Name);
        }
        [TestMethod]
        public void Filter_SELF()
        {
            userManager.FilterListTasks(TypeFilters.SELF);
            Assert.AreEqual("TEST", userManager.GetFilterListTasks()[0].Name);
        }
    }*/

}
