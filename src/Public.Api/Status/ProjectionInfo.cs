namespace Public.Api.Status
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ProjectionInfo
    {
        private readonly IEnumerable<RegistryProjectionInfo> _registryProjectionInfo =
            new List<RegistryProjectionInfo>
            {
                new RegistryProjectionInfo
                {
                    Name = "AddressRegistry",
                    Infos = new[]
                    {
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Extract.AddressExtract.AddressExtractProjection",
                            Name = "Extract - AddressExtractProjection",
                            Description = "Adres data voor testbestand."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Extract.AddressExtract.AddressLinkExtractProjection",
                            Name = "Extract - AddressLinkExtractProjection",
                            Description = "Adres-link data voor testbestand."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Extract.AddressCrabHouseNumberIdExtract.AddressCrabHouseNumberIdExtractProjection",
                            Name = "Extract - AddressCrabHouseNumberIdExtractProjection",
                            Description = "CRAB huisnummer data voor testbestand."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Extract.AddressCrabSubaddressIdExtract.AddressCrabSubaddressIdExtractProjection",
                            Name = "Extract - AddressCrabSubaddressIdExtractProjection",
                            Description = "CRAB subadres data voor testbestand."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.LastChangedList.LastChangedListProjections",
                            Name = "LastChangedList",
                            Description = "Markeert de adressen waarvan de cached data moet geupdate worden."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Legacy.AddressDetail.AddressDetailProjections",
                            Name = "Legacy - AddressDetail",
                            Description = "Adres detail data."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Legacy.AddressList.AddressListProjections",
                            Name = "Legacy - AddressList",
                            Description = "Adres lijst data."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Legacy.AddressSyndication.AddressSyndicationProjections",
                            Name = "Legacy - AddressSyndication",
                            Description = "Adres data voor de feed."
                        },
                        new Info
                        {
                            Key = "AddressRegistry.Projections.Legacy.CrabIdToPersistentLocalId.CrabIdToPersistentLocalIdProjections",
                            Name = "Legacy - CrabIdToPersistentLocalId",
                            Description = "Linking the CRAB-id to the GRAR-id"
                        }
                    }
                },
                new RegistryProjectionInfo
                {
                    Name = "BuildingRegistry",
                    Infos = new[]
                    {
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Extract.BuildingExtract.BuildingExtractProjections",
                            Name = "Extract - BuildingExtract",
                            Description = "Gebouw data voor testbestand."
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Extract.BuildingUnitExtract.BuildingUnitExtractProjections",
                            Name = "Extract - BuildingUnitExtract",
                            Description = "Gebouweenheid data voor testbestand."
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.LastChangedList.BuildingUnitProjections",
                            Name = "LastChangedList",
                            Description = "Markeert de gebouw en gebouweenheden waarvan de cached data moet geupdate worden."
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Legacy.BuildingDetail.BuildingDetailProjections",
                            Name = "Legacy - BuildingUnitDetail",
                            Description = "Gebouw detail data."
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Legacy.BuildingSyndication.BuildingSyndicationProjections",
                            Name = "Legacy - BuildingSyndication",
                            Description = "Gebouw en gebouweenheid data voor de feed."
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Legacy.BuildingUnitDetail.BuildingUnitDetailProjections",
                            Name = " Legacy - BuildingUnitDetail",
                            Description = "Gebouweenheid detail data."
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Legacy.BuildingPersistentIdCrabIdMapping.BuildingPersistenLocalIdCrabIdProjections",
                            Name = "Legacy - BuildingPersistentLocalIdCrabId",
                            Description = "Linking the CRAB-id to the GRAR-id"
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Wms.Building.BuildingProjections",
                            Name = "Wms - Building",
                            Description = "Gebouw data voor WMS"
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Wms.BuildingUnit.BuildingUnitProjections",
                            Name = "Wms - BuildingUnit",
                            Description = "Gebouweenheid data voor WMS"
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Wfs.Building.BuildingProjections",
                            Name = "Wfs - Building",
                            Description = "Gebouweenheid data voor WFS"
                        },
                        new Info
                        {
                            Key = "BuildingRegistry.Projections.Wfs.BuildingUnit.BuildingUnitProjections",
                            Name = "Wfs - BuildingUnit",
                            Description = "Gebouweenheid data voor WFS"
                        }
                    }
                },
                new RegistryProjectionInfo
                {
                    Name = "MunicipalityRegistry",
                    Infos = new[]
                    {
                        new Info
                        {
                            Key = "MunicipalityRegistry.Projections.Extract.MunicipalityExtract.MunicipalityExtractProjections",
                            Name = "Extract - MunicipalityExtract",
                            Description = "Straatnaam data voor testbestand."
                        },
                        new Info
                        {
                            Key = "MunicipalityRegistry.Projections.LastChangedList.LastChangedListProjections",
                            Name = "LastChangedList",
                            Description = "Markeert de gemeentes waarvan de cached data moet geupdate worden."
                        },
                        new Info
                        {
                            Key = "MunicipalityRegistry.Projections.Legacy.MunicipalityDetail.MunicipalityDetailProjections",
                            Name = "Legacy - MunicipalityDetail",
                            Description = "Gemeente detail data."
                        },
                        new Info
                        {
                            Key = "MunicipalityRegistry.Projections.Legacy.MunicipalityList.MunicipalityListProjections",
                            Name = "Legacy - MunicipalityList",
                            Description = "Gemeente lijst data."
                        },
                        new Info
                        {
                            Key = "MunicipalityRegistry.Projections.Legacy.MunicipalityName.MunicipalityNameProjections",
                            Name = "Legacy - MunicipalityName",
                            Description = "Gemeente naam data"
                        },
                        new Info
                        {
                            Key = "MunicipalityRegistry.Projections.Legacy.MunicipalitySyndication.MunicipalitySyndicationProjections",
                            Name = "Legacy - MunicipalitySyndication",
                            Description = "Gemeente data voor de feed."
                        }
                    }
                },
                new RegistryProjectionInfo
                {
                    Name = "ParcelRegistry",
                    Infos = new[]
                    {
                        new Info
                        {
                            Key = "ParcelRegistry.Projections.Extract.ParcelExtract.ParcelExtractProjections",
                            Name = "Extract - ParcelExtract",
                            Description = "Perceel data voor testbestand."
                        },
                        new Info
                        {
                            Key = "ParcelRegistry.Projections.LastChangedList.LastChangedListProjections",
                            Name = "LastChangedList",
                            Description = "Markeert de percelen waarvan de cached data moet geupdate worden."
                        },
                        new Info
                        {
                            Key = "ParcelRegistry.Projections.Legacy.ParcelDetail.ParcelDetailProjections",
                            Name = "Legacy - ParcelDetail",
                            Description = "Perceel detail data."
                        },
                        new Info
                        {
                            Key = "ParcelRegistry.Projections.Legacy.ParcelSyndication.ParcelSyndicationProjections",
                            Name = "Legacy - ParcelSyndication",
                            Description = "Perceel data voor de feed."
                        }
                    }
                },
                new RegistryProjectionInfo
                {
                    Name = "PostalRegistry",
                    Infos = new[]
                    {
                        new Info
                        {
                            Key = "PostalRegistry.Projections.Extract.PostalInformationExtract.PostalInformationExtractProjections",
                            Name = "Extract - PostalInformationExtract",
                            Description = "Postinfo data voor testbestand."
                        },
                        new Info
                        {
                            Key = "PostalRegistry.Projections.LastChangedList.LastChangedListProjections",
                            Name = "LastChangedList",
                            Description = "Markeert de postinfo waarvan de cached data moet geupdate worden."
                        },
                        new Info
                        {
                            Key = "PostalRegistry.Projections.Legacy.PostalInformation.PostalInformationProjections",
                            Name = "Legacy - PostalInformation",
                            Description = "Postinfo detail data."
                        },
                        new Info
                        {
                            Key = "PostalRegistry.Projections.Legacy.PostalInformationSyndication.PostalInformationSyndicationProjections",
                            Name = "Legacy - PostalInformationSyndication",
                            Description = "Postinfo data voor de feed."
                        }
                    }
                },
                new RegistryProjectionInfo
                {
                    Name = "StreetNameRegistry",
                    Infos = new[]
                    {
                        new Info
                        {
                            Key = "StreetNameRegistry.Projections.Extract.StreetNameExtract.StreetNameExtractProjections",
                            Name = "Extract - StreetNameExtract",
                            Description = "Straatnaam data voor testbestand."
                        },
                        new Info
                        {
                            Key = "StreetNameRegistry.Projections.LastChangedList.LastChangedProjections",
                            Name = "LastChangedList",
                            Description = "Markeert de straatnaam waarvan de cached data moet geupdate worden."
                        },
                        new Info
                        {
                            Key = "StreetNameRegistry.Projections.Legacy.StreetNameDetail.StreetNameDetailProjections",
                            Name = "Legacy - StreetNameDetail",
                            Description = "Straatnaam detail data."
                        },
                        new Info
                        {
                            Key = "StreetNameRegistry.Projections.Legacy.StreetNameList.StreetNameListProjections",
                            Name = "Legacy - StreetNameList",
                            Description = "Straatnaam lijst data."
                        },
                        new Info
                        {
                            Key = "StreetNameRegistry.Projections.Legacy.StreetNameName.StreetNameNameProjections",
                            Name = "Legacy - StreetNameName",
                            Description = "Straatnaam naam data (BOSA)."
                        },
                        new Info
                        {
                            Key = "StreetNameRegistry.Projections.Legacy.StreetNameSyndication.StreetNameSyndicationProjections",
                            Name = "Legacy - StreetNameSyndication",
                            Description = "Straatnaam data voor de feed."
                        }
                    }
                }
            };

        public IEnumerable<Info> For(string repository)
            => _registryProjectionInfo
                .SingleOrDefault(info => info.Name.Equals(repository, StringComparison.InvariantCultureIgnoreCase))
               ?.Infos ?? new List<Info>();

        private class RegistryProjectionInfo
        {
            public string Name { get; set; }
            public IEnumerable<Info> Infos { get; set; }
        }

        internal class Info
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
