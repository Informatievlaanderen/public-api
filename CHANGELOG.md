# [2.6.0](https://github.com/informatievlaanderen/public-api/compare/v2.5.1...v2.6.0) (2019-04-17)


### Features

* use proper logging for aws fargate ([124cbaa](https://github.com/informatievlaanderen/public-api/commit/124cbaa))

## [2.5.1](https://github.com/informatievlaanderen/public-api/compare/v2.5.0...v2.5.1) (2019-04-17)


### Bug Fixes

* add forgotten line extension for docker run ([62a4e8e](https://github.com/informatievlaanderen/public-api/commit/62a4e8e))

# [2.5.0](https://github.com/informatievlaanderen/public-api/compare/v2.4.0...v2.5.0) (2019-04-17)


### Features

* add containerid to serilog on aws ([211bb14](https://github.com/informatievlaanderen/public-api/commit/211bb14))

# [2.4.0](https://github.com/informatievlaanderen/public-api/compare/v2.3.4...v2.4.0) (2019-04-17)


### Bug Fixes

* install datadog in docker ([9ce8867](https://github.com/informatievlaanderen/public-api/commit/9ce8867))


### Features

* update dependencies and add datadog ([9010c84](https://github.com/informatievlaanderen/public-api/commit/9010c84))

## [2.3.4](https://github.com/informatievlaanderen/public-api/compare/v2.3.3...v2.3.4) (2019-04-08)


### Bug Fixes

* only run push to prod on master branch ([330b081](https://github.com/informatievlaanderen/public-api/commit/330b081))
* only run push to prod on master branch ([3e5ea82](https://github.com/informatievlaanderen/public-api/commit/3e5ea82))

## [2.3.3](https://github.com/informatievlaanderen/public-api/compare/v2.3.2...v2.3.3) (2019-04-08)


### Bug Fixes

* manual hold for production ([ad5685c](https://github.com/informatievlaanderen/public-api/commit/ad5685c))

## [2.3.2](https://github.com/informatievlaanderen/public-api/compare/v2.3.1...v2.3.2) (2019-04-08)


### Bug Fixes

* manual hold for production ([5d60f23](https://github.com/informatievlaanderen/public-api/commit/5d60f23))
* manual hold for production ([ac04de8](https://github.com/informatievlaanderen/public-api/commit/ac04de8))
* manual hold for production ([4e7789b](https://github.com/informatievlaanderen/public-api/commit/4e7789b))

## [2.3.1](https://github.com/informatievlaanderen/public-api/compare/v2.3.0...v2.3.1) (2019-04-08)


### Bug Fixes

* push docker to production registry ([e8d38a5](https://github.com/informatievlaanderen/public-api/commit/e8d38a5))
* push docker to production registry ([d248ba9](https://github.com/informatievlaanderen/public-api/commit/d248ba9))

# [2.3.0](https://github.com/informatievlaanderen/public-api/compare/v2.2.1...v2.3.0) (2019-03-25)


### Features

* add publicservice registry ([9063f14](https://github.com/informatievlaanderen/public-api/commit/9063f14))

## [2.2.1](https://github.com/informatievlaanderen/public-api/compare/v2.2.0...v2.2.1) (2019-03-07)


### Bug Fixes

* generate docs again ([dec8947](https://github.com/informatievlaanderen/public-api/commit/dec8947))

# [2.2.0](https://github.com/informatievlaanderen/public-api/compare/v2.1.0...v2.2.0) (2019-03-07)


### Features

* add address and streetname filters ([2fd13d8](https://github.com/informatievlaanderen/public-api/commit/2fd13d8))

# [2.1.0](https://github.com/informatievlaanderen/public-api/compare/v2.0.3...v2.1.0) (2019-03-06)


### Features

* add addresses ([2b12cb9](https://github.com/informatievlaanderen/public-api/commit/2b12cb9))

## [2.0.3](https://github.com/informatievlaanderen/public-api/compare/v2.0.2...v2.0.3) (2019-01-24)


### Bug Fixes

* not modified 304 examples have to be empty ([0e14165](https://github.com/informatievlaanderen/public-api/commit/0e14165))
* not modified 304 must not contain a body ([4d7d755](https://github.com/informatievlaanderen/public-api/commit/4d7d755))

## [2.0.2](https://github.com/informatievlaanderen/public-api/compare/v2.0.1...v2.0.2) (2019-01-22)


### Bug Fixes

* rename perceelId parameter to capaKey ([380518b](https://github.com/informatievlaanderen/public-api/commit/380518b))

## [2.0.1](https://github.com/informatievlaanderen/public-api/compare/v2.0.0...v2.0.1) (2019-01-22)

# [2.0.0](https://github.com/informatievlaanderen/public-api/compare/v1.2.0...v2.0.0) (2019-01-22)


### Features

* add parcel, fixes [#1](https://github.com/informatievlaanderen/public-api/issues/1) ([c792c95](https://github.com/informatievlaanderen/public-api/commit/c792c95))
* syndication always returns full objects ([0f5f78e](https://github.com/informatievlaanderen/public-api/commit/0f5f78e)), closes [#3](https://github.com/informatievlaanderen/public-api/issues/3)


### BREAKING CHANGES

* Syndication does not have an embed option anymore. It is always the full response.

# [1.2.0](https://github.com/informatievlaanderen/public-api/compare/v1.1.1...v1.2.0) (2019-01-17)


### Features

* add postal codes ([d762918](https://github.com/informatievlaanderen/public-api/commit/d762918))

## [1.1.1](https://github.com/informatievlaanderen/public-api/compare/v1.1.0...v1.1.1) (2019-01-15)


### Bug Fixes

* update route for streetname detail ([9f8eed0](https://github.com/informatievlaanderen/public-api/commit/9f8eed0))

# [1.1.0](https://github.com/informatievlaanderen/public-api/compare/v1.0.2...v1.1.0) (2019-01-15)


### Features

* add streetnames to public api ([21b16ca](https://github.com/informatievlaanderen/public-api/commit/21b16ca))

## [1.0.2](https://github.com/informatievlaanderen/public-api/compare/v1.0.1...v1.0.2) (2019-01-14)

## [1.0.1](https://github.com/informatievlaanderen/public-api/compare/v1.0.0...v1.0.1) (2019-01-14)


### Bug Fixes

* correctly deduce accepttype from fileextension when it is passed ([5557e81](https://github.com/informatievlaanderen/public-api/commit/5557e81))

# 1.0.0 (2019-01-10)


### Features

* open source with EUPL-1.2 license as 'agentschap Informatie Vlaanderen ([efc5b26](https://github.com/informatievlaanderen/public-api/commit/efc5b26))
