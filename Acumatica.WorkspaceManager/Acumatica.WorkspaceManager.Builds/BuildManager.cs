using Acumatica.WorkspaceManager.Builds.Properties;
using Acumatica.WorkspaceManager.Common;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Acumatica.WorkspaceManager.Builds
{
    public static class BuildManager
    {
        public delegate void ProgressCallbackDelegate(int percentDone, long counter, long total);

        public static IEnumerable<BuildPackage> GetBuildPackages(ProgressCallbackDelegate progressCallback)
        {
            var localBuildPackages = GetLocalBuildPackages().ToDictionary(k => k.Key, v => v);
            int count = 0;

            foreach (var remoteBuildPackage in GetRemoteBuildPackages())
            {
                remoteBuildPackage.SetIsRemote();

                if (localBuildPackages.ContainsKey(remoteBuildPackage.Key))
                {
                    localBuildPackages.Remove(remoteBuildPackage.Key);

                    string filePath = GetPathFromKey(remoteBuildPackage.Key);
                    string directory = Path.GetDirectoryName(filePath);
                    string installDirectory = Path.Combine(directory, Constants.filesDirectory);
                    string wizardPath = Path.Combine(installDirectory, Constants.dataDirectory, Constants.wizardFilename);
                    remoteBuildPackage.SetIsLocal(File.Exists(filePath));
                    remoteBuildPackage.SetIsInstalled(File.Exists(wizardPath));
                }

                if (progressCallback != null)
                {
                    progressCallback.Invoke(0, ++count, 0);
                }

                yield return remoteBuildPackage;
            }

            foreach(var localOnlyPackage in localBuildPackages.Values)
            {
                localOnlyPackage.SetIsLocal(true);
                yield return localOnlyPackage;
            }
        }

        private static IEnumerable<BuildPackage> GetLocalBuildPackages()
        {
            foreach (var file in GetLocalFiles(GetLocalRepositoryFolder()))
            {
                var key = GetKeyFromPath(file);

                BuildPackage buildPackage;

                if (key.EndsWith(string.Concat(Constants.acumaticaDirectory, Constants.slash, Resources.PackageName), StringComparison.InvariantCultureIgnoreCase) &&
                    BuildPackage.TryCreate(key, out buildPackage))
                {
                    yield return buildPackage;
                }
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

                        if (s3Object.Key.EndsWith(Resources.PackageName, StringComparison.InvariantCultureIgnoreCase) &&
                            BuildPackage.TryCreate(s3Object.Key, out buildPackage))
                        {
                            yield return buildPackage;
                        }
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

        public static void DownloadPackage(BuildPackage buildPackage, ProgressCallbackDelegate progressCallback)
        {
            if (!buildPackage.IsRemote)
                throw new Exception(Messages.downloadError);
            
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
                        response.WriteObjectProgressEvent += new EventHandler<WriteObjectProgressArgs>(delegate (object sender, WriteObjectProgressArgs e)
                        {
                            if (progressCallback != null)
                            {
                                progressCallback.Invoke(e.PercentDone, e.TransferredBytes, e.TotalBytes);
                            }
                        });

                        response.WriteResponseStreamToFile(dest);
                    }
                }
            }
        }

        public static void DeletePackage(BuildPackage buildPackage)
        {
            if (!buildPackage.IsLocal)
                throw new Exception(Messages.deletePackageError);
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
