
using Dapper.net.contract;
using Dapper.net.Dto;
using Dapper.net.Entities;
using DapperASPNetCore.Context;
using System.Data;

namespace Dapper.net.Repository
{
    public class CompanyRepository : IcompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateCompany(CompanycreationDto company)
        {
            var query = "INSERT INTO Companies(Name,Address,Country) VALUES (@Name,@Address,@Country)" + "SELECT CAST (SCOPE_IDENTITY() AS int)";
            var parameters = new DynamicParameters();
            parameters.Add("Name", company.Address, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);

                var createCompany = new Company
                {
                    Id = id,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country,
                };
                return createCompany;
            }
        }

        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies WHERE Id=@Id";

            using(var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query,new {id});
            }
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "SELECT Id, Name AS CompanyName, Address, Country FROM Companies";

            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }

        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
                return company;
            }
        }

        public async Task<Company> GetCompanyByEmployeeId(int id)
        {
            var procedureName = "ShowCompanyByEmployeeId";
            var parameter = new DynamicParameters();
            parameter.Add("id", id, DbType.Int32, ParameterDirection.Input);

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>(procedureName, parameter,commandType:CommandType.StoredProcedure);
                return company;

            }
        }

        public  async Task<Company> GetMultipleResults(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id =@Id;" + "SELECT * FROM Employees WHERE CompanyId=@Id";
            using (var connection= _context.CreateConnection())
            using (var mutli = await connection.QueryMultipleAsync(query,new { id }))
            {
                var company=await mutli.ReadSingleOrDefaultAsync<Company>();
                if (company is not null)
                    company.Employees=(await mutli.ReadAsync<Employee>()).ToList();
                return company;

            }


            
            
        }

        public async Task<List<Company>> MultipleMapping()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON c.Id = e.CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var companyDict = new Dictionary<int, Company>();

                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(currentCompany.Id, currentCompany);
                        }

                        currentCompany.Employees.Add(employee);
                        return currentCompany;
                    }
                );

                return companies.Distinct().ToList();
            }
        }

        public async Task UpdateCompany(int id, CompanyupdatedDto Company)
        {
            var query = "UPDATE Companies SET Name = @Name,Address=@Address,Country=@Country WHERE Id=@Id";

            var parameters = new DynamicParameters();
            parameters.Add("id", id, DbType.Int32);
            parameters.Add("Name", Company.Name, DbType.String);
            parameters.Add("Address", Company.Address, DbType.String);
            parameters.Add("Country", Company.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task CreateMultipleCompanies(List<CompanycreationDto> companies)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var company in companies)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", company.Name, DbType.String);
                        parameters.Add("Address", company.Address, DbType.String);
                        parameters.Add("Country", company.Country, DbType.String);

                        await connection.ExecuteAsync(query, parameters, transaction: transaction);
                    }

                    transaction.Commit();
                }
            }
        }
    }   }   