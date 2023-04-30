using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancioBackend.Services;

namespace FinancioBackend.Controllers;

[ApiController]
[Route("user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;

    }

    [HttpGet("me")]
    public ActionResult GetUser()
    {
        var email = _userService.GetEmail();

        return Ok(new { email });
    }
}
