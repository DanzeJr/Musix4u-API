using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Musix4u_API.Models;
using Musix4u_API.Services;

namespace Musix4u_API.Controllers
{
    public class TracksController : BaseApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly StorageService _storageService;

        public TracksController(AppDbContext dbContext, StorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult<List<Track>>> Get([FromQuery] FilterTrackRequest request)
        {
            var queryable = _dbContext.Track.AsQueryable();
            if (User.Identity?.IsAuthenticated == true)
            {
                queryable = queryable.Include(x => x.FavoriteTracks);
                if (request.Favorite == null && request.PlaylistId == null && string.IsNullOrEmpty(request.Title))
                {
                    queryable = queryable.Where(x => x.IsPublic || x.UploaderId == UserId);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.Title))
                    {
                        queryable = queryable.Where(x => EF.Functions.Like(x.Title, $"{request.Title}%"));
                    }

                    if (request.Favorite != null)
                    {
                        queryable = queryable.Where(x => x.FavoriteTracks.Any(f => f.UserId == UserId));
                    }

                    if (request.PlaylistId != null)
                    {
                        var playlist = await _dbContext.Playlist.FindAsync(request.PlaylistId);
                        if (playlist == null)
                        {
                            return NotFound("Playlist not found");
                        }

                        if (playlist.OwnerId != UserId)
                        {
                            if (!playlist.IsPublic)
                            {
                                return NotFound("Playlist not found");
                            }
                            queryable = queryable.Where(x => x.IsPublic);
                        }
                        queryable = queryable.Where(x => x.PlaylistTracks.Any(p => p.PlaylistId == request.PlaylistId));
                    }
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(request.Title))
                {
                    queryable = queryable.Where(x => EF.Functions.Like(x.Title, $"{request.Title}%"));
                }

                if (request.PlaylistId != null)
                {
                    var playlist = await _dbContext.Playlist.FindAsync(request.PlaylistId);
                    if (playlist == null || !playlist.IsPublic)
                    {
                        return NotFound("Playlist not found");
                    }
                    queryable = queryable.Where(x => x.PlaylistTracks.Any(p => p.PlaylistId == request.PlaylistId));
                }
                queryable = queryable.Where(x => x.IsPublic);
            }

            var result = await queryable.ToListAsync();

