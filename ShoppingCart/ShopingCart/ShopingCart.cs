﻿using ShoppingCart.EventFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShopingCart
{
    public class ShoppingCart
    {
        private HashSet<ShoppingCartItem> items = new HashSet<ShoppingCartItem>();

        public int UserId { get; }
        public IEnumerable<ShoppingCartItem> Items { get { return items; } }

        public ShoppingCart(int userId)
        {
            this.UserId = userId;
        }

        public ShoppingCart(int userId, IEnumerable<ShoppingCartItem> shoppingCartItems)
        {
            this.UserId = userId;
            shoppingCartItems.ToList().ForEach(item => this.items.Add(item));
        }

        public void AddItems(
          IEnumerable<ShoppingCartItem> shoppingCartItems,
          IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
                if (this.items.Add(item))
                    eventStore.Raise(
                      "ShoppingCartItemAdded",
                      new { UserId, item });
        }

        public void RemoveItems(
          int[] productCatalogueIds,
          IEventStore eventStore)
        {
            items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
        }
    }

    public class ShoppingCartItem
    {
        public int ProductCatalogueId { get; }
        public string ProductName { get; }
        public string ProductDescription { get; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public Money Price { get; }

        public ShoppingCartItem(
          int productCatalogueId,
          string productName,
          string description,
          Money price)
        {
            this.ProductCatalogueId = productCatalogueId;
            this.ProductName = productName;
            this.ProductDescription = description;
            this.Price = price;

            this.Currency = price.Currency;
            this.Amount = (int)price.Amount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = obj as ShoppingCartItem;
            return this.ProductCatalogueId.Equals(that.ProductCatalogueId);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.ProductCatalogueId.GetHashCode();
        }
    }

    public class Money
    {
        public string Currency { get; }
        public decimal Amount { get; }

        public Money(string currency, decimal amount)
        {
            this.Currency = currency;
            this.Amount = amount;
        }
    }

}