using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Microsoft.AspNetCore.Authorization;

namespace Shop.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryController([FromServices] DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> GetAll()
        {
            return Ok(await _context.Categories.AsNoTracking().OrderBy(c => c.Id).ToListAsync());
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _context.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Create([FromBody] Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Edit(int id, [FromBody] Category model)
        {
            if (model.Id != id)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry<Category>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover a categoria" });
            }
        }
    }
}