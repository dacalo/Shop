namespace Shop.Web.Controllers.API
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;

    [Route("api/[Controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            this.productRepository = productRepository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                return this.Ok(this.productRepository.GetAllWithUsers());
            }
            catch (Exception ex)
            {
                var message = $"Error: {ex}";
                this.logger.LogError(message);
                return this.BadRequest(message);
            }
        }
    }

}