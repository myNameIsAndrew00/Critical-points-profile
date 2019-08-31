/*use this variables for dots controll*/ 

var isMouseDown = false;
var dotsCount = 1;
var currentDot = 1;

/*the limits of the slider*/
var LeftMargin = 40 + Number(window.getComputedStyle(document.getElementById('color-picker-container'), null).marginLeft.replace(/[a-zA-Z]/g, ""));
var SliderWidth = 800;
var SliderLeft = 30;

/*color mapping. this variable is loaded when the script is loaded first time (see cshtml code) */
var colorList = null;

function changePosition() { 
    if(currentDot == 1) return;
    if(isMouseDown){
        var dotPosition = event.clientX - LeftMargin;
        var dot = document.getElementById('dot-' + currentDot);
        var dotLabel = document.getElementById('dot-' + currentDot + '-label');
        var margins = colorList.GetPointMargins('dot-' + currentDot);
     
        if(dotPosition < margins.left  || dotPosition > margins.right ) return;

        //change the label content
        dotLabel.innerText = colorList.GetPercentage(dotPosition).toFixed(2) + '%';
        dotLabel.style.left = dotPosition + 'px';

        //change the point
        dot.style.left = dotPosition + SliderLeft + 'px';
       
        colorList.SetPointPosition('dot-' + currentDot, dotPosition);       
        $('#slider').css({ background: colorList.BuildGradientString() }); 
        showColorPicker(event.clientX - 10);
    }  
}


function changeButtonState(state){ 
    isMouseDown = state;

}


function changeSelectedDot(){
    var id = event.srcElement.id; 
    if(id.includes('dot-')) currentDot = id.split('-')[1];
    
    showColorPicker(event.clientX);
}

/*function which handles the points addition to the slider*/  
function addDot() { 
    
    var dotColor = '#'+(Math.random()*0xFFFFFF<<0).toString(16);
    var dotPosition = event.clientX - LeftMargin;     
 
    showColorPicker(event.clientX);
    var spanID = createSpan(dotPosition + SliderLeft, dotColor);
    createLabel(dotPosition);

    colorList.AddNode( dotPosition, dotColor, spanID);
     
    $('#slider').css({ background: colorList.BuildGradientString() });
 
}

function createLabel(dotPosition){
    var label = document.createElement('label');
    label.innerText = colorList.GetPercentage(dotPosition).toFixed(2) + '%';
    label.style.left = dotPosition + 'px';
    label.id = 'dot-' + dotsCount + '-label';

    $('#dots-container').append(label);
}

function createSpan(dotPosition, dotColor){
    dotsCount++;
    //set the current dot to be the newest one
    currentDot = dotsCount;
    
    var dot = document.createElement('span');
   
    dot.classList.add('dot');
    dot.id = 'dot-' + dotsCount;
    dot.style.left = dotPosition +'px';   
    dot.style.backgroundColor = dotColor;
    dot.draggable = false;
    dot.addEventListener('mousedown',changeSelectedDot);
 
    $('#dots-container').append(dot);

    return dot.id;
}

function changeSpanColor(newColor){
    colorList.SetPointColor('dot-' + currentDot, newColor);
    document.getElementById('dot-' + currentDot).style.backgroundColor = newColor;
    
    $('#slider').css({ background: colorList.BuildGradientString() });
     
}

function showColorPicker(horizontalPosition) { 
   changeColorPickerVisibility(true);

   $('#color-picker').css('left', horizontalPosition - 25 + 'px');
}

function changeColorPickerVisibility(isVisible){
    isVisible ? $('#color-picker').removeClass('color-picker-hidden') :  $('#color-picker').addClass('color-picker-hidden');
}

function enableSubmit() {    
    $('#finish-info-card').children('#send-palette-button').prop('disabled',
        $('#name-info-card').children('#color-palette-name').val() === '');
}

function sendColorPalette() {
    var paletteName = $('#name-info-card').children('#color-palette-name').val();

    if (paletteName === '') return; //TODO: display overlay

    $.ajax({
        type: "POST",
        data: {
            Intervals: colorList.GetColorMap(),
            Name: paletteName,
        },
        url: 'api/settings/SaveColorsPalette',
        success: function (receivedInfo) {
            $('#settings-layer-container').children('#settings-layer-overlay').removeClass('message-overlay-hidden');
            $('#settings-layer-container').children('#settings-layer-overlay').html(receivedInfo);
        },
        error: function (receivedInfo) {
            $('#settings-layer-container').children('#settings-layer-overlay').removeClass('message-overlay-hidden');
            $('#settings-layer-container').children('#settings-layer-overlay').html(receivedInfo);
        }
    });;
}

function hideOverlay() {
    $('#settings-layer-container').children('#settings-layer-overlay').addClass('message-overlay-hidden');
    $('#settings-layer-container').children('#settings-layer-overlay').empty();
}