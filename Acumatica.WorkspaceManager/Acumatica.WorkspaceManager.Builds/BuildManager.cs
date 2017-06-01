using Amazon.Auth.AccessControlPolicy;
using System.Collections.Generic;
using System.Linq;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System;
using Acumatica.WorkspaceManager.Builds.Properties;

namespace Acumatica.WorkspaceManager.Builds
{
    public static class BuildManager
    {
        public static void Test()
        {
            var builds = GetBuildPackages().ToList();
            foreach(var buildPackage in GetBuildPackages())
            {
                if (buildPackage.MajorVersion == 6 && buildPackage.MinorVersion == 1)
                    DownloadPackage(buildPackage);
            }
        }

        public static AmazonS3Client CreateAnonymousS3Client(S3Uri s3Uri)
        {
            return new AmazonS3Client(
                    null,//accessKey,
                    null,//secretKey,
                    s3Uri.Region
                    );
        }

        public static S3Uri GetS3Uri()
        {
            return new S3Uri(Resources.S3Url);
        }

        public static IEnumerable<BuildPackage> GetBuildPackages()
        {
            var s3Uri = GetS3Uri();
            using (var client = CreateAnonymousS3Client(s3Uri))
            {
                // Build your request to list objects in the bucket
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = s3Uri.Bucket
                };

                do
                {
                    // Build your call out to S3 and store the response
                    ListObjectsResponse response = client.ListObjects(request);

                    foreach (var s3Object in response.S3Objects)
                    {
                        BuildPackage buildPackage;
                        if (s3Object.Key.EndsWith(Resources.PackageName) && BuildPackage.TryCreate(s3Object.Key, out buildPackage))
                            yield return buildPackage;

                    }

                    // If the response is truncated, we'll make another request 
                    // and pull the next batch of keys
                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
            }
        }

        public static void DownloadPackage(BuildPackage buildPackage)
        {
            var s3Uri = GetS3Uri();
            using (var client = CreateAnonymousS3Client(s3Uri))
            { 
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = s3Uri.Bucket,
                    Key = buildPackage.Key
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), buildPackage.Key);
                    if (!File.Exists(dest))
                    {
                        response.WriteResponseStreamToFile(dest);
                    }
                }
            }
        }
    }

    
}
