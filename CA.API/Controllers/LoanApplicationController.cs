using CA.Application.Interfaces;
using CA.Application.Interfaces.DTOs;
using CA.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CA.API.Controllers
{
    [ApiController]
    [Route("api/loan-application")]
    public class LoanApplicationController : ControllerBase
    {
        private readonly ILoanApplicationService _createService;

        public LoanApplicationController(
            ILoanApplicationService createService)
        {
            _createService = createService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLoanApplicationRequest request)
        {
            var response = await _createService.CreateAsync(request);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(new
            {
                LoanApplicationId = id
            });
        }
    }
}
