using Microsoft.AspNetCore.Mvc;

namespace CA.API.Controllers
{
    [ApiController]
    [Route("api/loan-application")]
    public class LoanApplicationController : ControllerBase
    {
        public LoanApplicationController()
        {

        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _getLoanApplicationHandler.Handle(
                new GetLoanApplicationByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
