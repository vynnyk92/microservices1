using Microsoft.Extensions.Configuration;
using ShoppingCart.EventFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace ShoppingCart.ShopingCart
{
    public class ShoppingCartStore : IShoppingCartStore
    {
        private readonly string connString = "Server=GOLPE\\SQLEXPRESS;Database=ShoppingCartStore;Trusted_Connection=True;MultipleActiveResultSets=true";

        private const string readItemsSql = @"select * from ShoppingCart, ShoppingCartItems
where ShoppingCartItems.ShoppingCartId = ShoppingCart.Id
and ShoppingCart.UserId=@UserId";

        private const string deleteAllForShoppingCartSql =
 @"delete item from ShoppingCartItems item
inner join ShoppingCart cart on item.ShoppingCartId = cart.Id
and cart.UserId=@UserId";

        private const string addAllForShoppingCartSql =
  @"insert into ShoppingCartItems 
(ShoppingCartId, ProductCatalogId, ProductName, 
ProductDescription, Amount, Currency)
values 
(1, @ProductCatalogId, @ProductName, @ProductDescription, @Amount, @Currency)";

       
        public async Task<ShoppingCart> Get(int userId)
        {
            
            using (var conn = new SqlConnection(connString))
            {
                var items = await conn.QueryAsync<dynamic>(readItemsSql, new { UserId = userId });

                var shopCartItems = items.Select(it => new ShoppingCartItem(it.ProductCatalogueId, it.ProductName, it.ProductDescription, new Money(it.Amount, it.Currency)));

                var shopCart = new ShoppingCart(userId, shopCartItems);
                return shopCart;
            }

        }

        public async Task Save(ShoppingCart shoppingCart)
        {

            using (var conn = new SqlConnection(connString))
            {
                using (var tx = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted, "1"))
                {
                    //await conn.ExecuteAsync(
                    //  deleteAllForShoppingCartSql,
                    //  new { UserId = shoppingCart.UserId },
                    //  tx).ConfigureAwait(false);
                    await conn.ExecuteAsync(
                      addAllForShoppingCartSql,
                      shoppingCart.Items,
                      tx).ConfigureAwait(false);
                }
            }
        }
    }
}
