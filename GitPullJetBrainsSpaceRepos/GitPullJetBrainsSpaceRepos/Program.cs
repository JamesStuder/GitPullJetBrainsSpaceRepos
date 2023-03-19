using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace GitPullJetBrainsSpaceRepos
{
    internal class Program
    {
        private static void Main()
        {
            string parentDirectory = Environment.GetEnvironmentVariable("GIT_PARENT_DIRECTORY", EnvironmentVariableTarget.User) ?? string.Empty;
            string gitPat = Environment.GetEnvironmentVariable("GIT_PAT", EnvironmentVariableTarget.User) ?? string.Empty;
            string gitUser = Environment.GetEnvironmentVariable("GIT_USER", EnvironmentVariableTarget.User) ?? string.Empty;
            string gitEmail = Environment.GetEnvironmentVariable("GIT_EMAIL", EnvironmentVariableTarget.User) ?? string.Empty;

            DirectoryInfo parentDirInfo = new (parentDirectory);
            ProcessDirectory(parentDirInfo, gitPat, gitUser, gitEmail);
        }

        private static void ProcessDirectory(DirectoryInfo directory, string gitPat, string gitUser, string gitEmail)
        {
            string gitDirectory = Path.Combine(directory.FullName, ".git");

            if (Directory.Exists(gitDirectory))
            {
                Console.WriteLine($"Processing repository: {directory.Name}");

                try
                {
                    Repository repo = new (directory.FullName);
                    FetchOptions options = new()
                    {
                        CredentialsProvider = (_, _, _) =>
                            new UsernamePasswordCredentials
                            {
                                Username = gitUser,
                                Password = gitPat
                            }
                    };

                    Remote remote = repo.Network.Remotes["origin"];
                    List<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToList();
                    Commands.Fetch(repo, remote.Name, refSpecs, options, null);

                    MergeResult mergeResult = Commands.Pull(repo, new Signature("username", gitEmail, DateTimeOffset.Now), new PullOptions
                    {
                        FetchOptions = options
                    });

                    Console.WriteLine(mergeResult.Status == MergeStatus.UpToDate
                        ? $"No changes to pull for repository: {directory.Name}"
                        : $"Successfully updated repository: {directory.Name}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error processing repository: {directory.Name}. {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                DirectoryInfo[] subdirectories = directory.GetDirectories();
                foreach (DirectoryInfo subDir in subdirectories)
                {
                    ProcessDirectory(subDir, gitPat,gitUser, gitEmail);
                }
            }
        }
    }
}