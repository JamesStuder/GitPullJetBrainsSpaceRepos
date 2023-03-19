using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace GitPullJetBrainsSpaceRepos
{
    internal class Program
    {
        private enum MessageTypes
        {
            Information,
            Success,
            Failure
        }
        
        private static void Main()
        {
            string parentDirectory = Environment.GetEnvironmentVariable("GIT_PARENT_DIRECTORY", EnvironmentVariableTarget.User) ?? string.Empty;
            string gitPat = Environment.GetEnvironmentVariable("GIT_PAT", EnvironmentVariableTarget.User) ?? string.Empty;
            string gitUser = Environment.GetEnvironmentVariable("GIT_USER", EnvironmentVariableTarget.User) ?? string.Empty;
            string gitEmail = Environment.GetEnvironmentVariable("GIT_EMAIL", EnvironmentVariableTarget.User) ?? string.Empty;

            DirectoryInfo parentDirInfo = new (parentDirectory);
            ProcessDirectory(parentDirInfo, gitPat, gitUser, gitEmail);
            LogOutput("Press Any Key To Close", MessageTypes.Information);
            Console.ReadLine();
        }

        private static void ProcessDirectory(DirectoryInfo directory, string gitPat, string gitUser, string gitEmail)
        {
            string gitDirectory = Path.Combine(directory.FullName, ".git");

            if (Directory.Exists(gitDirectory))
            {
                LogOutput($"Processing repository: {directory.Name}", MessageTypes.Information);

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

                    if (mergeResult.Status == MergeStatus.UpToDate)
                    {
                        LogOutput($"No changes to pull for repository: {directory.Name}", MessageTypes.Information);
                    }
                    else
                    {
                        LogOutput($"Successfully updated repository: {directory.Name}", MessageTypes.Success);
                    }
                }
                catch (Exception ex)
                {
                    LogOutput($"Error processing repository: {directory.Name}. {ex.Message}", MessageTypes.Failure);
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

        private static void LogOutput(string message, MessageTypes messageType)
        {
            switch (messageType)
            {
                case MessageTypes.Information :
                    Console.ResetColor();
                    break;
                case MessageTypes.Success :
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case MessageTypes.Failure :
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}