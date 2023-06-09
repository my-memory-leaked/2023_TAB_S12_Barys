using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gdziekupuja.Common;
using Gdziekupuja.Exceptions;
using Gdziekupuja.Models;
using Gdziekupuja.Models.DTOs.CommentDtos;
using Gdziekupuja.Models.DTOs.OfferDtos;
using Microsoft.EntityFrameworkCore;

namespace Gdziekupuja.Services;

public interface IOfferService
{
    int Create(CreateOfferDto dto, int productInstanceId);
    OffersWithTotalCount SearchOffers(int? countyId, string? productName, int? categoryId, int? pageSize, int? pageNumber, int? userId);
    void AddOfferToFavourites(int offerId, int userId);
    IEnumerable<CommentDto>? GetAllComments(int offerId, int? userId);
    OffersWithTotalCount GetFavouritesOffers(int userId, int pageSize, int pageNumber);
    int UpdateOffer(int id, decimal? price);
    void Ban(int adminId, int offerId);
    void Unban(int adminId, int offerId);
}

public class OfferService : IOfferService
{
    private readonly IMapper _mapper;
    private readonly GdziekupujaContext _dbContext;

    public OfferService(IMapper mapper, GdziekupujaContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public int Create(CreateOfferDto dto, int productInstanceId)
    {
        var salesPoint = _dbContext.SalesPoints.FirstOrDefault(x => x.Id == dto.SalesPointId) ?? throw new NotFoundException("");
        var user = _dbContext.Users.FirstOrDefault(x => x.Id == dto.UserId) ?? throw new NotFoundException("");
        var productInstance = _dbContext.ProductInstances.FirstOrDefault(x => x.Id == productInstanceId) ?? throw new NotFoundException("");

        var offer = new Offer
        {
            Price = dto.Price,
            SalesPointId = dto.SalesPointId,
            SalesPoint = salesPoint,
            User = user,
            Product = productInstance,
            ProductId = productInstanceId,
            UserId = dto.UserId,
            CreationTime = DateTime.Now
        };

        _dbContext.Offers.Add(offer);
        _dbContext.SaveChanges();
        return offer.Id;
    }

    public OffersWithTotalCount SearchOffers(int? countyId, string? productName, int? categoryId, int? pageSize,
        int? pageNumber,
        int? userId)
    {
        var categoriesIds = _dbContext.Database
            .SqlQuery<int>($@"
                WITH Subcategories AS
				(
		            SELECT DISTINCT Id, parent_id
		            FROM Categories
		            WHERE parent_id = {categoryId}

		            UNION ALL

		            SELECT Categories.Id, Categories.parent_id
		            FROM Subcategories, Categories
		            WHERE Categories.parent_id = Subcategories.Id
				)

				SELECT Id FROM Subcategories").ToList();
        if (categoryId is not null)
            categoriesIds.Add(categoryId.Value);

        var offers = _dbContext.Offers
            .Where(o => o.AdminId == null)
            .Include(o => o.Product)
            .ThenInclude(pi => pi.Product)
            .Include(o => o.User)
            .Include(o => o.SalesPoint)
            .ThenInclude(s => s.Address)
            .ThenInclude(a => a.County)
            .Where(o => EF.Functions.Like(o.Product.Product.Name, $"%{productName}%"))
            .Where(o => o.Product.Categories.Any(c => categoriesIds.Contains(c.Id)) || categoryId == null)
            .Where(o => countyId == null || o.SalesPoint.Address.County.Id == countyId)
            .OrderBy(o => o.CreationTime)
            .ProjectTo<OfferDto>(_mapper.ConfigurationProvider);
        //.ToList();

        var totalCount = offers.Count();

        if (pageSize.HasValue && pageNumber.HasValue)
            offers = offers.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value);

        var offersDto = offers.ToList();
        //var totalCount = offersDto.Count;

        if (userId == null)
            return new OffersWithTotalCount { Count = totalCount, Offers = offersDto };

        var user = _dbContext.Users.Include(u => u.Offers).FirstOrDefault(u => u.Id == userId);

        var userFavs = _dbContext.Users
            .Where(u => u.Id == userId)
            .Include(u => u.Offers)
            .SelectMany(u => u.Offers);

        offersDto.ForEach(o => { o.IsFavourite = userFavs.FirstOrDefault(uf => uf.Id == o.Id) != null; });

        return new OffersWithTotalCount { Count = totalCount, Offers = offersDto };
    }

    public void AddOfferToFavourites(int offerId, int userId)
    {
        var user = _dbContext.Users.Include(u => u.Offers).FirstOrDefault(u => u.Id == userId) ??
                   throw new NotFoundException("Uzytkownik nie istnieje");
        var offer = _dbContext.Offers.FirstOrDefault(o => o.Id == offerId) ?? throw new NotFoundException("Oferta nie istnieje");

        var offerInFavs = user.Offers.FirstOrDefault(f => f.Id == offerId);

        if (offerInFavs is null)
            user.Offers.Add(offer);
        else
            user.Offers.Remove(offerInFavs);

        _dbContext.SaveChanges();
    }

    public IEnumerable<CommentDto>? GetAllComments(int offerId, int? userId)
    {
        var comments = _dbContext
            .Comments
            .Where(c => c.OfferId == offerId && c.AdminId == null)
            .OrderByDescending(c => c.CreationTime)
            .Include(c => c.User)
            .Include(c => c.Users)
            .Include(c => c.UsersNavigation);

        foreach (var comment in comments)
        {
            var dislikers = comment.Users;
            var likers = comment.UsersNavigation;

            var liker = likers.FirstOrDefault(l => l.Id == userId);
            var disliker = dislikers.FirstOrDefault(l => l.Id == userId);

            var commentDto = _mapper.Map<CommentDto>(comment);
            commentDto.IsLikedOrDislikedByUser = liker == null ? (disliker == null ? null : false) : true;

            yield return commentDto;
        }
    }

    public OffersWithTotalCount GetFavouritesOffers(int userId, int pageSize, int pageNumber)
    {
        var offers = _dbContext
            .Users
            .Where(u => u.Id == userId)
            .Include(u => u.Offers)
            .SelectMany(u => u.Offers)
            .Where(o => o.AdminId == null)
            .Include(o => o.Product)
            .ThenInclude(pi => pi.Product)
            .Include(o => o.SalesPoint)
            .ThenInclude(s => s.Address)
            .ThenInclude(a => a.County)
            .ProjectTo<OfferDto>(_mapper.ConfigurationProvider);

        var offersDto = offers.ToList();
        var count = offers.Count();

        offers = offers.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

        offersDto.ForEach(o => o.IsFavourite = true);

        return new OffersWithTotalCount { Count = count, Offers = offersDto };
    }

    public int UpdateOffer(int id, decimal? price)
    {
        var offer = _dbContext.Offers.FirstOrDefault(o => o.Id == id)
                    ?? throw new NotFoundException("Oferta nie istnieje");

        offer.Price = price ?? offer.Price;
        _dbContext.SaveChanges();

        return id;
    }

    public void Ban(int adminId, int offerId)
    {
        var admin = _dbContext.Administrators.FirstOrDefault(u => u.UserId == adminId)
                    ?? throw new NotFoundException("Nie jestes administratorem");

        var offer = _dbContext.Offers.FirstOrDefault(o => o.Id == offerId)
                    ?? throw new NotFoundException("Oferta nie istnieje");

        offer.AdminId = adminId;
        offer.Admin = admin;

        _dbContext.SaveChanges();
    }

    public void Unban(int adminId, int offerId)
    {
        _ = _dbContext.Administrators.FirstOrDefault(u => u.UserId == adminId)
            ?? throw new NotFoundException("Nie jestes administratorem");

        var offer = _dbContext.Offers.FirstOrDefault(o => o.Id == offerId)
                    ?? throw new NotFoundException("Oferta nie istnieje");

        offer.AdminId = null;
        offer.Admin = null;

        _dbContext.SaveChanges();
    }
}