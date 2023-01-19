<!-- Original template: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>

<!-- PROJECT SHIELDS -->
<!-- [![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
-->
[![InfoTrackSEO-dev](https://github.com/mattnieland/InfoTrackSEO/actions/workflows/InfoTrackSEO-dev.yml/badge.svg)](https://github.com/mattnieland/InfoTrackSEO/actions/workflows/InfoTrackSEO-dev.yml)

[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/mattnieland/InfoTrackSEO">
    <img src="images/logo.png" alt="Logo" width="120" height="120">
  </a>

  <h3 align="center">InfoTrackSEO</h3>

  <p align="center">
    <a href="https://github.com/mattnieland/InfoTrackSEO"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/mattnieland/InfoTrackSEO">View Demo</a>
    ·
    <a href="https://github.com/mattnieland/InfoTrackSEO/issues">Report Bug</a>
    ·
    <a href="https://github.com/mattnieland/InfoTrackSEO/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
        <li><a href="#authentication">Authentication</a></li>
      </ul>
    </li>    
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

![Product Name Screen Shot][product-screenshot]

<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Built With

This API was built with the following tools:

* [![.NET][.NET]][.NET-url]
* [![EFCORE][EFCORE]][EFCORE-url]
* [![Docker][Docker]][Docker-url]
* [![Doppler][Doppler]][Doppler-url]
* [![Auth0][Auth0]][Auth0-url]
* [![Swagger][Swagger]][Swagger-url]
* [![Sentry][Sentry]][Sentry-url]
* [![Serilog][Serilog]][Serilog-url]
* [![NuGet][NuGet]][NuGet-url]
* [![ReSharper][ReSharper]][ReSharper-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GTTING STARTED -->
## Getting Started

The default template uses in memory database and seeds
its data automatically.  You can enable/disable any of the "extras" like Doppler, Auth0, Sentry, or Serilog.  If you'd like to use all of these,
read on.

### Prerequisites
You need the following pre-requisites to run this project:
* A Doppler account with a project (use local/dev/prod config environments)
* An Auth0 account (create an API & the read:urls, read:terms, read:data, write:urls, and write:terms scopes using this guide: https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/interactive)
* A Sentry account (use this guide to get a DSN https://docs.sentry.io/platforms/dotnet/guides/aspnetcore)
* Logging is provided by Serilog and dumps to Azure Table Storage (If you'd like a different sink, refer to this list: https://github.com/serilog/serilog/wiki/Provided-Sinks)

### Installation

To start, apply the following tokens to your Doppler configs:
  ```sh
  AUTH0_AUTHORITY = Your Auth0 Domain
  AUTH0_AUDIENCE = Your audience identifier (can be the anything)
  SENTRY_DSN = Your Sentry DSN from above
  DB_CONNECTION_STRING = Database connection (for dev & prod instances)
  LOGGING_CONNECTION_STRING = Your Azure Table Storage Connection String (If you don't have this key, logging will be skipped in setup)  
  ```

To access the doppler secrets, get a Doppler token and add a : to it (Doppler uses Basic Auth).  Refer to this page for how to authenticate to the Doppler API: https://docs.doppler.com/reference/api.

Finally, base64 encode (https://www.base64encode.org) the string and run the following command in the InfoTrackSEO.csproj folder:

* Set Doppler Token
  ```sh
  dotnet user-secrets set "DOPPLER_TOKEN" "YOUR_DOPPLER_TOKEN_HERE"
  ```

* Alternatively, you may use the appsettings.json or Dev/Prod transforms to set this value

That's it!  You're ready to compile and run via Visual Studio or VS Code.

### Authentication
Authentication is done via Auth0.  Create/Update/Delete endpoints require read:urls, read:terms, read:data, write:urls, and write:terms scopes
to work.  To get a Bearer token, use this guide (https://auth0.com/docs/secure/tokens/access-tokens/get-access-tokens)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

* Matt Nieland: matt.nieland@gmail.com
* Project Link: [https://github.com/mattnieland/InfoTrackSEO](https://github.com/mattnieland/InfoTrackSEO)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [Choose an Open Source License](https://choosealicense.com)
* [Img Shields](https://shields.io)
* https://hodo.dev/posts/post-03-dynamic-filter-ef for Dynamic Filtering code
* https://www.c-sharpcorner.com/blogs/rate-limiting-in-net-60 for rate limiting logic

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/othneildrew/Best-README-Template.svg?style=for-the-badge
[contributors-url]: https://github.com/mattnieland/InfoTrackSEO/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/othneildrew/Best-README-Template.svg?style=for-the-badge
[forks-url]: https://github.com/mattnieland/InfoTrackSEO/network/members
[stars-shield]: https://img.shields.io/github/stars/othneildrew/Best-README-Template.svg?style=for-the-badge
[stars-url]: https://github.com/mattnieland/InfoTrackSEO/stargazers
[issues-shield]: https://img.shields.io/github/issues/othneildrew/Best-README-Template.svg?style=for-the-badge
[issues-url]: https://github.com/mattnieland/InfoTrackSEO/issues
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/mattnieland/InfoTrackSEO/blob/main/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/mattnieland
[product-screenshot]: images/screenshot.png
[token]: images/token.png
[.NET]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=.net
[.NET-url]: https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-6.0
[EFCORE]: https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge
[EFCORE-url]: https://learn.microsoft.com/en-us/ef/core
[Docker]: https://img.shields.io/badge/Docker-000000?style=for-the-badge&logo=Docker
[Docker-url]: https://www.docker.com
[Auth0]: https://img.shields.io/badge/Auth0-000000?style=for-the-badge&logo=Auth0
[Auth0-url]: https://auth0.com
[Swagger]: https://img.shields.io/badge/Swagger-000000?style=for-the-badge&logo=Swagger
[Swagger-url]: https://swagger.io
[Doppler]: https://img.shields.io/badge/Doppler-000000?style=for-the-badge
[Doppler-url]: https://www.doppler.com
[Sentry]: https://img.shields.io/badge/Sentry-362D59?style=for-the-badge&logo=Sentry
[Sentry-url]: https://sentry.io
[Serilog]: https://img.shields.io/badge/Serilog-000000?style=for-the-badge
[Serilog-url]: https://serilog.net
[NuGet]: https://img.shields.io/badge/NuGet-004880?style=for-the-badge&logo=NuGet
[NuGet-url]: https://www.nuget.org
[ReSharper]: https://img.shields.io/badge/ReSharper-000000?style=for-the-badge&logo=ReSharper
[ReSharper-url]: https://www.jetbrains.com/resharper
