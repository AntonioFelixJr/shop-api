using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Service;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController([FromServices] DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            return Ok(await _context.Users
                            .Select(u => new { u.Id, u.Username, u.Role })
                            .AsNoTracking()
                            .OrderBy(p => p.Id)
                            .ToListAsync());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Create([FromBody] User model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employee";

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                model.Password = string.Empty;

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar o usuário" });
            }
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Edit(int id, [FromBody] User model)
        {
            if (model.Id != id)
                return NotFound(new { message = "Usuário não encontrado" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry<User>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                model.Password = string.Empty;

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o usuário" });
            }
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(c => c.Id == id);

            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuário removido" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover o usuário" });
            }
        }
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model)
        {
            var user = await _context.Users
                            .AsNoTracking()
                            .Where(u => u.Username == model.Username && u.Password == model.Password)
                            .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            return new
            {
                user,
                token
            };
        }
    }
}