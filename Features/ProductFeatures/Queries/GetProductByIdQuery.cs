using CQRS_Mediator.WebApi.Context;
using CQRS_Mediator.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS_Mediator.WebApi.Features.ProductFeatures.Queries
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public int Id { get; set; }
        public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
        {
            private readonly IApplicationContext _context;
            private readonly IDistributedCache _distributedCache;

            public GetProductByIdQueryHandler(IApplicationContext context, IDistributedCache distributedCache)
            {
                _context = context;
                _distributedCache = distributedCache;
            }
            public async Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
            {
                var cacheKey = "product";
                string serializedProduct;
                var product = new Product();
                var redisProduct = await _distributedCache.GetAsync(cacheKey);
                if (redisProduct != null)
                {
                    serializedProduct = Encoding.UTF8.GetString(redisProduct);
                    product = JsonConvert.DeserializeObject<Product>(serializedProduct);
                }
                else
                {
                    product = await _context.Products.Where(a => a.Id == query.Id).AsNoTracking().FirstOrDefaultAsync();
                    serializedProduct = JsonConvert.SerializeObject(product);
                    redisProduct = Encoding.UTF8.GetBytes(serializedProduct);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                    await _distributedCache.SetAsync(cacheKey, redisProduct, options);
                }
                return product;
            }
        }
    }
}
