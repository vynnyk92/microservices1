using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using ShoppingCart.ShopingCart;

namespace ShoppingCart.ProductCatalogueClient
{
    public class ProductCatalogueClient : IProductCatalogClient
    {

        private static AsyncRetryPolicy exponentialRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

        private static string productCatalogueBaseUrl = @"http://private-05cc8-chapter2productcataloguemicroservice.apiary-mock.com";
        private static string getProductPathTemplate ="/products?productIds=[{0}]";

        public async Task<IEnumerable<ShoppingCartItem>> GetShopingCartItems(IEnumerable<int> productCatalogueIds)
        {
            return await exponentialRetryPolicy.ExecuteAsync(async () => await GetShopingCartItemsFromCatalogue(productCatalogueIds)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetShopingCartItemsFromCatalogue(IEnumerable<int> productCatalogueIds)
        {
            var response = await RequestProductFromProductCatalogue(productCatalogueIds).ConfigureAwait(false);

            var result = await ConvertShoppingCartItems(response).ConfigureAwait(false);

            return result;
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertShoppingCartItems(HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.EnsureSuccessStatusCode();
            var prods = JsonConvert.DeserializeObject<List<ProductCatalogueProduct>>(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));

            return prods.Select(p => new ShoppingCartItem(int.Parse(p.ProductId), p.ProductName, p.ProductDescription, p.Price));
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalogue(IEnumerable<int> productCatalogueIds)
        {
            var prodSource = string.Format(getProductPathTemplate, string.Join(",", productCatalogueIds));

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(productCatalogueBaseUrl);
                return await httpClient.GetAsync(prodSource).ConfigureAwait(false);
            }
        }
    }

    internal class ProductCatalogueProduct
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Money Price { get; set; }
    }
}
