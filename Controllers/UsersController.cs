using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Musix4u_API.Models;
using Musix4u_API.Services;

namespace Musix4u_API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly AppDbContext _dbContext;

        public UsersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _dbContext.User.ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            var result = await _dbContext.User.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserRequest request)
        {
            var entity = await _dbContext.User.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (entity != null)
            {
                return Conflict();
            }

            entity = new User()
            {
                Name = request.Name,
                Email = request.Email,
                IsAdmin = request.IsAdmin
            };

            entity = _dbContext.User.Add(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(UpdateUserRequest request)
        {
            var entity = await _dbContext.User.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (entity != null)
            {
                return Conflict();
            }

            entity = await _dbContext.User.FindAsync(request.Id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = request.Name ?? entity.Name;
            entity.Email = request.Email ?? entity.Email;
            entity.IsAdmin = request.IsAdmin ?? entity.IsAdmin;

            entity = _dbContext.User.Update(entity).Entity;

            _dbContext.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var entity = await _dbContext.User.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            entity = _dbContext.User.Remove(entity).Entity;

            _dbContext.SaveChanges();
            
            return Ok(entity);
        }
    }
}