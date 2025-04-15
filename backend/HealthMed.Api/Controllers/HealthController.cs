using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HealthMed.Api.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly IDbConnection _connection;

        public HealthController(IDbConnection dbConnection)
        {
            _connection = dbConnection;
        }

        [HttpGet("db")]
        public IActionResult CheckDatabase()
        {
            try
            {
                _connection.Open();
                _connection.Close();
                return Ok(new { status = "ok", db = "connected" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", db = "disconnected" });
            }
        }
    }
}
