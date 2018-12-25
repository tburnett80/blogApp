using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Common.Contracts.Accessors
{
    public interface IBlobAccessor
    {
        Task<IEnumerable<string>> ListBuckets();

        Task<IEnumerable<string>> EnumerateBucket(string bucket);

        Task AddBlob(string bucketName, string blobName, byte[] blob, string mime = null);

        Task AddBlob(string bucketName, string blobName, Stream blob, long len, string mime);

        Task<byte[]> GetBlob(string bucketName, string blobName);

        Task<bool> DeleteBucket(string bucketName);
    }
}