            if (User.Identity?.IsAuthenticated == true)
            {
                foreach (var track in result)
                {
                    track.IsFavorite = track.FavoriteTracks.Any(x => x.UserId == UserId);
                }
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Track>>> Get(long id)
        {
            var result = await _dbContext.Track.FindAsync(id);

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Track>> Create([FromForm] CreateTrackRequest request)
        {
            var file = TagLib.File.Create(new FormFileAbstraction(request.Song));

            var entity = new Track
            {
                Title = request.Title ?? file.Tag.Title,
                Performers = request.Performers ?? string.Join(", ", file.Tag.Performers),
                Album = request.Album ?? file.Tag.Album,
                Year = request.Year ?? (file.Tag.Year == 0 ? null : file.Tag.Year),
                Duration = (long)file.Properties.Duration.TotalMilliseconds,
                IsPublic = request.IsPublic,
                UploaderId = UserId
            };

            if (request.PlaylistId != null)
            {
                var playlist =
                    await _dbContext.Playlist.FirstOrDefaultAsync(
                        x => x.Id == request.PlaylistId && x.OwnerId == UserId);
                if (playlist == null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "No permission on this playlist");
                }

                entity.PlaylistTracks = new List<PlaylistTrack>
                {
                    new PlaylistTrack
                    {
                        PlaylistId = request.PlaylistId.Value,
                        TrackId = entity.Id
                    }
                };
            }

            if (request.IsFavorite)
            {
                entity.FavoriteTracks = new List<FavoriteTrack>
                {
                    new FavoriteTrack
                    {
                        TrackId = entity.Id,
                        UserId = UserId.Value
                    }
                };
                entity.IsFavorite = true;
            }

            if (request.Cover != null)
            {
                entity.CoverUrl = await _storageService.UploadFile(
                    "covers",
                    request.Cover.FileName,
                    request.Cover,
                    false);
            }
            else if (file.Tag.Pictures != null && file.Tag.Pictures.Any())
            {
                entity.CoverUrl = await _storageService.UploadFile(
                    "covers",
                    file.Tag.Pictures[0].Filename ?? $"{Path.GetFileNameWithoutExtension(request.Song.FileName)}.{file.Tag.Pictures[0].MimeType.Split("/")[1]}",
                    file.Tag.Pictures[0].Data.Data,
                    false);
            }

            //var fileName = $"{entity.Title} - {entity.Performers}{Path.GetExtension(request.File.FileName)}";
            entity.Url = await _storageService.UploadFile(
                "tracks",
                request.Song.FileName,
                request.Song,
                false);


            entity = _dbContext.Track.Add(entity).Entity;

            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Create), entity);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromForm] UpdateTrackRequest request)
        {
            var entity = await _dbContext.Track.Include(x => x.FavoriteTracks).FirstOrDefaultAsync(x => x.Id == request.Id);

            if (entity == null || entity.UploaderId != UserId)
            {
                return NotFound();
            }

            entity.Title = request.Title ?? entity.Title;

            TagLib.File file = null;
            if (request.File != null)
            {
                file = TagLib.File.Create(new FormFileAbstraction(request.File));
                entity.Url = await _storageService.UploadFile(
                    "tracks",
                    request.File.FileName,
                    request.File,
                    false);
            }

            if (request.Cover != null)
            {
                entity.CoverUrl = await _storageService.UploadFile(
                    "covers",
                    request.Cover.FileName,
                    request.Cover,
                    false);
            }
            else if (file?.Tag.Pictures != null && file.Tag.Pictures.Any())
            {
                entity.CoverUrl = await _storageService.UploadFile(
                    "covers",
                    file.Tag.Pictures[0].Filename ?? $"{Path.GetFileNameWithoutExtension(request.File.FileName)}.{file.Tag.Pictures[0].MimeType.Split("/")[1]}",
                    file.Tag.Pictures[0].Data.Data,
                    false);
            }

            var performers = request.Performers ?? (file != null ? string.Join(", ", file.Tag.Performers) : null);
            entity.Performers = performers ?? entity.Performers;
            entity.Album = request.Album ?? file?.Tag.Album ?? entity.Album;
            entity.Year = request.Year ?? (file?.Tag.Year == 0 ? null : file?.Tag.Year) ?? entity.Year;
            entity.Duration = (long?)file?.Properties.Duration.TotalMilliseconds ?? entity.Duration;
            entity.IsPublic = request.IsPublic ?? entity.IsPublic;

            entity = _dbContext.Track.Update(entity).Entity;

            _dbContext.SaveChanges();

            if (entity.FavoriteTracks.Any(x => x.UserId == UserId))
            {
                entity.IsFavorite = true;
            }

            return Ok(entity);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _dbContext.Track.FindAsync(id);

            if (entity == null || entity.UploaderId != UserId)
            {
                return NotFound();
            }

            entity = _dbContext.Track.Remove(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpPost("{id}/favorites")]
        [Authorize]
        public async Task<ActionResult<Track>> Favorite(long id)
        {
            var favTrack =
                await _dbContext.FavoriteTrack.FirstOrDefaultAsync(x => x.TrackId == id && x.UserId == UserId);
            if (favTrack != null)
            {
                return Conflict("Already like this track");
            }

            var track = await _dbContext.Track.FindAsync(id);

            if (track == null || (!track.IsPublic && track.UploaderId != UserId))
            {
                return NotFound("Track not found");
            }

            favTrack = new FavoriteTrack()
            {
                TrackId = id,
                UserId = UserId.Value
            };

            _dbContext.FavoriteTrack.Add(favTrack);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Favorite), favTrack);
        }

        [HttpDelete("{id}/favorites")]
        [Authorize]
        public async Task<ActionResult<Track>> UnFavorite(long id)
        {
            var favTrack = await _dbContext.FavoriteTrack.FirstOrDefaultAsync(x => x.TrackId == id && x.UserId == UserId);

            if (favTrack == null)
            {
                return NotFound("Track not found in favorite list");
            }

            _dbContext.FavoriteTrack.Remove(favTrack);

            await _dbContext.SaveChangesAsync();

            return Ok(favTrack);
        }

    }
}