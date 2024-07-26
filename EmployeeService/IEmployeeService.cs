using EmployeeService.App_Code.Models;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;

namespace EmployeeService
{
    [ServiceContract]
    public interface IEmployeeService
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetEmployeeById?id={id}",
            ResponseFormat = WebMessageFormat.Json,  BodyStyle = WebMessageBodyStyle.Bare)]
        Task<Employee> GetEmployeeByIdAsync(int id);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "EnableEmployee?id={id}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        Task<bool> EnableEmployeeAsync(int id, bool enable);
    }
}