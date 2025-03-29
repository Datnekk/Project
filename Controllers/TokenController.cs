using be.Dtos.Auth;
using be.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace be.Controllers;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(){
        
        HttpContext.Request.Cookies.TryGetValue("accessToken", out var accessToken);

        HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

        var tokenDTO = new TokenDTO{
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var tokenDtoToReturn = await _tokenService.RefreshJWTTokenAsync(tokenDTO);

        _tokenService.SetTokenCookie(tokenDtoToReturn, HttpContext);

        return Ok();
    }
}