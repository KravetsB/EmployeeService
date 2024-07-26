using System.Collections.Generic;

namespace EmployeeService.App_Code.Models
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ManagerID { get; set; }
        public bool Enable { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
    }
}