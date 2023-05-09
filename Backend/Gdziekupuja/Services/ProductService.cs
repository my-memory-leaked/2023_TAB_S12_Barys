using AutoMapper;
using Gdziekupuja.Exceptions;
using Gdziekupuja.Models;
using Gdziekupuja.Models.DTOs.ProductDtos;

namespace Gdziekupuja.Services;

public interface IProductService
{
    int CreateProduct(CreateProductDto dto);
}

public class ProductService : IProductService
{
    private readonly GdziekupujaContext _dbContext;
    private readonly IMapper _mapper;

    public ProductService(GdziekupujaContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public int CreateProduct(CreateProductDto dto)
    {
        if (_dbContext.Products.Any(p => p.Name == dto.Name))
            throw new NotUniqueElementException("Produkt o podanej nazwie już istnieje");

        var product = _mapper.Map<Product>(dto);
        _dbContext.Products.Add(product);
        _dbContext.SaveChanges();

        return product.Id;
    }
}