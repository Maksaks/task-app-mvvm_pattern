using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_App.Models
{
    enum TaskStatus
    {
        InWork = 0,
        Completed = 1
    }

    enum TaskComplexity
    {
        Easy = 0,
        Medium = 1,
        High = 2,
        Highest = 3
    }
    enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    public enum TypeSorts
    {
        NAME,
        TIME_BEFORE,
        PRIORITY,
        STATUS
    }
    public enum TypeFilters
    {
        TIME_BEFORE_EQ_DAY,  // дедлайн меньше дня
        TIME_BEFORE_EXPIRED, // дедлайн истек
        PRIORITY_MAX,        // максимальный приоритет
        COMPLEXITY_MAX,      // самые сложные
        SELF
    }
}
