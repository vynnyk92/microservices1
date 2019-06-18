using Nancy;
using Nancy.ModelBinding;
using ShoppingCart.EventFeed;
using ShoppingCart.ShopingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShopingCart
{
    public class ShopingCartModule : NancyModule
    {
        public ShopingCartModule(
            IShoppingCartStore shoppingCartStore,
            IProductCatalogClient productCatalogClient,
            IEventStore eventStore)
            : base("/shopingcart")
        {
            Get("/{userid:int}", parameters =>
            {
                var userId = (int)parameters.userid;
                return shoppingCartStore.Get(userId);
            });

            Post("/{userid:int}/items", async (parameters, _) =>
            {
                var productCatalogIds = this.Bind<int[]>();
                var userId = (int)parameters.userid;

                var shopingCart = await shoppingCartStore.Get(userId);
                var shopingCartItems = await productCatalogClient.GetShopingCartItems(productCatalogIds).ConfigureAwait(false);

                shopingCart.AddItems(shopingCartItems, eventStore);
                await shoppingCartStore.Save(shopingCart);

                return shopingCart;
            });

            Delete("/{userid:int}/items", async (parameters, _) =>
            {
                var productCatalogIds = this.Bind<int[]>();
                var userId = (int)parameters.userid;

                var shopingCart = await shoppingCartStore.Get(userId);

                shopingCart.RemoveItems(productCatalogIds, eventStore);
                await shoppingCartStore.Save(shopingCart);

                return shopingCart;
            });
        }
    }
}
