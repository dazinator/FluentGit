# FluentGit
A fluent API for managing Git repositories in .NET.

## Load a repo and checkout a branch:

``` csharp
FluentRepo.Load("C:/Myrepo/.git")
  .WithBranch("development")
    .Checkout();
```

## Create a bare clone of a repo and checkout particular files from a partciular branch.

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

## Why?

Because I like it and because I can! ;)









