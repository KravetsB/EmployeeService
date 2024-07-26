using EmployeeService.App_Code.Models;
using EmployeeService.App_Code.Repositories;
using System.Threading.Tasks;

namespace EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            string connectionString = "Server=DESKTOP-0U79TKP\\SQLEXPRESS;Database=EmployeesDB;Trusted_Connection=True;"; //should be in config but it not loaded properly using ConfigurationManager.ConnectionStrings[...]
            _employeeRepository = new EmployeeRepository(connectionString); // should be created with DI but again this aged framework and WCF not support it directly and as well as simple web api
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(id);
        }

        public async Task<bool> EnableEmployeeAsync(int id, bool enable)
        {
            return await _employeeRepository.SetEmployeeEnableStatusAsync(id, enable);
        }
    }
}