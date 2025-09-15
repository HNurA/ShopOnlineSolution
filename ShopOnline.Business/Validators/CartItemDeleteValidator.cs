using FluentValidation;
using ShopOnline.DataAccess.Repositories.Contracts;

namespace ShopOnline.Business.Validators
{
    public class CartItemDeleteValidator : AbstractValidator<int>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public CartItemDeleteValidator(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;

            RuleFor(cartItemId => cartItemId)
                .GreaterThan(0)
                .WithMessage("Valid Cart Item Id is required");

            RuleFor(cartItemId => cartItemId)
                .MustAsync(async (cartItemId, cancellation) => await CartItemExists(cartItemId))
                .WithMessage(cartItemId => $"Cart item with ID {cartItemId} not found");
        }

        private async Task<bool> CartItemExists(int cartItemId)
        {
            try
            {
                var cartItem = await _shoppingCartRepository.GetItem(cartItemId);
                return cartItem != null;
            }
            catch
            {
                return false;
            }
        }
    }
}