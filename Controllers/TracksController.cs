using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<List<Track>>> Get()
        {
            var result = await _dbContext.Track.ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Track>>> Get(long id)
        {
            var result = await _dbContext.Track.FindAsync(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<List<Track>>> Create([FromForm] CreateTrackRequest request)
        {
            var file = TagLib.File.Create(new FormFileAbstraction(request.File));
            var performers = request.Performers != null && request.Performers.Any(x => !string.IsNullOrEmpty(x))
                ? request.Performers
                : file.Tag.Performers.ToList();
            var entity = new Track
            {
                Title = request.Title ?? file.Tag.Title,
                Performers = string.Join(", ", performers),
                Album = request.Album ?? file.Tag.Album,
                Year = request.Year ?? (file.Tag.Year == 0 ? null : file.Tag.Year),
                Duration = (long)file.Properties.Duration.TotalMilliseconds
            };

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
                    file.Tag.Pictures[0].Filename ?? $"{Path.GetFileNameWithoutExtension(request.File.FileName)}.{file.Tag.Pictures[0].MimeType.Split("/")[1]}",
                    file.Tag.Pictures[0].Data.Data,
                    false);
            }

            //var fileName = $"{entity.Title} - {entity.Performers}{Path.GetExtension(request.File.FileName)}";
            entity.Url = await _storageService.UploadFile(
                "tracks",
                request.File.FileName,
                request.File,
                false);


            entity = _dbContext.Track.Add(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(UpdateTrackRequest request)
        {
            var entity = await _dbContext.Track.FindAsync(request.Id);

            if (entity == null)
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

            var performers = request.Performers != null && request.Performers.Any(x => !string.IsNullOrEmpty(x))
                ? request.Performers
                : file?.Tag.Performers.ToList();
            entity.Performers = performers == null ? entity.Performers : string.Join(", ", performers);
            entity.Album = request.Album ?? file?.Tag.Album ?? entity.Album;
            entity.Year = request.Year ?? (file?.Tag.Year == 0 ? null : file?.Tag.Year) ?? entity.Year;
            entity.Duration = (long?)file?.Properties.Duration.TotalMilliseconds ?? entity.Duration;

            entity = _dbContext.Track.Update(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _dbContext.Track.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity = _dbContext.Track.Remove(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }
    }
}