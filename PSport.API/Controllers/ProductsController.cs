using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSport.API.Models;

namespace PSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsController(ShopContext shopContext)
        {
            _shopContext = shopContext;

            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            return Ok(await _shopContext.Products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null) 
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction("GetProductById", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _shopContext.Entry(product).State = EntityState.Modified;

            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!_shopContext.Products.Any(p => p.Id == id)) 
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null) 
            {
                return NotFound(); 
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteProducts([FromQuery] int[] ids)
        {
            var products = new List<Product>();
            foreach (int id in ids)
            {
                var product = await _shopContext.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                products.Add(product);
            }

            _shopContext.Products.RemoveRange(products);
            await _shopContext.SaveChangesAsync();

            return Ok(products);
        }
    }
}