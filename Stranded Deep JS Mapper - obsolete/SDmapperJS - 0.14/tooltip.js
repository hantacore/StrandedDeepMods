// Stranded deep Mapper by Hantacore - v0.10
var dots = [];

// show tooltip when mouse hovers over dot
function handleMapMouseMove(e) {
	
	let canvasdiv = document.getElementById("canvasdiv"); 
	
	let tipCanvas = document.getElementById("tooltipCanvas");
	let tipCtx = tipCanvas.getContext("2d");

	
	let mouseposX = parseInt(e.clientX - canvasdiv.offsetLeft);
	let mouseposY = parseInt(e.clientY - canvasdiv.offsetTop);

	let unscaledrelativemouseX = parseInt(mouseposX - marginLeft)
	let unscaledrelativemouseY = parseInt(mouseposY - marginTop);	
	
	let relativemouseX = unscaledrelativemouseX / zoomLevel;
	let relativemouseY = unscaledrelativemouseY / zoomLevel;	
	
	// debug
	if (debug) {
		tipCanvas.style.left = mouseposX + "px";
		tipCanvas.style.top = (mouseposY + 20) + "px";
		tipCtx.clearRect(0, 0, tipCanvas.width, tipCanvas.height);
		tipCtx.fillStyle = "#88ff88";
		tipCtx.rect(0, 0, 250, 20);
		tipCtx.fill();
		tipCtx.fillStyle = "#ff0000";
		tipCtx.fillText(mouseposX + " / " + mouseposY + " // " + relativemouseX + " / " + relativemouseY, 5, 15);
	} else {
		tipCtx.clearRect(0, 0, tipCanvas.width, tipCanvas.height);
	}

	// Put your mousemove stuff here
	var hit = false;
	for (var i = 0; i < dots.length; i++) {
	  var dot = dots[i];
	  var dx = relativemouseX - dot.x;
	  var dy = relativemouseY - dot.y;
	  if (dx * dx + dy * dy < dot.rXr) {
			tipCanvas.style.left = mouseposX + "px";
			tipCanvas.style.top = (mouseposY + 20) + "px";
		  
			tipCtx.fillStyle = "#88ff88";
			tipCtx.rect(0, 0, 250, 20);
			tipCtx.fill();
		  
			tipCtx.fillStyle = '#cccccc';
			tipCtx.fill();
			tipCtx.fillStyle = "#000000";
		  
			tipCtx.fillText(dot.tip, 5, 15);
			hit = true;
	  }
	}
}

function clearToolTips() {
	dots = [];
}

function addTooltip(objectx, objecty, tooltipValue) {
	dots.push({
            x: objectx,
            y: objecty,
            r: 4,
            rXr: 16,
            tip: tooltipValue
        });
}