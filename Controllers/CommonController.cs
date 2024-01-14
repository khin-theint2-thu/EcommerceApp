using EcommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Controllers
{
    public class CommonController : Controller
    {
        public CommonController() { }

        public static EcommerceDBContext CreateMainDbContext()
        {
            DbContextOptionsBuilder<EcommerceDBContext> _optionsBuilder = new DbContextOptionsBuilder<EcommerceDBContext>();
            _optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DefaultConnection"]);

            EcommerceDBContext _DbContext = new EcommerceDBContext(_optionsBuilder.Options);
            return _DbContext;
        }
    }
}
