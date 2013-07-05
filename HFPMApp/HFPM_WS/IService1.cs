using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HFPM_WS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);
        

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here

        [OperationContract]
        Person GetData(string id);


    }


    //[ServiceContract()]
    //public interface IEmployeeService
    //{
    //    [OperationContract]
    //    List<Employee> GetAllEmployeeDetails();

    //    [OperationContract]
    //    Employee GetEmployee(int Id);

    //    [OperationContract]
    //    void AddEmployee(Employee newEmp);

    //    [OperationContract]
    //    void UpdateEmployee(Employee newEmp);

    //    [OperationContract]
    //    void DeleteEmployee(string empId);
    //}




    //[ServiceContract()]
    //public interface IEmployeeService
    //{
    //    [WebGet(UriTemplate = "Employee")]
    //    [OperationContract]
    //    List<Employee> GetAllEmployeeDetails();

    //    [WebGet(UriTemplate = "Employee?id={id}")]
    //    [OperationContract]
    //    Employee GetEmployee(int Id);

    //    [WebInvoke(Method = "POST", UriTemplate = "EmployeePOST")]
    //    [OperationContract]
    //    void AddEmployee(Employee newEmp);

    //    [WebInvoke(Method = "PUT", UriTemplate = "EmployeePUT")]
    //    [OperationContract]
    //    void UpdateEmployee(Employee newEmp);

    //    [WebInvoke(Method = "DELETE", UriTemplate = "Employee/{empId}")]
    //    [OperationContract]
    //    void DeleteEmployee(string empId);
    //}


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }



    
    

}
