using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
}
