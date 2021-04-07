using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;

namespace Api.Configuration.Compression
{
    internal static class CompressionExtension
    {
        public static void AddApiCompression(this IServiceCollection services)
        {
            services.AddResponseCompression();

            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        }
    }
}