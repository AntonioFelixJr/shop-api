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
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController([FromServices] DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            return Ok(await _context.Products
                            .Include(p => p.Category)
                            .AsNoTracking()
                            .OrderBy(p => p.Id)
                            .ToListAsync());
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products
                            .Include(p => p.Category)
                            .AsNoTracking()
                            .SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            return Ok(product);
        }

        [HttpGet]
        [Route("categories/{id:int:min(1)}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetByCategory(int id)
        {
            var products = await _context.Products
                            .Include(p => p.Category)
                            .AsNoTracking()
                            .Where(c => c.Category.Id == id)
                            .ToListAsync();

            if (products == null)
                return NotFound(new { message = "Não existem produtos com essa categoria" });

            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Create([FromBody] Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar ao produto" });
            }
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Edit(int id, [FromBody] Product model)
        {
            if (model.Id != id)
            {
                return NotFound(new { message = "Produto não encontradao" });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry<Product>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar o produto" });
            }
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(c => c.Id == id);

            if (product == null)
                return NotFound(new { message = "Produto não encontrado" });

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Produto removido" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover o produto" });
            }
        }
    }
}