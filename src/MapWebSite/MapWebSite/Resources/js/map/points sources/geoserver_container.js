﻿/*!
 * Component: Points source manager
 * This script contains code which manages the source for the points layer
 * */
import { DisplayPointInfo, SetPointInfoData } from '../../point info/point_info.js';
import { SelectedDataset } from '../../points settings/chose_dataset.js';
import { ColorPalette } from '../../home.js';
import { Router, endpoints } from '../../api/api_router.js';

export class GeoserverPointsSectionsContainer {

    constructor(map) {
        this.map = map; 
        this.pointsLayer = null;
    }

    LoadPoints() {
        
    }

    UpdatePointsLayer() {
        this.RemoveLayers();

        var self = this;

        Router.Get(endpoints.PointsSettingsApi.ValidateGeoserverStyle,
            {
                datasetName: SelectedDataset.name,
                datasetUsername: SelectedDataset.user,
                paletteName: ColorPalette.name,
                paletteUsername: ColorPalette.user
            },
            function (response) {
                self.initPointslayer();
                self.InitialiseMapInteraction();
            });
         
    }

    InitialiseMapInteraction() {
        var self = this;

        this.map.on('click', function (evt) {
            if (self.pointsLayer == null) return;

            //adapt properties received from Geoserver to a format accepted by ' point_info '
            function adaptProperties(geoserverFeature) {
                var result = {};
                result.Number = geoserverFeature.Number;
                result.ReferenceX = geoserverFeature.ReferenceX;
                result.ReferenceY = geoserverFeature.ReferenceY;
                result.Longitude = geoserverFeature.Longitude;
                result.Latitude = geoserverFeature.Latitude;
                result.Height = geoserverFeature.Height;
                result.DeformationRate = geoserverFeature.Deformati;
                result.StandardDeviation = geoserverFeature.StandardDe;
                result.EstimatedHeight = geoserverFeature.EstimatedH;
                result.EstimatedDeformationRate = geoserverFeature.EstimatedD;
                result.Displacements = [];

                const displacementsCount = Object.keys(geoserverFeature).length - 10;

                for (var index = 0; index < displacementsCount; index++)
                    result.Displacements[index] = {
                        DaysFromReference: index,
                        Value: geoserverFeature['D_' + index]
                    };

                return result;
            }

            var view = self.map.getView();
            var viewResolution = view.getResolution();
            var source = self.pointsLayer.getSource();
            var url = source.getFeatureInfoUrl(
                evt.coordinate, viewResolution, view.getProjection(),
                {
                    INFO_FORMAT: 'application/json',
                    FEATURE_COUNT: 1
                });
            if (url) {
                //make a request to that url to receive data
                $.ajax({
                    type: 'GET',           
                    crossDomain: true, 
                    url: url,
                    success: function (jsondata) {                       
                        var pointInfo = adaptProperties(jsondata.features[0].properties);

                        SetPointInfoData(pointInfo);

                        DisplayPointInfo();                        
                    }
                })
               

            }
        });
    }

    RemoveLayers() {
        if(this.pointsLayer != null)
            this.map.removeLayer(this.pointsLayer);

        this.pointsLayer = null;
    }

    /*private region below*/

    initPointslayer() {
        this.pointsLayer = new ol.layer.Tile({
            visible: true,
            source: new ol.source.TileWMS({
                url: 'http://localhost:8080/geoserver/wms',
                params: {
                    'FORMAT': 'image/png',
                    'VERSION': '1.1.1',
                    tiled: true,
                    "LAYERS": SelectedDataset.name,
                    "exceptions": 'application/vnd.ogc.se_inimage',
                    tilesOrigin: 26.744917 + "," + 43.1027705,
                    'STYLES': ColorPalette.user + '_' + ColorPalette.name
                }
            })
        });

        this.map.addLayer(this.pointsLayer);
    }
}