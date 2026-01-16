namespace Common.FeatureToggles
{
    public sealed class ChangeRoadSegmentAttributesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentChangeAttributes";

        public ChangeRoadSegmentAttributesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeRoadSegmentDynamicAttributesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentChangeDynamicAttributes";

        public ChangeRoadSegmentDynamicAttributesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class ChangeRoadSegmentOutlineGeometryToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentChangeOutlineGeometry";

        public ChangeRoadSegmentOutlineGeometryToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class CreateRoadSegmentOutlineToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentCreateOutline";

        public CreateRoadSegmentOutlineToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DeleteRoadSegmentOutlineToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentDeleteOutline";

        public DeleteRoadSegmentOutlineToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class DeleteRoadSegmentsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentDeleteSegments";

        public DeleteRoadSegmentsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class LinkRoadSegmentStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentLinkStreetName";

        public LinkRoadSegmentStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class UnlinkRoadSegmentStreetNameToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentUnlinkStreetName";

        public UnlinkRoadSegmentStreetNameToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class GetRoadSegmentToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadSegmentGet";

        public GetRoadSegmentToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class GetRoadOrganizationsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadGetOrganizations";

        public GetRoadOrganizationsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadJobsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadJobs";

        public RoadJobsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadDownloadGetForEditorToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadDownloadGetForEditor";

        public RoadDownloadGetForEditorToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadDownloadGetForProductToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadDownloadGetForProduct";

        public RoadDownloadGetForProductToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractCreateJobToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractCreateJob";

        public RoadExtractCreateJobToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractDownloadRequestsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractDownloadRequests";

        public RoadExtractDownloadRequestsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractDownloadRequestsByContourToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractDownloadRequestsByContour";

        public RoadExtractDownloadRequestsByContourToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractDownloadRequestsByFileToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractDownloadRequestsByFile";

        public RoadExtractDownloadRequestsByFileToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractDownloadRequestsByNisCodeToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractDownloadRequestsByNisCode";

        public RoadExtractDownloadRequestsByNisCodeToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractGetDownloadToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractGetDownload";

        public RoadExtractGetDownloadToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
    public sealed class RoadExtractGetUploadToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractGetUpload";

        public RoadExtractGetUploadToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
    public sealed class RoadExtractGetDetailsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractGetDetails";

        public RoadExtractGetDetailsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractGetOverlappingTransactionZonesGeoJsonToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractGetOverlappingTransactionZonesGeoJson";

        public RoadExtractGetOverlappingTransactionZonesGeoJsonToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractGetStatusToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractGetStatus";

        public RoadExtractGetStatusToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadExtractGetTransactionZonesGeoJsonToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadExtractGetTransactionZonesGeoJson";

        public RoadExtractGetTransactionZonesGeoJsonToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadListExtractsToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadListExtracts";

        public RoadListExtractsToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadOverlappingExtractsByContourToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadOverlappingExtractsByContour";

        public RoadOverlappingExtractsByContourToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadOverlappingExtractsByNisCodeToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadOverlappingExtractsByNisCode";

        public RoadOverlappingExtractsByNisCodeToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
    public sealed class RoadUploadExtractToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadUploadExtract";

        public RoadUploadExtractToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
    public sealed class RoadCloseExtractToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadCloseExtract";

        public RoadCloseExtractToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadGrbExtractByContourToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadGrbExtractByContour";

        public RoadGrbExtractByContourToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadGrbUploadForDownloadToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadGrbUploadForDownload";

        public RoadGrbUploadForDownloadToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadInwinningRequestExtractToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadInwinningRequestExtract";

        public RoadInwinningRequestExtractToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadInwinningUploadExtractToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadInwinningUploadExtract";

        public RoadInwinningUploadExtractToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadInwinningListToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadInwinningList";

        public RoadInwinningListToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }

    public sealed class RoadInwinningNisCodesToggle : KeyedFeatureToggleBase, IKeyedFeatureToggle
    {
        public override string Key => "RoadInwinningNisCodes";

        public RoadInwinningNisCodesToggle(IDynamicFeatureToggleService? dynamicFeatureToggleService)
            : base(dynamicFeatureToggleService)
        { }
    }
}
