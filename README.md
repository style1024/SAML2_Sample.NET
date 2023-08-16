# samlsp-dotnet-backend
> SAML service provider demonstrate in .NET

* Downoload .NET [here](https://dotnet.microsoft.com/download/dotnet/5.0)
* In this sample project, I use **ITfoxtec Identity SAML2**

## Commands
```bash
dotnet new sln --name SampleSP.NET
dotnet new web --name SampleSP.NET.Web
dotnet new gitignore
dotnet sln add SampleSP.NET.Web
```

``` bash
dotnet add package ITfoxtec.Identity.Saml2 --version 4.7.0
dotnet add package ITfoxtec.Identity.Saml2.MvcCore --version 4.7.0
```

## Reference
* ITfoxtec SAML 2.0: https://www.itfoxtec.com/IdentitySaml2