using Models;
using Models.DTO;
using Models.Employees.Interfaces;

namespace Services.Employees;

public interface IEmployeeService 
{
    public ResponsePageDto<IEmployee> ReadEmployees(int pageNumber, int pageSize);
    public IEmployee ReadEmployee(Guid id, bool cc_decrypted);
}