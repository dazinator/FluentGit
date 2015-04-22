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

## Under the hood

FluentGit uses libgit2sharp internally, and surfaces it in a fluent way. 










