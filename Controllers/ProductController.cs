using EcommerceApp.DTO;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace EcommerceApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly EcommerceDBContext _DBContext;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
            _DBContext = CommonController.CreateMainDbContext();
        }

        [HttpGet(Name = "GetAll")]
        public List<Product> GetAll()
        {
            try
            {
                List<Product> dataRecords = _DBContext.Products.Where(c => c.Active == true).ToList();
                return dataRecords;
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "Exception occurred while processing GetAll.");
                throw;
            }
        }

        [HttpGet(Name = "Get")]
        public IActionResult Get(string id)
        {
            try
            {
                Product? dataRecord = _DBContext.Products.Where(c => c.ProductId == id).FirstOrDefault();
                if (dataRecord != null)
                {
                    return Ok(new { Status = "Success" , Data = dataRecord });
                }
                else
                {
                    return NotFound("Product not found.");
                }
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "Exception occurred while processing Get.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete(Name = "Delete")]
        public IActionResult Delete(string id)
        {
            try
            {
                Product? dataRecord = _DBContext.Products.Where(c => c.ProductId == id && c.Active == true).FirstOrDefault();
                if (dataRecord != null)
                {
                    dataRecord.Active = false;
                    _DBContext.SaveChanges();
                    return Ok(new { Status = "Success" });
                }
                else
                {
                    return NotFound("Product not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing Delete.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost(Name = "Save")]
        public IActionResult Save([FromBody] ProductDTO product)
        {
            try
            {
                var dataRecord = _DBContext.Products.Where(c => c.ProductId == product.ProductId).FirstOrDefault();
                if (dataRecord == null)
                {
                    dataRecord = new Product()
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        CreatedOn = DateTime.Now,
                        Active = true
                    };
                    _DBContext.Products.Add(dataRecord);
                }

                dataRecord.Name = product.Name;
                dataRecord.Price =Convert.ToDecimal(product.Price);
                dataRecord.ImageUrl = product.ImageUrl;
                dataRecord.Description = product.Description;
                dataRecord.ModifedOn = DateTime.Now;

                _DBContext.SaveChanges();
                return Ok(new { Status = "Success" ,Data = dataRecord });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing Save.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile file, [FromQuery] string productId)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                var fileName = "";
                var filePath = "";

                if (file.Length > 0)
                {
                     fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                     filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(productId))
                    {
                        var dataRecord = _DBContext.Products.Where(c => c.ProductId == productId).FirstOrDefault();
                        if (dataRecord != null)
                        {
                            dataRecord.ImageUrl = filePath;
                            _DBContext.SaveChanges();
                        }
                    }

                }

                return Ok(new { Status = "Success", Data = filePath });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing Upload Photo.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
