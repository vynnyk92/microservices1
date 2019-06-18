﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShopingCart
{
    public interface IShoppingCartStore
    {
        Task<ShoppingCart> Get(int userId);
        Task Save(ShoppingCart shoppingCart);
    }
}
