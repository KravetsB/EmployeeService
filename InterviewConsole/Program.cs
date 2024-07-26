namespace InterviewConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            EmployeeService.EmployeeService employeeService = new EmployeeService.EmployeeService();

            _ = employeeService.GetEmployeeByIdAsync(1).Result;
            _ = employeeService.EnableEmployeeAsync(1, false).Result;
        }
    }
}
