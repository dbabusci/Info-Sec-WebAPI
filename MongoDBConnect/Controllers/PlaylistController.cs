using System;
using Microsoft.AspNetCore.Mvc;
using MongoDBConnect.Services;
using MongoDBConnect.Models;
using ZstdSharp.Unsafe;

namespace MongoDBConnect.Controllers;
[Controller]
[Route("api/[controller]")]
//NO DATA VALIDATION
//LOOK INTO AFTER MVP
public class PlaylistController: Controller {

    private readonly ApplicationServices _applicationServices;

    public PlaylistController(ApplicationServices applicationServices){
        _applicationServices = applicationServices;
    }

    //read
    [HttpGet]
    public async Task<List<Playlist>> GetPasswords() 
    {
        return await _applicationServices.GetAsync();
    }

    //create
    [HttpPost]
    public async Task<IActionResult> PostPasswords([FromBody] Playlist playlist) 
    {
        await _applicationServices.CreateAsync(playlist);
        return CreatedAtAction(nameof(GetPasswords), new { id = playlist.ID}, playlist);
    }

    //update
    [HttpPut("{id}")] //look what id is in there thing
    public async Task<IActionResult> AddPasswords(string id, [FromBody] string websiteName, [FromBody] string websiteUsername, [FromBody] string websitePassword)
    {
        await _applicationServices.AddToPlaylistAsync(id, websiteName, websiteUsername, websitePassword);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _applicationServices.DeleteAsync(id);
        return NoContent();
    }
}
