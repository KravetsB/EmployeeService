using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmployeeService.App_Code.Models;
using System;

namespace EmployeeService.App_Code.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private const string GetHierarchicalEmployeesSqlCommandText =
            @"WITH HierarchicalEmployees (ID, Name, ManagerID, Enable) AS (
                SELECT ID, Name, ManagerID, Enable
                FROM Employee
                WHERE ID = @ID
                
                UNION ALL
                
                SELECT emp.ID, emp.Name, emp.ManagerID, emp.Enable
                FROM Employee emp
                INNER JOIN HierarchicalEmployees he ON emp.ManagerID = he.ID
                WHERE emp.ManagerID != emp.ID
            )
            SELECT ID, Name, ManagerID, Enable FROM HierarchicalEmployees;";

        private const string SetEnableFieldSqlCommandTest = "UPDATE Employee SET Enable = @Enable WHERE ID = @ID";

        private readonly string connectionString;

        public EmployeeRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand(GetHierarchicalEmployeesSqlCommandText, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@ID", id);

                    sqlConnection.OpenAsync().Wait(); //wait to handle potential error. If use await we will not see exceptions

                    List<Employee> employees = new List<Employee>();

                    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Employee employee = await ReadEmployeeAsync(reader);
                            employees.Add(employee);
                        }
                    }

                    foreach (Employee employee in employees)
                    {
                        employee.Employees = employees.Where(e => e.ManagerID == employee.ID && e.ID != e.ManagerID);
                    }

                    return employees.FirstOrDefault(e => e.ID == id);
                }
            }
            catch (Exception ex)
            {
                //log errors
                return null;
            }
        }

        public async Task<bool> SetEmployeeEnableStatusAsync(int id, bool enable)
        {
            try
            {
                const int rowsToAffect = 1;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand(SetEnableFieldSqlCommandTest, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@Enable", enable);
                    sqlCommand.Parameters.AddWithValue("@ID", id);

                    sqlConnection.OpenAsync().Wait(); //wait to handle potential error. If use await we will not see exceptions
                    int rowsAffected = await sqlCommand.ExecuteNonQueryAsync();

                    return rowsAffected == rowsToAffect;
                }
            }
            catch (Exception ex)
            {
                //log error
                return false;
            }
        }

        private async Task<Employee> ReadEmployeeAsync(SqlDataReader reader)
        {
            Employee employee = new Employee()
            {
                ID = await reader.GetFieldValueAsync<int>(reader.GetOrdinal("ID")),
                Name = await reader.GetFieldValueAsync<string>(reader.GetOrdinal("Name")),
                Enable = await reader.GetFieldValueAsync<bool>(reader.GetOrdinal("Enable"))
            };

            int managerIdOrdinal = reader.GetOrdinal("ManagerID");

            if(!await reader.IsDBNullAsync(managerIdOrdinal)) //simpler syntax is not supported in current C# version
            {
                employee.ManagerID = await reader.GetFieldValueAsync<int>(managerIdOrdinal);
            }

            return employee;
        }
    }
}