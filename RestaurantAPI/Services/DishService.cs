using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Services
{
    public class DishService : IDishService
    {
        RestaurantDbContex _dbContext;
        IMapper _mapper;
        public DishService(RestaurantDbContex dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public int Create(int restaurantId, CreateDishDto dishDto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dishDto);
            dishEntity.RestaurantId = restaurantId;

            _dbContext.Dishes.Add(dishEntity);
            _dbContext.SaveChanges();

            return dishEntity.Id;
        }
        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = GetDishById(dishId, restaurant);

            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;
        }



        public void Delete(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = GetDishById(dishId, restaurant);

            _dbContext.Remove(dish);
            _dbContext.SaveChanges();
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }
        public void DeleteAll(int restaurantId)
        {

            var restaurant = GetRestaurantById(restaurantId);

            _dbContext.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
        }
        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext.Restaurant
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            return restaurant;
        }
        private Dish GetDishById(int dishId, Restaurant restaurant)
        {
            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);

            if (dish is null)
                throw new NotFoundException("Dish not found");

            return dish;
        }
    }
}
