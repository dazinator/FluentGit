# FluentGit
A fluent API for performing Git operations in .NET.

## Why?

The goal with FluentGit, is to provide an API that guides the consumer through making operations with Git, in a straightforward manner - one that's easy to read, and easy to write.

##Examples

### Simple example.. Load a repo and checkout a branch:

``` csharp
FluentRepo.Load("C:/Myrepo/.git")
  .WithBranch("development")
    .Checkout();
```

### Complex example - demonstrating a larger surface of the API.

This example:

1. Performs a bare clone of a repo - using authentication credentials.
2. Adds a remote named "fluentgit", with fetch refspecs (using refspec builder api)
3. Adds another remote named "libgit2sharp" with fetch refspecs (using refspec specified as a string)
4. Switches to the "development" branch
5. Checks out a "readme.txt" file
6. If it exists, checks out a licence.txt file.

``` csharp
 var repo = new FluentRepo().Clone()
                .FromUrl("http://someurl.git")
                .ToDirectory("c:/myclone/")
                .WithCredentials("johnycash", "password")
                .Bare()
                .Obtain() // Performs the clone operation and returns an instance of the fluent repo builder.
                      .AddRemote(a =>
                          a.WithName("fluentgit")
                           .WithUrl("https://github.com/dazinator/FluentGit.git")
                           .WithFetchRefSpec(r =>
                               r.Source("refs/heads/master")
                                .Destination("refs/remotes/fluentgit/master")
                                .ForceUpdateIfFastForwardNotPossible()))
                      .AddRemote(a =>
                          a.WithName("libgit2sharp")
                           .WithUrl("https://github.com/libgit2/libgit2sharp.git")
                           .WithFetchRefSpec("+refs/heads/master:refs/remotes/libgit2sharp/master"));
                .WithBranch("development")
                  .CheckoutFile("readme.txt")
                  .CheckoutFileIfExists("licence.txt")
```

## Under the hood

FluentGit uses https://github.com/libgit2/libgit2sharp internally, and surfaces it in a fluent way. 










