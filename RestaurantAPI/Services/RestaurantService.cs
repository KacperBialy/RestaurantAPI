using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RestaurantAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContex _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContex dbContex, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContex;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        public void Update(int id, UpdateRestaurantDto dto)
        {

            var restaurant = _dbContext
                .Restaurant
                .FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResoult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResoult.Succeeded)
            {
                throw new ForbidException();
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();

        }
        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
                .Restaurant
                .FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResoult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResoult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Restaurant.Remove(restaurant);
            _dbContext.SaveChanges();
        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
                .Restaurant
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return restaurantDto;
        }
        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
               .Restaurant
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .Where(r => query.SearchPharse == null || (r.Name.ToLower().Contains(query.SearchPharse.ToLower()) || r.Description.ToLower().Contains(query.SearchPharse.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>()
                {
                    {nameof(Restaurant.Name),r=>r.Name },
                    {nameof(Restaurant.Description),r=>r.Description },
                    {nameof(Restaurant.Category),r=>r.Category }
            };

                var selectedColumn = columsSelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
               .Skip(query.PageSize * (query.PageNumber - 1))
               .Take(query.PageSize)
               .ToList();

            var totalItemCount = baseQuery.Count();
            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDto, totalItemCount, query.PageSize, query.PageNumber);

            return result;
        }
        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<CreateRestaurantDto, Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;

            _dbContext.Restaurant.Add(restaurant);
            _dbContext.SaveChanges();
            return restaurant.Id;
        }
    }
}
