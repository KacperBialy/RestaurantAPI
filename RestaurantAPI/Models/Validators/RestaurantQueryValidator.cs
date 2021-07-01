using FluentValidation;
using RestaurantAPI.Entities;
using System;
using System.Linq;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new int[] { 5, 10, 15 };
        private string[] allowedSortByCollumnsNames = { nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category) };
        public RestaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
                {
                    if (!allowedPageSizes.Contains(value))
                    {
                        context.AddFailure("PageSize", $"PageSize must in [{string.Join(',', allowedPageSizes)}]");
                    }
                });
            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByCollumnsNames.Contains(value))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(',', allowedSortByCollumnsNames)}]");
        }
    }
}
