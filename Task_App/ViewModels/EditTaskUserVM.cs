using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Task_App.Models;
using Task_App.Views;

namespace Task_App.ViewModels
{
    class EditTaskUserVM : BaseViewModel
    {
        public EditTaskUserWindow window;
        public TaskControllerSystem controllerSystem;
        public TaskInfo tmp { get; set; }
        public DelegateCommand EditTaskCommand { get; }
        public string PROGRESS
        {
            get => tmp.Progress.TrimEnd('%');
            set
            {
                tmp.Progress = value.Split('.').First() + "%";
            }
        }
        public EditTaskUserVM(TaskControllerSystem contr, TaskInfo task)
        {
            window = new EditTaskUserWindow();
            window.DataContext = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            controllerSystem = contr;
            tmp = task;
            EditTaskCommand = new DelegateCommand(EditTask);
            window.ShowDialog();
        }
        private void EditTask(object obj)
        {
            if (controllerSystem.EditTaskInfoUser(tmp.SubTasks, tmp.Progress))
            {
                window.Close();
            }
        }
    }
}
