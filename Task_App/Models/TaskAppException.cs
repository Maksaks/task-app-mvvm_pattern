using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_App.Models
{
    public abstract class TaskAppException : System.Exception
    {
        public string msg = "ERROR";
        public TaskAppException(string mes)
        {
            msg = mes;
        }
    }
    public class InCorrectDate : TaskAppException
    {
        public InCorrectDate() : base("Некоректно введені дані!")
        {

        }
    }
    public class NoConnectionWithDB : TaskAppException
    {
        public NoConnectionWithDB() : base("Відсутній зв'язок з базою даних!")
        {

        }
    }
    public class OutOfLimitTasks : TaskAppException
    {
        public OutOfLimitTasks() : base("Перевищено ліміт задач!")
        {

        }
    }
    public class InCorrectDateInput : TaskAppException
    {
        public InCorrectDateInput() : base("Некоректне введення дати!")
        {

        }
    }
    public class NoFillInputs : TaskAppException
    {
        public NoFillInputs() : base("Не заповнені обов'язкові поля!")
        {

        }
    }
    public class OutOfLimitUserOnTask : TaskAppException
    {
        public OutOfLimitUserOnTask() : base("Перевищено ліміт користувачів на одну задачу!")
        {

        }
    }
}
