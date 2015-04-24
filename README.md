# FluentGit
A fluent API for performing Git operations in .NET.

## Why?

The goal with FluentGit, is to provide an API that guides the consumer through making operations with Git, in a straightforward manner - one that's easy to read, and easy to write.

##Examples

### Load a repo and checkout a branch:

``` csharp
FluentRepo.Load("C:/Myrepo/.git")
  .WithBranch("development")
    .Checkout();
```

### Create a bare clone of a remote repo and checkout files from the development branch.

``` csharp
FluentRepo.Clone
  .FromUrl("http://someurl.git")
  .ToDirectory("c:/myclone/")
  .WithCredentials("johnycash","password")
  .Bare()
  .Obtain() // Performs the clone operation and returns an instance of the fluent repo builder.
    .WithBranch("development")
      .CheckoutFile("readme.txt")
      .CheckoutFileIfExists("licence.txt")
```

### Adding remotes.
``` csharp
 var fluentRepo = new FluentRepo().Load(GitRepoPath)
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
```

## Under the hood

FluentGit uses https://github.com/libgit2/libgit2sharp internally, and surfaces it in a fluent way. 










