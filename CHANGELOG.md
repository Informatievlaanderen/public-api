## [2.14.1](https://github.com/informatievlaanderen/public-api/compare/v2.14.0...v2.14.1) (2019-04-25)


### Bug Fixes

* escape postalcode filterheader value ([f333546](https://github.com/informatievlaanderen/public-api/commit/f333546))

# [2.14.0](https://github.com/informatievlaanderen/public-api/compare/v2.13.2...v2.14.0) (2019-04-19)


### Features

* move to sockets transport ([a5282a1](https://github.com/informatievlaanderen/public-api/commit/a5282a1))

## [2.13.2](https://github.com/informatievlaanderen/public-api/compare/v2.13.1...v2.13.2) (2019-04-19)


### Bug Fixes

* make sure every action has a unique name for redoc urls ([9434615](https://github.com/informatievlaanderen/public-api/commit/9434615))

## [2.13.1](https://github.com/informatievlaanderen/public-api/compare/v2.13.0...v2.13.1) (2019-04-19)


### Bug Fixes

* add available sort fields to documentation) ([e0a14d8](https://github.com/informatievlaanderen/public-api/commit/e0a14d8))

# [2.13.0](https://github.com/informatievlaanderen/public-api/compare/v2.12.2...v2.13.0) (2019-04-19)


### Bug Fixes

* add mapping from public names to internal names for sort ([d69562a](https://github.com/informatievlaanderen/public-api/commit/d69562a))
* pass in descending sort order properly ([3ddf511](https://github.com/informatievlaanderen/public-api/commit/3ddf511))


### Features

* add sorting, fixes [#7](https://github.com/informatievlaanderen/public-api/issues/7) ([03e61d4](https://github.com/informatievlaanderen/public-api/commit/03e61d4))

## [2.12.2](https://github.com/informatievlaanderen/public-api/compare/v2.12.1...v2.12.2) (2019-04-19)

## [2.12.1](https://github.com/informatievlaanderen/public-api/compare/v2.12.0...v2.12.1) (2019-04-19)


### Bug Fixes

* add responseoptions for swagger ([0e26e62](https://github.com/informatievlaanderen/public-api/commit/0e26e62))

# [2.12.0](https://github.com/informatievlaanderen/public-api/compare/v2.11.0...v2.12.0) (2019-04-19)


### Features

* add parcels feed ([a011c60](https://github.com/informatievlaanderen/public-api/commit/a011c60))

# [2.11.0](https://github.com/informatievlaanderen/public-api/compare/v2.10.4...v2.11.0) (2019-04-19)


### Features

* add brotli compression ([945beaa](https://github.com/informatievlaanderen/public-api/commit/945beaa))
* add filtering on municipality ([2b32549](https://github.com/informatievlaanderen/public-api/commit/2b32549))

## [2.10.4](https://github.com/informatievlaanderen/public-api/compare/v2.10.3...v2.10.4) (2019-04-19)


### Bug Fixes

* add query parameters to the next page link of a list ([30efff1](https://github.com/informatievlaanderen/public-api/commit/30efff1))
* add query parameters to the next uri for atom feeds ([eaaf944](https://github.com/informatievlaanderen/public-api/commit/eaaf944))
* create cache key based of full request query of a list ([e31dc15](https://github.com/informatievlaanderen/public-api/commit/e31dc15))
* group parameter name + value in cachekey format ([650d59a](https://github.com/informatievlaanderen/public-api/commit/650d59a))

## [2.10.3](https://github.com/informatievlaanderen/public-api/compare/v2.10.2...v2.10.3) (2019-04-19)

## [2.10.2](https://github.com/informatievlaanderen/public-api/compare/v2.10.1...v2.10.2) (2019-04-18)

## [2.10.1](https://github.com/informatievlaanderen/public-api/compare/v2.10.0...v2.10.1) (2019-04-18)

# [2.10.0](https://github.com/informatievlaanderen/public-api/compare/v2.9.0...v2.10.0) (2019-04-18)


### Features

* add traceid to logcontext ([beeb0f1](https://github.com/informatievlaanderen/public-api/commit/beeb0f1))

# [2.9.0](https://github.com/informatievlaanderen/public-api/compare/v2.8.3...v2.9.0) (2019-04-18)


### Features

* add traceid to log output when available ([8f7cff1](https://github.com/informatievlaanderen/public-api/commit/8f7cff1))

## [2.8.3](https://github.com/informatievlaanderen/public-api/compare/v2.8.2...v2.8.3) (2019-04-18)


### Bug Fixes

* prevent traceid from being negative ([e487f95](https://github.com/informatievlaanderen/public-api/commit/e487f95))

## [2.8.2](https://github.com/informatievlaanderen/public-api/compare/v2.8.1...v2.8.2) (2019-04-18)


### Bug Fixes

* properly parse aws trace id to datadog apm trace id ([87dfbef](https://github.com/informatievlaanderen/public-api/commit/87dfbef))

## [2.8.1](https://github.com/informatievlaanderen/public-api/compare/v2.8.0...v2.8.1) (2019-04-18)


### Bug Fixes

* properly register tracesource factory ([c4adbee](https://github.com/informatievlaanderen/public-api/commit/c4adbee))

# [2.8.0](https://github.com/informatievlaanderen/public-api/compare/v2.7.3...v2.8.0) (2019-04-18)


### Features

* add extra debug logging for traceagent ([c01a7fa](https://github.com/informatievlaanderen/public-api/commit/c01a7fa))

## [2.7.3](https://github.com/informatievlaanderen/public-api/compare/v2.7.2...v2.7.3) (2019-04-18)


### Bug Fixes

* remove aws root tag from trace id generation ([c1e2988](https://github.com/informatievlaanderen/public-api/commit/c1e2988))

## [2.7.2](https://github.com/informatievlaanderen/public-api/compare/v2.7.1...v2.7.2) (2019-04-18)


### Bug Fixes

* traceid has to be numeric ([c489d52](https://github.com/informatievlaanderen/public-api/commit/c489d52))

## [2.7.1](https://github.com/informatievlaanderen/public-api/compare/v2.7.0...v2.7.1) (2019-04-18)


### Bug Fixes

* properly register datadog with autofac ([4f1d68a](https://github.com/informatievlaanderen/public-api/commit/4f1d68a))

# [2.7.0](https://github.com/informatievlaanderen/public-api/compare/v2.6.0...v2.7.0) (2019-04-18)


### Features

* plug in traceid from amazon, hopefully ([f05a068](https://github.com/informatievlaanderen/public-api/commit/f05a068))

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
