
var container = document.getElementById('container');
var cMain = document.getElementById('cMain');
var btnFullScreen = document.getElementById('btnFullScreen');
var headerMain = document.getElementById('headerMain');
var ctx = cMain.getContext('2d');
var dTabs = [];

btnFullScreen.style.position = 'fixed';
btnFullScreen.style.top = '10px';
btnFullScreen.style.left = ((Number(window.innerWidth) / 2) - 15).toString() + 'px';

var DominoTab = {
    points: [{ x: 0, y: 0 }, { x: 0, y: 88 }, { x: 48, y: 88 }, { x: 48, y: 0 }],
    message: "DominoTab",
    color: 'white',
    volt: false,
    values: [2, 6]
}

dTabs.push(DominoTab);

function fullScreen() {
    container.width = window.innerWidth;
    container.height = window.innerHeight;
    container.style.margin = '0';

    cMain.width = window.innerWidth;
    cMain.height = window.innerHeight;
    cMain.style.margin = '0';
    cMain.style.backgroundSize = 'cover';
    cMain.style.backgroundRepeat = 'no-repeat';

    btnFullScreen.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-fullscreen-exit" viewBox="0 0 16 16"><path d = "M5.5 0a.5.5 0 0 1 .5.5v4A1.5 1.5 0 0 1 4.5 6h-4a.5.5 0 0 1 0-1h4a.5.5 0 0 0 .5-.5v-4a.5.5 0 0 1 .5-.5zm5 0a.5.5 0 0 1 .5.5v4a.5.5 0 0 0 .5.5h4a.5.5 0 0 1 0 1h-4A1.5 1.5 0 0 1 10 4.5v-4a.5.5 0 0 1 .5-.5zM0 10.5a.5.5 0 0 1 .5-.5h4A1.5 1.5 0 0 1 6 11.5v4a.5.5 0 0 1-1 0v-4a.5.5 0 0 0-.5-.5h-4a.5.5 0 0 1-.5-.5zm10 1a1.5 1.5 0 0 1 1.5-1.5h4a.5.5 0 0 1 0 1h-4a.5.5 0 0 0-.5.5v4a.5.5 0 0 1-1 0v-4z" /></svg > ';
    
    headerMain.style.display = 'none';

    btnFullScreen.setAttribute('onclick', 'noFullScreen()');
    draw(dTabs);
}

function noFullScreen() {

    container.width = '1100';
    container.height = '600';
    container.style.margin = '5vh auto';

    cMain.width = '1100';
    cMain.height = '600';
    cMain.style.margin = '0';
    cMain.style.backgroundSize = 'cover';
    cMain.style.backgroundRepeat = 'no-repeat';

    

    btnFullScreen.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-fullscreen" viewBox="0 0 16 16"><path d = "M1.5 1a.5.5 0 0 0-.5.5v4a.5.5 0 0 1-1 0v-4A1.5 1.5 0 0 1 1.5 0h4a.5.5 0 0 1 0 1h-4zM10 .5a.5.5 0 0 1 .5-.5h4A1.5 1.5 0 0 1 16 1.5v4a.5.5 0 0 1-1 0v-4a.5.5 0 0 0-.5-.5h-4a.5.5 0 0 1-.5-.5zM.5 10a.5.5 0 0 1 .5.5v4a.5.5 0 0 0 .5.5h4a.5.5 0 0 1 0 1h-4A1.5 1.5 0 0 1 0 14.5v-4a.5.5 0 0 1 .5-.5zm15 0a.5.5 0 0 1 .5.5v4a1.5 1.5 0 0 1-1.5 1.5h-4a.5.5 0 0 1 0-1h4a.5.5 0 0 0 .5-.5v-4a.5.5 0 0 1 .5-.5z" /></svg >';
    btnFullScreen.style.position = 'fixed';
    // .replace(/px$/, '')
    
    headerMain.style.display = 'block';

    btnFullScreen.setAttribute('onclick', 'fullScreen()');
    draw(dTabs);
}

function define(tab) {            
    var points = tab.points;
    ctx.beginPath();
    ctx.moveTo(points[0].x, points[0].y);
    for (var i = 1; i < points.length; i++) {
        ctx.lineTo(points[i].x, points[i].y);
    }
}

function draw(tabs) {
    for (var i = 0; i < tabs.length; i++) {
        var tab = tabs[i];
        define(tab);
        ctx.fillStyle = tab.color;
        ctx.fill();
        ctx.stroke();
    }
}

function handleMouseDown(e) {
    e.preventDefault();

    var $canvas = $("#cMain");
    var canvasOffset = $canvas.offset();
    var offsetX = canvasOffset.left;
    var offsetY = canvasOffset.top;

    // get the mouse position
    var mouseX = parseInt(e.clientX - offsetX);
    var mouseY = parseInt(e.clientY - offsetY);

    // iterate each shape in the shapes array
    for (var i = 0; i < dTabs.length; i++) {
        var tab = dTabs[i];
        // define the current shape
        define(tab);
        // test if the mouse is in the current shape
        if (ctx.isPointInPath(mouseX, mouseY)) {
            // if inside, display the shape's message
            alert(tab.message);
        }
    }

}

draw(dTabs);

// listen for mousedown events
$("#cMain").mousedown(function (e) { handleMouseDown(e); });
