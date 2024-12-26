using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Evi_Mon_CP_V01.Models;

namespace Evi_Mon_CP_V01.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private readonly IWebHostEnvironment _env; 

        public ProductsController(ProductDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if(_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.ToListAsync();
        }

        // with child (to bring child to the parent) 
        //Get:api/Products/Sales/Include (to get all parent and child table data)
       
        [HttpGet("Sales/Include")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsWithSales()
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            return await _context.Products.Include(x=>x.Sales).ToListAsync();
        }
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound(); 
            }
                var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
        // with child (to bring child to the parent) 
        //Get:api/Products/Sales/Include (to get single parent and child table data)

        [HttpGet("Sales/Include/{id}")]
        public async Task<ActionResult<Product>> GetProductWithSales(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(x=>x.Sales).FirstOrDefaultAsync(x=>x.ProductId ==id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }
           //parent table
            var existing = await _context.Products.Include(x => x.Sales).FirstOrDefaultAsync(x => x.ProductId == id);
            if (existing == null) { return NotFound(); }
            existing.ProductName = product.ProductName;
            existing.ProductType = product.ProductType;
            existing.Price = product.Price;
            existing.MfgDate = product.MfgDate;
            existing.InStock = product.InStock;
            existing.Picture = product.Picture;

            //child table
            await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM sales WHERE productid={id}");
            foreach(var s in product.Sales)
            {
                _context.Sales.Add(new Sale { SellerName = s.SellerName, Quantity = s.Quantity, ProductId = id });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ProductDbContext.Products' is null."); 
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        //post:api/Products/Image/Upload (to upload images)

        [HttpPost("Image/Upload")]
        public async Task<ActionResult<string>> Upload(IFormFile pic)
        {
            string ext = Path.GetExtension(pic.FileName);
            string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
            string savePath = Path.Combine(_env.WebRootPath, "Pictures", f);
            FileStream fs = new FileStream(savePath, FileMode.Create);
            await pic.CopyToAsync(fs);
            fs.Close();
            return f;
        }
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound(); 
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
