using EcommerceApi.Dtos;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler :IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GetProductsQueryHandler> _logger;
        private static readonly Dictionary<string, Func<IQueryable<Product>,bool, 
            IOrderedQueryable<Product>>> sortMap=new(StringComparer.OrdinalIgnoreCase)
        {
            ["price"] = (q, desc) => desc ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
            ["name"] = (q, desc) => desc ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
            ["createdat"] = (q, desc) => desc ? q.OrderByDescending(p => p.CreatedAt) : q.OrderBy(p => p.CreatedAt)
        };
        public GetProductsQueryHandler(AppDbContext context, ILogger<GetProductsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            
            var query = _context.Products.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }
           
            var sortKey = request.SortBy ?? "id";
            if (sortMap.TryGetValue(sortKey, out var sortExpression))
                query = sortExpression(query, request.IsDescending);
            else
            {
                query = request.IsDescending ? query.OrderByDescending(p => p.Id)
                    : query.OrderBy(p => p.Id);
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize).Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Sku = p.Sku,
                    CreatedAt = p.CreatedAt
                }).ToListAsync(cancellationToken);
            return new PagedResult<ProductDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
