using Microsoft.AspNetCore.Mvc;
using PlayerAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private static List<Player> _players = new List<Player>();

    [HttpPost("create")]
    public IActionResult Create([FromBody] Player player)
    {
        player.Id = Guid.NewGuid();
        _players.Add(player);

        return Ok(player);
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var player = _players.FirstOrDefault(p => p.Id == id);
        if (player == null)
            return NotFound();

        return Ok(player);
    }
}
