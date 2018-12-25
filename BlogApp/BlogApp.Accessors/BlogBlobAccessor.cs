using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BlogApp.Common.Contracts.Accessors;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel;

namespace BlogApp.Accessors
{
    public sealed class BlogBlobAccessor : IBlobAccessor
    {
        #region Constructor and private members
        private readonly IConfiguration _config;
        private readonly MinioClient _client;

        public BlogBlobAccessor(IConfiguration config)
        {
            _config = config
                ?? throw new ArgumentNullException(nameof(config));

            _client = new Minio.MinioClient(_config["minio:endPoint"],
                _config["minio:accessKey"], _config["minio:secretKey"]);
        }
        #endregion

        public async Task<IEnumerable<string>> ListBuckets()
        {
            var buckets = await _client.ListBucketsAsync();
            return buckets.Buckets.Select(b => b.Name);
        }

        public async Task<IEnumerable<string>> EnumerateBucket(string bucket)
        {
            if (!await _client.BucketExistsAsync(bucket))
                return new string[0];

            var blobNames = new List<string>();
            var observable = _client.ListObjectsAsync(bucket);
            using (observable.Subscribe(item => blobNames.Add(item.Key)))
            {
                await observable;
                return blobNames.ToList();
            }
        }

        public async Task AddBlob(string bucketName, string blobName, byte[] blob, string mime = null)
        {
             await AddBlob(bucketName, blobName, new MemoryStream(blob), blob.Length, mime);
        }

        public async Task AddBlob(string bucketName, string blobName, Stream blob, long len, string mime)
        {
            if (!await _client.BucketExistsAsync(bucketName))
                await _client.MakeBucketAsync(bucketName);

            await _client.PutObjectAsync(bucketName, blobName, blob, len, mime);
        }

        public async Task<byte[]> GetBlob(string bucketName, string blobName)
        {
            using (var ms = new MemoryStream())
            {
                return await _client.GetObjectAsync(bucketName, blobName, (stream) =>
                {
                    stream.CopyTo(ms);
                }).ContinueWith(tsk => ms.ToArray());
            }
        }

        public async Task<bool> DeleteBucket(string bucketName)
        {
            if (!await _client.BucketExistsAsync(bucketName))
                return false;

            try
            {
                //Clear items from bucket
                var items = await EnumerateBucket(bucketName);
                //await _client.RemoveObjectAsync(bucketName, items);
                foreach (var item in items)
                    await _client.RemoveObjectAsync(bucketName, item);
                
                //remove bucket itself
                await _client.RemoveBucketAsync(bucketName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
