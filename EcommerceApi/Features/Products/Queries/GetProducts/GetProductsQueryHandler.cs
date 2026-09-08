using EcommerceApi.Dtos;
using EcommerceApi.Features.Base;
using EcommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler(AppDbContext _context) : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
    {
        private static readonly Dictionary<string, Func<IQueryable<Product>, bool,
            IOrderedQueryable<Product>>> sortMap = new(StringComparer.OrdinalIgnoreCase)
            {
                ["price"] = (q, desc) => desc ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
                ["name"] = (q, desc) => desc ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
                ["createdat"] = (q, desc) => desc ? q.OrderByDescending(p => p.CreatedAt) : q.OrderBy(p => p.CreatedAt)
            };
        public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                //query = query.Where(p => EF.Functions.FreeText(p.Name, searchTerm) ||
                //         EF.Functions.FreeText(p.Description, searchTerm));
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                             (p.Description != null && p.Description.Contains(searchTerm)));
            }
            var sortKey = request.SortBy ?? "id";
            if (sortMap.TryGetValue(sortKey, out var sortExpression))
                query = sortExpression(query, request.IsDescending);
            else
            {
                query = request.IsDescending ? query.OrderByDescending(p => p.Id)
                    : query.OrderBy(p => p.Id);
            }
            var itemsPlusOne = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize + 1)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Sku = p.Sku,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(cancellationToken);
            var hasNextPage = itemsPlusOne.Count > request.PageSize;
            var items = itemsPlusOne.Take(request.PageSize).ToList();
            return new PagedResult<ProductDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                HasNextPage = hasNextPage
            };
        }
    }
}
