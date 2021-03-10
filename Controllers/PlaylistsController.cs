using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Musix4u_API.Models;
using Musix4u_API.Services;

namespace Musix4u_API.Controllers
{
    public class PlaylistsController : BaseApiController
    {
        private readonly AppDbContext _dbContext;

        public PlaylistsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _dbContext.Playlist.ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            var result = await _dbContext.Playlist.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreatePlaylistRequest request)
        {
            var entity = await _dbContext.Playlist.FirstOrDefaultAsync(x => x.Name == request.Name);

            if (entity != null)
            {
                return Conflict();
            }

            entity = new Playlist
            {
                Name = request.Name,
                IsPublic = request.IsPublic
            };

            entity = _dbContext.Playlist.Add(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(UpdatePlaylistRequest request)
        {
            var entity = await _dbContext.Playlist.FirstOrDefaultAsync(x => x.Name == request.Name);

            if (entity != null)
            {
                return Conflict();
            }

            entity = await _dbContext.Playlist.FindAsync(request.Id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = request.Name ?? entity.Name;
            entity.IsPublic = request.IsPublic ?? entity.IsPublic;

            entity = _dbContext.Playlist.Update(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _dbContext.Playlist.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity = _dbContext.Playlist.Remove(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }
    }
}