# FluentGit
A fluent API for performing Git operations in .NET.

## Why?

Well, [libgit2sharp](https://github.com/libgit2/libgit2sharp) provides a great object model for working with git repositories.

The goal of FluentGit is to provide a Fluent API that makes it quicker and easier to accomplish things, than using the `libgit2sharp` object model directly.

For example, rather than having to write 50 lines of code with `libgit2sharp` to instantiate a repo, add a remote, checkout a branch and checkout files, you could achieve the same things using `FluentGit` in say, only 10 lines - and also benefit from being guided through each operation by the fluent API (intellisense) every step of the way.

##Examples

### Complex example - aim's to demonstrate a decent surface of the API.

``` csharp
 var repo = new FluentRepo().Clone()
                .FromUrl("http://someurl.git")
                .ToDirectory("c:/myclone/")
                .WithCredentials("johnycash", "password")
                .Bare()
                .Obtain() // Performs the clone operation and returns an instance of the fluent repo builder.
                      .AddRemote(a =>
                          a.WithName("fluentgit")
                           .WithUrl("https://github.com/dazinator/fluentgit.git")
                           .WithFetchRefSpec(r =>
                               r.Source("refs/heads/master")
                                .Destination("refs/remotes/fluentgit/master")
                                .ForceUpdateIfFastForwardNotPossible()))
                      .AddRemote(a =>
                          a.WithName("libgit2sharp")
                           .WithUrl("https://github.com/libgit2/libgit2sharp.git")
                           .WithFetchRefSpec("+refs/heads/master:refs/remotes/libgit2sharp/master"))
                      .UpdateRemote(r => r.Name == "fluentgit",
                          u =>
                                u.ChangeUrlTo("https://sometherurl.com/fluentgit/fluentgit.git")
                                 .ChangeRefSpecs()
                                    .Add("+refs/heads/development:refs/remotes/fluentgit/development")
                                        // can alternatively use RefSpecBuilder to build refspec strings:
                                    .Add(b =>
                                        b.Source("refs/heads/qa")
                                         .Destination("refs/remotes/fluentgit/qa")
                                         .ForceUpdateIfFastForwardNotPossible())
                                    .AddIfNotExists("+refs/heads/*:refs/remotes/fluentgit/*")
                                    .Remove("+refs/heads/somebranch:refs/remotes/somebranch"))
                      .UpdateRemoteIfExists(r => r.Url == "non existing url",
                           u =>
                               // will only execute below update logic, if the remote in question exists.
                                 u.ChangeUrlTo("http://someurl.com")
                                  .ChangeRefSpecs()
                                     .Add("+whatever/*:whatever/*")
                                     .Remove("+blah/*:blah/*"))
                .WithBranch("development")
                  .CheckoutFile("readme.txt")
                  .CheckoutFileIfExists("licence.txt")
```

The above code should be self explanatory, but just in case - it demonstrates the following:-

1. Performs a bare clone of a repo - using authentication credentials.
2. Adds a remote named "fluentgit", with fetch refspecs (using refspec builder api)
3. Adds another remote named "libgit2sharp" with fetch refspecs (using refspec specified as a string)
4. Updates the "fluentgit" remote, to change it's URL, and also add in further fetch refspecs - as well as remove a fetch refspec.
5. Updates the remote that has the url "non existing url" - if it exists, and adds and removes fetch ref specs.
6. Switches to the "development" branch
7. Checks out a "readme.txt" file
8. If it exists, checks out a licence.txt file.

## Under the hood

FluentGit uses https://github.com/libgit2/libgit2sharp internally, and surfaces it in a fluent way. 










