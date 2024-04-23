using System;
using Microsoft.AspNetCore.Mvc;
using MongoDBConnect.Services;
using MongoDBConnect.Models;
using ZstdSharp.Unsafe;
using System.Diagnostics.Eventing.Reader;

namespace MongoDBConnect.Controllers;
[Controller]
[Route("api/[controller]")]
//NO DATA VALIDATION
//LOOK INTO AFTER MVP
public class PlaylistController: ControllerBase {

    private readonly ApplicationServices _applicationServices;

    public PlaylistController(ApplicationServices applicationServices){
        _applicationServices = applicationServices;
    }

    //read
    [HttpGet]
    public async Task<List<Playlist>> GetPasswords() => await _applicationServices.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Playlist>> GetPassword(string id) 
    {
        var playlist = await _applicationServices.GetAsync(id);
        if(playlist is null) 
        {
            return NotFound();
        }
        return playlist; 
    }

    //Gets a document by the username
    [HttpGet("user:length(7)")]
    public async Task<List<Playlist>> GetUserEntries(string user) => await _applicationServices.GetUserEntriesAsync(user);

    //create
    [HttpPost]
    public async Task<IActionResult> PostPasswords([FromBody] Playlist playlist) 
    {
        await _applicationServices.CreateAsync(playlist);
        return CreatedAtAction(nameof(GetPasswords), new { id = playlist._id}, playlist);
    }

    //update
    [HttpPut("{id}")] //look what id is in there thing
    public async Task<IActionResult> AddPasswords(string id, Playlist updatePlaylist)
    {
        var playlist = await _applicationServices.GetAsync(id);
        if(playlist is null)
        {
            return NotFound();
        }
        updatePlaylist._id = playlist._id;
        await _applicationServices.UpdateAsync(id, updatePlaylist);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var playlist = await _applicationServices.GetAsync(id);
        if(playlist is null)
        {
            return NotFound();
        }
        await _applicationServices.DeleteAsync(id);

        return NoContent();
    }
}
