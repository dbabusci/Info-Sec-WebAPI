using System;
using Microsoft.AspNetCore.Mvc;
using MongoDBConnect.Services;
using MongoDBConnect.Models;
using ZstdSharp.Unsafe;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Cors;

namespace MongoDBConnect.Controllers;
[Controller]
[Route("api/[controller]")]
[EnableCors("ReactApp")]
//NO DATA VALIDATION
//LOOK INTO AFTER MVP
public class PlaylistController: ControllerBase {

    private readonly ApplicationServices _applicationServices;

    public PlaylistController(ApplicationServices applicationServices)
    {
        _applicationServices = applicationServices;
    }

    //read
    [HttpGet]
    public async Task<List<Playlist>> GetPasswords() => await _applicationServices.GetAsync();

    //Gets a document by the username
    [HttpGet("{user}")]
    public async Task<List<Playlist>> GetUserEntries(string user) => await _applicationServices.GetUserEntriesAsync(user);

    //create
    [HttpPost]
    public async Task<IActionResult> PostPasswords([FromBody] Playlist playlist) 
    {
        await _applicationServices.CreateAsync(playlist);
        return CreatedAtAction(nameof(GetPasswords), new { id = playlist._id}, playlist);
    }
}
