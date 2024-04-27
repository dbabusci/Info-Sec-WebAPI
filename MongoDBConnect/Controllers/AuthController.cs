using System;
using Microsoft.AspNetCore.Mvc;
using MongoDBConnect.Services;
using MongoDBConnect.Models;
using ZstdSharp.Unsafe;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Cors;
using System.ComponentModel;

namespace MongoDBConnect.Controllers;
[Controller]
[Route("api/[controller]")]
[EnableCors("ReactApp")]

public class AuthController : ControllerBase 
{
    private readonly AuthServices _authServices;
    public AuthController(AuthServices auth)
    {
        _authServices = auth;
    }

    [HttpGet]
    public async Task<List<Auth>> GetUsers() => await _authServices.GetAsync();

    [HttpGet("{UserAuth}")]
    public async Task<List<Auth>> GetLogin(string user, string pass)
    {
        var ret = await _authServices.LoginAsync(user, pass);
        return ret;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] Auth newUser)
    {
        await _authServices.RegisterAsync(newUser);
        return CreatedAtAction(nameof(GetUsers), new { id = newUser._id}, newUser);
    }
}