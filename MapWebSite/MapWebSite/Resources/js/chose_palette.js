﻿var __selected_palette_index = -1;

async function useColorMap(paletteIndex, username, paletteName) {
    function changeSelectedRowOnMenu(id, visible){
        var paletteRow = $('#points-settings-layer-container-content').find(id)[0];
        if (paletteRow === undefined) return;
        visible ? paletteRow.classList.add('selected-row') : paletteRow.classList.remove('selected-row');
    }

    await $.get("/api/PointsSettingsApi/GetColorPalette", { username: username, paletteName: paletteName }, await function (palette) {
        colorPalette = palette;
        updatePointsLayer();

        var previousPaletteId = '#user_palette_index_' + __selected_palette_index;
        var paletteId = '#user_palette_index_' + paletteIndex;

        changeSelectedRowOnMenu(previousPaletteId, false);
        changeSelectedRowOnMenu(paletteId, true);

        __selected_palette_index = paletteIndex;
    });

}