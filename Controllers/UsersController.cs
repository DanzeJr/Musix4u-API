using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
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
        [Authorize]
        [AllowAnonymous]
        public async Task<ActionResult> Create(CreateUserRequest request)
        {
            if (request.IsAdmin && !User.IsInRole(Role.Admin))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Permission denied.");
            }
            UserRecord user = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var tokenResult = await FirebaseAuth.DefaultInstance
                    .VerifyIdTokenAsync(Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "", StringComparison.InvariantCultureIgnoreCase));
                user = await FirebaseAuth.DefaultInstance.GetUserAsync(tokenResult.Uid);
                request.Email = user.Email;
                request.IsAdmin = false;
                request.Name = user.DisplayName;
            }
            var entity = await _dbContext.User.FirstOrDefaultAsync(x => x.Email == request.Email);

            if (entity != null)
            {
                return Conflict("An account with this email already exist.");
            }

            entity = new User()
            {
                Name = request.Name,
                Email = request.Email,
                IsAdmin = request.IsAdmin
            };

            entity = _dbContext.User.Add(entity).Entity;

            _dbContext.SaveChanges();

            if (User.Identity?.IsAuthenticated == false || User.IsInRole(Role.Admin))
            {
                var userArgs = new UserRecordArgs
                {
                    DisplayName = entity.Name,
                    Email = entity.Email,
                    EmailVerified = true,
                    Password = request.Password,
                    Disabled = false
                };

                user = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);

            }
            else
            {
                var userArgs = new UserRecordArgs
                {
                    Uid = user.Uid,
                    Email = user.Email,
                    EmailVerified = true,
                    PhotoUrl = user.PhotoUrl,
                    Disabled = false
                };

                user = await FirebaseAuth.DefaultInstance.UpdateUserAsync(userArgs);
            }
                
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Uid, new Dictionary<string, object>
            {
                {"userId", entity.Id},
                {ClaimTypes.Role, entity.IsAdmin ? Role.Admin : Role.User}
            });

            return CreatedAtAction(nameof(Create), entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateUserRequest request)
        {
            var entity = await _dbContext.User.FindAsync(id);

            if (entity == null || entity.Id != UserId)
            {
                return NotFound("User not found");
            }

            var firebaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(entity.Email);

            entity.Name = request.Name ?? entity.Name;

            entity = _dbContext.User.Update(entity).Entity;

            _dbContext.SaveChanges();

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
            {
                Uid = firebaseUser.Uid,
                DisplayName = entity.Name
            });

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        [Authorize]
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