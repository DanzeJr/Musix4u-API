using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult> Get([FromQuery] FilterPlaylistRequest request)
        {
            var queryable = _dbContext.Playlist.AsQueryable();
            if (User.Identity?.IsAuthenticated == true)
            {
                if (request.Name == null)
                {
                    if (request.Owned == true)
                    {
                        queryable = queryable.Where(x => x.OwnerId == UserId);
                    }
                    else if (request.Owned == false)
                    {
                        queryable = queryable.Where(x => x.OwnerId != UserId);
                    }
                    queryable = queryable.Where(x => x.IsPublic);
                }
                else
                {
                    queryable = queryable.Where(x => EF.Functions.Like(x.Name, $"{request.Name}%"));
                }

            }
            else
            {
                queryable = queryable.Where(x => x.IsPublic);
            }

            var result = await queryable.ToListAsync();

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
        [Authorize]
        public async Task<ActionResult> Create([FromBody] CreatePlaylistRequest request)
        {
            var entity = new Playlist
            {
                Name = request.Name,
                IsPublic = request.IsPublic,
                OwnerId = UserId.Value
            };

            entity = _dbContext.Playlist.Add(entity).Entity;

            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Create), entity);
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


        [HttpPost("{id}/tracks")]
        [Authorize]
        public async Task<ActionResult<PlaylistTrack>> AddSong(long id, [FromBody] AddTrackToPlaylistRequest request)
        {
            var playlistTrack = await
                _dbContext.PlaylistTrack.FirstOrDefaultAsync(x => x.PlaylistId == id && x.TrackId == request.TrackId);

            if (playlistTrack != null)
            {
                return Conflict("Track already in this playlist");
            }

            var playlist = await _dbContext.Playlist.FindAsync(id);

            if (playlist == null || playlist.OwnerId != UserId)
            {
                return NotFound("Playlist not found");
            }

            var track = await _dbContext.Track.FindAsync(request.TrackId);

            if (track == null || (!track.IsPublic && track.UploaderId != UserId))
            {
                return NotFound("Track not found");
            }

            playlistTrack = new PlaylistTrack
            {
                PlaylistId = id,
                TrackId = request.TrackId
            };

            _dbContext.PlaylistTrack.Add(playlistTrack);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(AddSong), playlistTrack);
        }

        [HttpDelete("{id}/tracks/{trackId}")]
        [Authorize]
        public async Task<ActionResult<PlaylistTrack>> RemoveSong(long id, long trackId)
        {
            var playlist = await _dbContext.Playlist.FindAsync(id);

            if (playlist == null || playlist.OwnerId != UserId)
            {
                return NotFound("Playlist not found");
            }

            var playlistTrack = await _dbContext.PlaylistTrack.FirstOrDefaultAsync(x => x.PlaylistId == id && x.TrackId == trackId);

            if (playlistTrack == null)
            {
                return NotFound("Track not found in playlist");
            }

            _dbContext.PlaylistTrack.Remove(playlistTrack);

            await _dbContext.SaveChangesAsync();

            return Ok(playlistTrack);
        }
    }
}