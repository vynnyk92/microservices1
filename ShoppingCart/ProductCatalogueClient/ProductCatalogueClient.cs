using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.ShopingCart;

namespace ShoppingCart.ProductCatalogueClient
{
    public class ProductCatalogueClient : IProductCatalogClient
    {
        private static string productCatalogueBaseUrl =
     @"http://private-05cc8-chapter2productcataloguemicroservice.apiary-mock.com";
        private static string getProductPathTemplate =
          "/products?productIds=[{0}]";

        public Task<IEnumerable<ShoppingCartItem>> GetShopingCartItems(IEnumerable<int> productCatalogueIds)
        {
            throw new NotImplementedException();
        }
    }
}
