using System;
using System.Collections.Generic;
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
            if (await _client.BucketExistsAsync(bucketName))
            {
                //Clear items from bucket
                try
                {
                    var item = await _client.ListObjectsAsync(bucketName);
                    while (item != null)
                    {
                        await _client.RemoveObjectAsync(bucketName, item.Key);
                        item = await _client.ListObjectsAsync(bucketName);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Sequence contains no elements.")
                        return false;
                }

                try
                {
                    await _client.RemoveBucketAsync(bucketName);
                }
                catch (Exception ex)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
