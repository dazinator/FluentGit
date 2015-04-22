# FluentGit
Use a fluent API for Git operations in .NET.

##Examples

### Load a repo and checkout a branch:

``` csharp
FluentRepo.Load("C:/Myrepo/.git")
  .WithBranch("development")
    .Checkout();
```

### Create a bare clone of a repo and checkout particular files from a partciular branch.

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

The goal with FluentGit, is to provide an API that guides the user through making operations with Git, in a straightforward, easy to read, and easy to write, manner.

## Under the hood

FluentGit uses libgit2sharp under the hood, but wraps it in a fluent API. 
Using libgit2sharp is great 









