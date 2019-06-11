using ShoppingCart.ShopingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public interface IProductCatalogClient
    {
        Task<IEnumerable<ShoppingCartItem>> GetShopingCartItems(IEnumerable<int> productCatalogueIds);
    }
}
