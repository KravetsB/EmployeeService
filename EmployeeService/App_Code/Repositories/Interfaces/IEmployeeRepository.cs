using EmployeeService.App_Code.Models;
using System.Threading.Tasks;

namespace EmployeeService.App_Code.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<bool> SetEmployeeEnableStatusAsync(int id, bool enable);
    }
}
