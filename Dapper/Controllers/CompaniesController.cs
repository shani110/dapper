using Dapper.net.contract;
using Dapper.net.Dto;
using Dapper.net.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dapper.net.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IcompanyRepository _companyRepo;

        public CompaniesController(IcompanyRepository companyRepo)
        {
            _companyRepo = companyRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepo.GetCompanies();
            return Ok(companies);
        }

        [HttpGet("{id}",Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company= await _companyRepo.GetCompany(id);
            if (company == null)
                return NotFound();
            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanycreationDto Company)
        {
            
                var createdCompany = await _companyRepo.CreateCompany(Company);
                return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }
        [HttpPut("{id}")]  
        public async Task<IActionResult>UpdateCompany(int id, [FromBody]CompanyupdatedDto Company)
        {
            var dbcompany = await _companyRepo.GetCompany(id);
            if (dbcompany is null)
                return NotFound();
            await _companyRepo.UpdateCompany(id, Company);
            return NoContent();
        }
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteCompany(int id)
        {
            var dbcompany = await _companyRepo.GetCompany(id);
            if (dbcompany is null)
                return NotFound();
            await _companyRepo.DeleteCompany(id);
            return NoContent();
        }
        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            var company = await _companyRepo.GetCompanyByEmployeeId(id);
            if (company is null)
                return NotFound();
            return Ok(company);
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var company = await _companyRepo.GetMultipleResults(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }
        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GeMultipleMapping()
        {
            var company = await _companyRepo.MultipleMapping();

            return Ok(company);

        }
    }
}
