using Dapper.net.Dto;
using Dapper.net.Entities;

namespace Dapper.net.contract
{
    public interface IcompanyRepository
    {
        public Task<IEnumerable<Company>> GetCompanies();
        public Task<Company> GetCompany(int id);
        public Task<Company> CreateCompany(CompanycreationDto company);
        public Task UpdateCompany(int id,CompanyupdatedDto Company);
        public Task DeleteCompany(int id);
        public Task<Company> GetCompanyByEmployeeId(int id);
        public Task<Company>GetMultipleResults(int id);
        public Task<List<Company>> MultipleMapping();
        public Task CreateMultipleCompanies(List<CompanycreationDto> companies);

    }
}
