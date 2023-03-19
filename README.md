# GitPullJetBrainsSpaceRepos
GitPullJetBrainsSpaceRepos is a command-line tool that helps you automatically pull all JetBrains Space repositories stored in a given directory and its subdirectories. The tool uses environment variables to securely handle sensitive information such as the Personal Access Token (PAT), Git user, and Git email.

# Environment Variables
Before running the tool, you need to set the following environment variables:

* `GIT_PARENT_DIRECTORY`: The parent directory containing the JetBrains Space repositories.
* `GIT_PAT`: Your Personal Access Token (PAT) from JetBrains Space.
* `GIT_USER`: Your Git user name, used for commit identification.
* `GIT_EMAIL`: Your Git email, used for commit identification.

To set the environment variables, open a command prompt and execute the following commands:

```
setx GIT_PARENT_DIRECTORY "your_parent_directory"
setx GIT_PAT "your_personal_access_token"
setx GIT_USER "your_git_user"
setx GIT_EMAIL "your_git_email"
```
Replace your_parent_directory, your_personal_access_token, your_git_user, and your_git_email with your actual values.

**Note**: These environment variables are stored for the current user.

# Building and Running the Application
To build and run the application, follow these steps:
1. Clone the repository or download the source code.
2. Open a command prompt and navigate to the project's root directory.
3. Build the application:

```
dotnet build
```

4. Run the application:

```
dotnet run --project GitPullJetBrainsSpaceRepos
```

The tool will process each JetBrains Space repository found in the parent directory and its subdirectories, fetch the latest changes, and perform a pull operation.
