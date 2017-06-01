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
        public static IEnumerable<BuildPackage> GetBuildPackages()
        {
            var localBuildPackages = GetLocalBuildPackages().ToDictionary(k => k.Key, v => v);

            foreach(var remoteBuildPackage in GetRemoteBuildPackages())
            {
                remoteBuildPackage.SetIsRemote();
                if (localBuildPackages.ContainsKey(remoteBuildPackage.Key))
                {
                    localBuildPackages.Remove(remoteBuildPackage.Key);
                    remoteBuildPackage.SetIsLocal();
                }

                yield return remoteBuildPackage;
            }

            foreach(var localOnlyPackage in localBuildPackages.Values)
            {
                localOnlyPackage.SetIsLocal();
                yield return localOnlyPackage;
            }
        }
        private static IEnumerable<BuildPackage> GetLocalBuildPackages()
        {
            foreach (var file in GetLocalFiles(GetLocalRepositoryFolder()))
            {
                var key = GetKeyFromPath(file);

                BuildPackage buildPackage;
                if (key.EndsWith(Resources.PackageName) && BuildPackage.TryCreate(key, out buildPackage))
                    yield return buildPackage;

            }

        }  
        private static IEnumerable<BuildPackage> GetRemoteBuildPackages()
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
            if (!buildPackage.IsRemote)
                throw new Exception("Cannot download package that is not available online");
            
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
                    string dest = GetPathFromKey(buildPackage.Key);
                    if (!File.Exists(dest))
                    {
                        response.WriteResponseStreamToFile(dest);
                    }
                }
            }
        }

        public static void DeletePackage(BuildPackage buildPackage)
        {
            if (!buildPackage.IsLocal)
                throw new Exception("Only local packages can be deleted");
        }

        public static string GetKeyFromPath(string filePath)
        {
            return filePath.Replace(GetLocalRepositoryFolder() + Path.DirectorySeparatorChar, string.Empty).Replace(Path.DirectorySeparatorChar, '/');
        }

        public static string GetPathFromKey(string key)
        {
            return Path.Combine(GetLocalRepositoryFolder(), key.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string GetLocalRepositoryFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.LocalRepositoryName);
        }

        private static AmazonS3Client CreateAnonymousS3Client(S3Uri s3Uri)
        {
            return new AmazonS3Client(
                    null,//accessKey,
                    null,//secretKey,
                    s3Uri.Region
                    );
        }

        private static S3Uri GetS3Uri()
        {
            return new S3Uri(Resources.S3Url);
        }

        //https://stackoverflow.com/a/929418/
        private static IEnumerable<string> GetLocalFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
    }
}
