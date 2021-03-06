﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using LVMS.FactuurSturen.Model;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        private List<Product> _cachedProducts;

        public async Task<Product[]> GetProducts(bool? allowCache = true)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;
            if ((bool)allowCache && _cachedProducts != null)
                return _cachedProducts.ToArray();

            var request = new RestRequest("products", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Product[]>(this, request);

            if ((bool)allowCache || _cachedProducts != null)
                _cachedProducts = new List<Product>(result);
            return result;
        }

        public async Task<Product> GetProduct(int id, bool? allowCache = true)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;
            if ((bool)allowCache && _cachedProducts != null && _cachedProducts.Any(p=>p.Id == id))
                return _cachedProducts.FirstOrDefault(p=>p.Id == id);

            var request = new RestRequest($"products/{id}", HttpMethod.Get, ContentTypes.Json);
            

            var result = await _httpClient.ExecuteWithPolicyAsync<Product>(this, request);
            if (result != null && (bool)allowCache)
            {
                StoreInCache(result);
            }
            return result;
        }

        public async Task<Product> CreateProduct(Product product, bool? storeInCache = true)
        {
            if (!storeInCache.HasValue)
                storeInCache = _allowResponseCaching;
            var request = new RestRequest("products", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };


            request.AddParameter(new[] { product});
            var response = await _httpClient.SendWithPolicyAsync<object>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                product.Id = Convert.ToInt32(response.Content);
                if ((bool)storeInCache)
                {
                    StoreInCache(product);
                }
                return product;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task<Product> UpdateProduct(Product product, bool? storeInCache = true)
        {
            if (!storeInCache.HasValue)
                storeInCache = _allowResponseCaching;
            var request = new RestRequest($"products/{product.Id}", HttpMethod.Put, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            request.AddParameter(new[] { product });
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if ((bool)storeInCache)
                {
                    StoreInCache(product);
                }
                return product;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task DeleteProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            var clientNr = product.Id;
            await DeleteProduct(clientNr);

            RemoveProductFromCache(product);
        }

        public async Task DeleteProduct(int productId)
        {
            var request = new RestRequest($"products/{productId}", HttpMethod.Delete, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (!response.HttpResponseMessage.IsSuccessStatusCode)
            {
                throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
            }
            RemoveProductFromCache(productId);
        }

        private void StoreInCache(Product product)
        {
            if (_cachedProducts == null)
                _cachedProducts = new List<Product>();
            _cachedProducts.Add(product);
        }

       

        private void RemoveProductFromCache(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productId = product.Id;
            RemoveProductFromCache(productId);
        }

        private void RemoveProductFromCache(int productId)
        {
            if (_cachedProducts == null)
                return;
            lock (_cachedProducts)
            {
                if (_cachedProducts != null && _cachedProducts.Any(p => p.Id == productId))
                    _cachedProducts.Remove(_cachedProducts.First(p => p.Id == productId));
            }
        }
    }
}
