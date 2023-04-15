// Stranded deep Mapper by Hantacore - v0.10
var debug = false;
var dragging = false;
var lastX;
var lastY;
var marginLeft = 0;
var marginTop = 0;

var zonesY = 5;
var zonesX = 5;

var width = 3000;
var height = 3000;

var offsetX = 1500;
var offsetY = 1500

var zoneXwidth = 0;
var zoneYwidth = 0;

var input, file, fr;
var newArr;

var startScale = 1;
var scale = startScale;
var zoomLevel = 1;

var drawAnimals = false;
var drawWreckages = false;
var drawMineables = false;
var drawItems = false;
var drawSavepoints = true;
var drawRaftMaterials = false;
var drawFruits = false;
var drawMedicine = false;
var drawZoneNames = false;
var drawBuildings = false;

var withTextures = false;

var sandpattern = null;
var shorepattern = null;
var lagoonpattern = null;

function updateFlags() {
	let chk0 = document.getElementById("checkItems");
	drawItems = chk0.checked;

	let chk1 = document.getElementById("checkAnimals");
	drawAnimals = chk1.checked;
	
	let chk2 = document.getElementById("checkWreckages");
	drawWreckages = chk2.checked;
	
	let chk3 = document.getElementById("checkMineables");
	drawMineables = chk3.checked;
	
	let chk4 = document.getElementById("checkSavePoints");
	drawSavepoints = chk4.checked;

	let chk5 = document.getElementById("checkRaftMaterials");
	drawRaftMaterials = chk5.checked;
	
	let chk6 = document.getElementById("checkTextures");
	withTextures = chk6.checked;

	let chk7 = document.getElementById("checkFruits");
	drawFruits = chk7.checked;
	
	let chk8 = document.getElementById("checkMedicine");
	drawMedicine = chk8.checked;
	
	let chk9 = document.getElementById("checkZoneNames");
	drawZoneNames = chk9.checked;

	let chk10 = document.getElementById("checkBuildings");
	drawBuildings = chk10.checked;
	
	let mainDiv = document.getElementById("canvasdiv");
	var body = document.getElementsByTagName('body')[0];
	if (withTextures) {
		mainDiv.style.backgroundImage = "url('icons/tex/water.jpg')";
		body.style.backgroundImage = "url(icons/tex/paper.jpg)";
	}
	else {
		mainDiv.style.backgroundImage = "";
		body.style.backgroundImage = "";
	}
	
	// tooltip
	clearToolTips();
	
	drawMap(true, false);
}

function resetAll(){
	let c = document.getElementById("mapCanvas");
	
	let ctx = c.getContext("2d");
	ctx.restore();	
	
	marginLeft = 0;
	marginTop = 0;
	c.style.marginLeft = "0px";
	c.style.marginTop = "0px";
		
	drawMap(true, false);
}

function initViewPort() {
	let c = document.getElementById("mapCanvas");
	
	let ctx = c.getContext("2d");
	ctx.save();	
	
	// tooltip
	let tc = document.getElementById("tooltipCanvas");
	tctx = tc.getContext("2d");
	
	// test
	let imageSand = new Image();
		imageSand.onload = function () {
		sandpattern = ctx.createPattern(imageSand, "repeat");
	};
	imageSand.src = './icons/tex/sand.jpg';
	
	let imageShore = new Image();
		imageShore.onload = function () {
		shorepattern = ctx.createPattern(imageShore, "repeat");
	};
	imageShore.src = './icons/tex/sand.jpg';
	
	let imageLagoon = new Image();
	imageLagoon.onload = function () {
		lagoonpattern = ctx.createPattern(imageLagoon, "repeat");
	};
	imageLagoon.src = './icons/tex/lagoon1.jpg';
	
	c.addEventListener('mousedown', function(e) {
		let evt = e || event;
		dragging = true;
		lastX = evt.clientX;
		lastY = evt.clientY;
		e.preventDefault();
	}, false);
	
	c.addEventListener('mousewheel',function(e){
		
		let evt = e || event;
		
		if (typeof(newArr) == "undefined"
		|| !newArr.hasOwnProperty("Version")) {
			return;
		}
		
		let delta = e.wheelDelta;

		let ctx = c.getContext("2d");
		ctx.clearRect(0, 0, c.width, c.height);
		
		if (delta > 0 && scale < 2)
		{
			scale = 1.1;
			ctx.scale(scale, scale);
			zoomLevel = 1.1 * zoomLevel;
		}
		else if (scale > 0.2) {
			scale = 0.9;
			ctx.scale(scale, scale);
			zoomLevel = 0.9 * zoomLevel;
		}
		
		let zoomTextBox = document.getElementById("txtZoomLevel");
		zoomTextBox.value = zoomLevel;
		
		marginLeft = scale * marginLeft;
		marginTop = scale * marginTop;
		c.style.marginLeft = marginLeft + "px";
		c.style.marginTop = marginTop + "px";
		
		drawMap(true, false);
		
		return false; 
	}, false);

	window.addEventListener('mousemove', function(e) {
		let evt = e || event;
		if (dragging) {
			let deltaX = evt.clientX - lastX;
			let deltaY = evt.clientY - lastY;
			lastX = evt.clientX;
			lastY = evt.clientY;
			marginLeft += deltaX;
			marginTop += deltaY;
			c.style.marginLeft = marginLeft + "px";
			c.style.marginTop = marginTop + "px";
		}
		// tooltip
		// request mousemove events
		handleMapMouseMove(e);
		e.preventDefault();
	}, false);

	window.addEventListener('mouseup', function() {
		dragging = false;
	}, false);
	
	let zoomTextBox = document.getElementById("txtZoomLevel");
	zoomTextBox.value = zoomLevel;
}

function loadFile() {
  
    if (typeof window.FileReader !== 'function') {
      alert("The file API isn't supported on this browser yet.");
      return;
    }

    input = document.getElementById('fileinput');
    if (!input) {
      alert("Um, couldn't find the fileinput element.");
    }
    else if (!input.files) {
      alert("This browser doesn't seem to support the `files` property of file inputs.");
    }
    else if (!input.files[0]) {
      alert("Please select a file before clicking 'Load'");
    }
    else {
      file = input.files[0];
      fr = new FileReader();
      fr.onload = receivedText;
      fr.readAsText(file);
    }

    function receivedText(e) {
		lines = e.target.result;
		newArr = JSON.parse(lines); 
		drawMap(true, false);
    }
}

function clearAll() {
	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");
	
	ctx.clearRect(0, 0, c.width, c.height);
	
	ctx.globalAlpha=0.98;
	
	ctx.restore();
	
	// ghost silhouette bug
	ctx.beginPath();
	
	// the water blue
	if (withTextures) {
		ctx.fillStyle = lagoonpattern
		ctx.fillRect(0, 0, width, height);
	}
	else{
		ctx.fillStyle = "#ccffff";
		ctx.fillStyle = "rgba(204, 255, 255, 0.7)";
		ctx.fillRect(0, 0, width, height);
	}
}

function drawGrid() {
	let c = document.getElementById("mapCanvas");

	zoneXwidth = width / zonesX;
	zoneYwidth = height / zonesY;
	
	let ctx = c.getContext("2d");
	
	ctx.lineWidth = 1;
	ctx.lineCap = "butt";
	ctx.lineJoin = "miter";
	ctx.strokeStyle = 'black';
	ctx.stroke();
	ctx.fillStyle = 'white';
	ctx.fill();
	
	for (y = 1; y < zonesY; y++) { 
		ctx.beginPath();
		ctx.moveTo(0, y * zoneYwidth);
		ctx.lineTo(width, y * zoneYwidth);
		ctx.closePath();
		ctx.stroke();
	}

	for (x = 1; x < zonesX; x++) {
		ctx.beginPath();
		ctx.moveTo(x * zoneXwidth, 0);
		ctx.lineTo(x * zoneXwidth, height);
		ctx.closePath();
		ctx.stroke();
	}
}

function drawMap(onFileSelected, focusOnPlayer) {
	if (onFileSelected === undefined) {
		onFileSelected = false;
    }
	
	if (focusOnPlayer === undefined) {
		focusOnPlayer = false;
    }
	
	clearAll();

	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");	
	
	if (withTextures) {
		ctx.fillStyle = lagoonpattern
	}
	else {
		ctx.fillStyle = "#ccffff";
		ctx.fillStyle = "rgba(204, 255, 255, 0.7)";
	}
	ctx.fillRect(0, 0, width, height);	
	
	drawGrid();
	
	if (onFileSelected) {
		if (typeof(newArr) == "undefined"
			|| !newArr.hasOwnProperty("Version")) {
			alert("No save file loaded !");
			return;
		}
		drawMapElements(focusOnPlayer);
	}
}


function drawMapElements(focusOnPlayer) {
	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");
	
	// transposition matrix
	var zones = [];
	zones[0] = 4;
	zones[1] = 9;
	zones[2] = 14;
	zones[3] = 19;
	zones[4] = 24;
	zones[5] = 3;
	zones[6] = 8;
	zones[7] = 13;
	zones[8] = 18;
	zones[9] = 23;
	zones[10] = 2;
	zones[11] = 7;
	zones[12] = 12;
	zones[13] = 17;
	zones[14] = 22;
	zones[15] = 1;
	zones[16] = 6;
	zones[17] = 11;
	zones[18] = 16;
	zones[19] = 21;
	zones[20] = 0;
	zones[21] = 5;
	zones[22] = 10;
	zones[23] = 15;
	zones[24] = 20;
	
	for (x = 0; x < zonesX; x++) {
		for (y = 0; y < zonesY; y++) {
			let zoneIndex = zones[x + y * 5];
			let currentZone = newArr.Persistent.StrandedWorld.Zones["[" + zoneIndex + "]Zone"];
							
			// debug : zone label
			if (drawZoneNames) {
				ctx.fillStyle = '#ccffff';
				ctx.fillRect(x * zoneXwidth + 5, y * zoneYwidth + 6,200,20);
				ctx.fillStyle = 'black';
				ctx.font = "10px Arial";
				ctx.fillText(currentZone.Name, x * zoneXwidth + 10, y * zoneYwidth + 20);	
				ctx.fillStyle = 'black';
			}
			
			if (!currentZone.hasOwnProperty("Discovered") || (currentZone.hasOwnProperty("Discovered") && currentZone["Discovered"] == false)){
				let calcx = x * zoneXwidth + 0.5 * zoneXwidth;
				let calcy = y * zoneYwidth + 0.5 * zoneYwidth;
				
				let imageObj = new Image();
				imageObj.src = './icons/unknown.png';
				imageObj.onload = function () {
					ctx.drawImage(imageObj, calcx - 25, calcy - 25, 50, 50);
				};
			} 
			else {	
				// draw wet sand test
				ctx.fillStyle = '#ff0000';
				ctx.beginPath();
				let start = true;
				$.each(currentZone.Objects, function(name, currentvalue) {
					
					let currentItemY = currentvalue["Transform"]["localPosition"]["y"];
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// key not contained in the dictionary, debug to help add it and exit
					if (!allDictionary.hasOwnProperty(key)) {
						if (debug) {
							alert("Key " + key + " not found in any dictionary, add it for mapping completion");
						}
						console.log(currentvalue["name"]);
						return;
					}
					
					// island silhouette
					if ((allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("ROCK") || String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("WRECK"))
						) && currentItemY <= -0.1 && currentItemY >= -0.8)
					{						
						let currentItemX = currentvalue["Transform"]["localPosition"]["x"];
						let currentItemZ = currentvalue["Transform"]["localPosition"]["z"];
					
						// calculated position
						let calcx = x * zoneXwidth + 0.5 * zoneXwidth + Number(currentItemX) * (width / 1000);
						let calcy = y * zoneYwidth + 0.5 * zoneYwidth - Number(currentItemZ) * (height / 1000);
					
						if (start) {
							ctx.moveTo(calcx, calcy);
							start = false;
						}
						else {
							ctx.lineTo(calcx, calcy);
						}
						
						ctx.arc(calcx, calcy, 0, 0, 2 * Math.PI, false);
					}
				});
				
				ctx.closePath();
				
				ctx.lineWidth = 25;
				ctx.lineCap = "round";
				ctx.lineJoin = "round";
				if (withTextures) {
					ctx.strokeStyle = shorepattern;
				}
				else {
					ctx.strokeStyle = '#c2b280';
				}
				ctx.stroke();
				if (withTextures){
					ctx.fillStyle = sandpattern;
				}
				else {
					ctx.fillStyle = '#c2b280';
				}
				ctx.fill();
			
				// end test
			
				// draw island silhouette
				ctx.fillStyle = '#c2b280';
				ctx.beginPath();
				//let start = true;
				$.each(currentZone.Objects, function(name, currentvalue) {
					
					let currentItemY = currentvalue["Transform"]["localPosition"]["y"];
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// key not contained in the dictionary, debug to help add it and exit
					if (!allDictionary.hasOwnProperty(key)) {
						if (debug) {
							alert("Key " + key + " not found in any dictionary, add it for mapping completion");
						}
						console.log(currentvalue["name"]);
						return;
					}
					
					// island silhouette
					if ((allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("ROCK") || String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("WRECK"))
						) && currentItemY >= -0.1)
					{						
						let currentItemX = currentvalue["Transform"]["localPosition"]["x"];
						let currentItemZ = currentvalue["Transform"]["localPosition"]["z"];
					
						// calculated position
						let calcx = x * zoneXwidth + 0.5 * zoneXwidth + Number(currentItemX) * (width / 1000);
						let calcy = y * zoneYwidth + 0.5 * zoneYwidth - Number(currentItemZ) * (height / 1000);
					
						if (start) {
							ctx.moveTo(calcx, calcy);
							start = false;
						}
						else {
							ctx.lineTo(calcx, calcy);
						}
						
						ctx.arc(calcx, calcy, 5, 0, 2 * Math.PI, false);
					}
				});
				
				ctx.closePath();

				ctx.lineWidth = 25;
				ctx.lineCap = "round";
				ctx.lineJoin = "round";
				if (withTextures) {
					ctx.strokeStyle = shorepattern;
				}
				else {
					ctx.strokeStyle = '#ede0b8';
				}
				ctx.stroke();
				if (withTextures){
					ctx.fillStyle = sandpattern;
				}
				else {
					ctx.fillStyle = '#ede0b8';
				}
				ctx.fill();

				// draw things
				$.each(currentZone.Objects, function(name, currentvalue) {
					
					let currentItemX = currentvalue["Transform"]["localPosition"]["x"];
					let currentItemY = currentvalue["Transform"]["localPosition"]["y"];
					let currentItemZ = currentvalue["Transform"]["localPosition"]["z"];
					
					// calculated position
					let calcx = x * zoneXwidth + 0.5 * zoneXwidth + Number(currentItemX) * (width / 1000);
					let calcy =  y * zoneYwidth + 0.5 * zoneYwidth - Number(currentItemZ) * (height / 1000);
					
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// draw rocks
					if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("ROCK"))
						)						{				
						let imageObj = new Image();
						imageObj.style.zindex = 1000;
						imageObj.src = './icons/rock.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
						};
					}
					
					// draw trees
					if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("PALM"))
						)
					{
						let imageObj = new Image();
						imageObj.style.zindex = 2000;
						if (key.indexOf("PALM") !== -1) {
							imageObj.src = './icons/palmtree.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
						}
						else {
							imageObj.src = './icons/plant.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 8, calcy - 8, 16, 16);
							};
						}
					}
					
					// draw wreckages
					if (drawWreckages) {
						if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("WRECK"))
						)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 3000;
							if (key.indexOf("WRECK") !== -1) {
								imageObj.src = './icons/shipwreck.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 18, calcy - 18, 36, 36);
								};
							}
						}
					}
					
					// draw animals
					if (drawAnimals) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SHARK"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 4000;
							if (key.indexOf("SHARK") !== -1) {
								imageObj.src = './icons/shark.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("STINGRAY"))
							)
						{
							let imageObj = new Image();
							if (key.indexOf("RAY") !== -1) {
								imageObj.src = './icons/stingray.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 8, calcy - 8, 16, 16);
								};
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("WHALE"))
							)
						{
							let imageObj = new Image();
							if (key.indexOf("WHALE") !== -1) {
								imageObj.src = './icons/whale.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 8, calcy - 8, 16, 16);
								};
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("MARLIN"))
							)
						{
							let imageObj = new Image();
							if (key.indexOf("MARLIN") !== -1) {
								imageObj.src = './icons/marlin.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 8, calcy - 8, 16, 16);
								};
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("BOAR"))
							)
						{	
							let imageObj = new Image();						
							if (key.indexOf("BOAR") !== -1) {
								imageObj.src = './icons/boar.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
						}
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SNAKE"))
							)
						{
							let imageObj = new Image();
							if (key.indexOf("SNAKE") !== -1) {
								imageObj.src = './icons/snake.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
						}
					}
					
					// draw minable resources
					if (drawMineables) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("RESOURCE")))
						{
							if (key.indexOf("MINING") !== -1 || key.indexOf("PILE") !== -1) {
								let imageObj = new Image();
								imageObj.style.zindex = 5000;
								imageObj.src = './icons/resource.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
						}
					}
					
					if (drawItems) {
						if (allDictionary.hasOwnProperty(key) 
								&& String(allDictionary[key]).includes("DRAW")
								&& (String(allDictionary[key]).includes("TOOL"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 5000;
							imageObj.src = './icons/tool.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 10, calcy - 10, 20, 20);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					
						if (allDictionary.hasOwnProperty(key) 
								&& String(allDictionary[key]).includes("DRAW")
								&& (String(allDictionary[key]).includes("ITEM"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 5000;
							imageObj.src = './icons/item.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 6, calcy - 6, 12, 12);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					}
					
					if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("RAFT"))
						)
					{
						let imageObj = new Image();
						imageObj.style.zindex = 5000;
						imageObj.src = './icons/raft.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 10, calcy - 10, 20, 20);
						};
					}
					
					// draw saving points
					if (drawSavepoints) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SAVE"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/save.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
						}
					}
					
					if (drawRaftMaterials)
					{
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("BARREL"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/barrel.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("BUOY"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/buoy.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("TIRE"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/tire.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					}
					
					// draw fruits
					if (drawFruits) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("FRUIT"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/fruit.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("COCONUT"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/coconut.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 6, calcy - 6, 12, 12);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("POTATO"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/potato.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("YUCCA")))
						{
							let imageObj = new Image();
							imageObj.style.zindex = 5000;
							imageObj.src = './icons/yucca.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					}
					
					// draw flowers
					if (drawMedicine) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("FLOWER"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/flower.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					}
					
					// draw buildings
					if (drawBuildings) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("STRUCTURE"))
							)
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/foundation.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
							
							// contructions list
							$.each(currentvalue["Constructions"], function(name, currentConstruction) {
								// loop on references
								$.each(currentZone.Objects, function(name, currentconstructiontolookfor) {
									// shitty inner loop to get the sub elements
									if (currentConstruction == currentconstructiontolookfor["reference"])
									{
										let currentSubItemX = currentconstructiontolookfor["Transform"]["localPosition"]["x"];
										let currentSubItemZ = currentconstructiontolookfor["Transform"]["localPosition"]["z"];
										
										let subcalcx = calcx + Number(currentSubItemX);
										let subcalcy = calcy + Number(currentSubItemZ);
										
										let imageSubObj = new Image();
										imageSubObj.style.zindex = 6000;
										imageSubObj.src = './icons/foundation.png';
										imageSubObj.onload = function () {
											ctx.drawImage(imageSubObj, subcalcx - 12, subcalcy - 12, 24, 24);
										};
									}
								});
							});
						}
					}
				});
			}
			
			// draw player position
			if (newArr.Persistent.StrandedWorld.PlayerZoneId.localeCompare(currentZone.Id) == 0)
			{
				// calculated position
				let calcx = x * zoneXwidth + 0.5 * zoneXwidth + (Number(newArr.Persistent.StrandedWorld.PlayerPosition["x"]) - (x - 2) * 630) * (width / 1000); // local position conversion from center of map, zone is 630 wide in game units
				let calcy = y * zoneYwidth + 0.5 * zoneYwidth - (Number(newArr.Persistent.StrandedWorld.PlayerPosition["z"]) + (y - 2) * 630) * (height / 1000);  // local position conversion from center of map, zone is 630 high in game units
				
				let imageObj = new Image();
				imageObj.style.zindex = 7000;
				imageObj.src = './icons/player.png';
				imageObj.onload = function () {
					ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
				};
				
				// tooltip
				addTooltip(calcx, calcy, "Yeah, that\'s you");
				
				if (focusOnPlayer) {
					let viewport = document.getElementById("canvasdiv");
					marginLeft = viewport.clientWidth/2 - zoomLevel * calcx;
					marginTop = viewport.clientHeight/2 - zoomLevel * calcy;
					c.style.marginLeft = marginLeft + "px";
					c.style.marginTop = marginTop + "px";
				}
			}
		}
	}
}

var allDictionary = {};

function initDictionaries() {
	// keys = DRAW|ROCK|PALM|TREE|SHARK|MARLIN|WHALE|STINGRAY|BOARD|SNAKE|WRECK|TOOL|MINABLE|FORT|SAVE|RESOURCE|RAFT|RAFTMAT|TIRE|BUOY|BARREL|HELI|TOOL
	// All for 0.39
	allDictionary["BAT"] = "BAT";
    allDictionary["SEAGULL"] = "SEAGULL";
    allDictionary["BOAR"] = "DRAW|BOAR";
    allDictionary["BOAR_RAGDOLL"] = "DRAW|BOAR";
    allDictionary["CRAB"] = "CRAB";
    allDictionary["ARCHER"] = "FISH";
    allDictionary["CLOWN_TRIGGERFISH"] = "FISH";
    allDictionary["COD"] = "FISH";
    allDictionary["DISCUS"] = "FISH";
    allDictionary["LIONFISH"] = "FISH";
    allDictionary["PILCHARD"] = "FISH";
    allDictionary["SARDINE"] = "FISH";
    allDictionary["STING_RAY"] = "DRAW|STINGRAY";
    allDictionary["STING_RAY_RAGDOLL"] = "DRAW|STINGRAY";
    allDictionary["MARLIN"] = "DRAW|MARLIN";
    allDictionary["SHARK_WHITE"] = "DRAW|SHARK";
    allDictionary["SHARK_REEF"] = "DRAW|SHARK";
    allDictionary["SHARK_TIGER"] = "DRAW|SHARK";
    allDictionary["WHALE"] = "DRAW|WHALE";
    allDictionary["MARLIN_RAGDOLL"] = "DRAW|MARLIN";
    allDictionary["SHARK_GREAT WHITE_RAGDOLL"] = "DRAW|SHARK";
    allDictionary["SHARK_REEF_RAGDOLL"] = "DRAW|SHARK";
    allDictionary["SHARK_TIGER_SHARK_RAGDOLL"] = "DRAW|SHARK";
    allDictionary["PATROL_GREATWHITE"] = "DRAW|SHARK";
    allDictionary["PATROL_MARLIN"] = "DRAW|MARLIN";
    allDictionary["PATROL_REEFSHARK"] = "DRAW|SHARK";
    allDictionary["PATROL_TIGERSHARK"] = "DRAW|SHARK";
    allDictionary["HIDESPOT_SNAKE"] = "DRAW|SNAKE";
    allDictionary["SNAKE"] = "DRAW|SNAKE";
    allDictionary["SNAKE_RAGDOLL"] = "DRAW|SNAKE";
    allDictionary["BARREL"] = "DRAW|BARREL";
    allDictionary["BARREL_PILE"] = "DRAW|BARREL";
    allDictionary["BRICK_ARCH"] = "";
    allDictionary["BRICK_FLOOR"] = "";
    allDictionary["BRICK_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["BRICK_PANEL_STATIC"] = "";
    allDictionary["BRICK_ROOF_CAP"] = "";
    allDictionary["BRICK_ROOF_CORNER"] = "";
    allDictionary["BRICK_ROOF_MIDDLE"] = "";
    allDictionary["BRICK_ROOF_WEDGE"] = "";
    allDictionary["BRICK_WALL_HALF"] = "";
    allDictionary["BRICK_WALL_WINDOW"] = "";
    allDictionary["BRICK_WEDGE_FLOOR"] = "";
    allDictionary["BRICK_WEDGE_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["BRICKS"] = "DRAW|RESOURCE";
    allDictionary["BUOYBALL"] = "DRAW|BUOY";
    allDictionary["BUOYBALL_PILE"] = "DRAW|BUOY";
    allDictionary["CORRUGATED_ARCH"] = "";
    allDictionary["CORRUGATED_DOOR"] = "";
    allDictionary["CORRUGATED_FLOOR"] = "";
    allDictionary["CORRUGATED_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["CORRUGATED_PANEL_STATIC"] = "";
    allDictionary["CORRUGATED_STEPS"] = "";
    allDictionary["CORRUGATED_WALL_HALF"] = "";
    allDictionary["CORRUGATED_WALL_WINDOW"] = "";
    allDictionary["CORRUGATED_WEDGE_FLOOR"] = "";
    allDictionary["CORRUGATED_WEDGE_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["DRIFTWOOD_ARCH"] = "";
    allDictionary["DRIFTWOOD_DOOR"] = "";
    allDictionary["DRIFTWOOD_FLOOR"] = "";
    allDictionary["DRIFTWOOD_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["DRIFTWOOD_PANEL_STATIC"] = "";
    allDictionary["DRIFTWOOD_STEPS"] = "";
    allDictionary["DRIFTWOOD_WALL_HALF"] = "";
    allDictionary["DRIFTWOOD_WALL_WINDOW"] = "";
    allDictionary["DRIFTWOOD_WEDGE_FLOOR"] = "";
    allDictionary["DRIFTWOOD_WEDGE_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["GYRO_BASE_1"] = "GYRO";
    allDictionary["GYRO_COCKPIT_4"] = "GYRO";
    allDictionary["GYRO_MOTOR_3"] = "GYRO";
    allDictionary["GYRO_ROTORS_5"] = "GYRO";
    allDictionary["GYRO_SEAT_2"] = "GYRO";
    allDictionary["GYRO_STRUCTURE"] = "GYRO";
    allDictionary["RAFT_OUTRIGGER"] = "";
    allDictionary["WOOD_CANOE"] = "";
    allDictionary["WOOD_RAFT"] = "";
    allDictionary["PLANK_ARCH"] = "";
    allDictionary["PLANK_DOOR"] = "";
    allDictionary["PLANK_FLOOR"] = "";
    allDictionary["PLANK_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["PLANK_PANEL_STATIC"] = "";
    allDictionary["PLANK_STEPS"] = "";
    allDictionary["PLANK_WALL_HALF"] = "";
    allDictionary["PLANK_WALL_WINDOW"] = "";
    allDictionary["PLANK_WEDGE_FLOOR"] = "";
    allDictionary["PLANK_WEDGE_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["RAFT_BASE_BALLS"] = "";
    allDictionary["RAFT_BASE_BARREL"] = "";
    allDictionary["RAFT_BASE_TYRE"] = "";
    allDictionary["RAFT_BASE_WOOD_BUNDLE"] = "";
    allDictionary["RAFT_FLOOR_CORRUGATED"] = "";
    allDictionary["RAFT_FLOOR_DRIFTWOOD"] = "";
    allDictionary["RAFT_FLOOR_PLANK"] = "";
    allDictionary["RAFT_FLOOR_STEEL"] = "";
    allDictionary["RAFT_FLOOR_WOOD"] = "";
    allDictionary["SCRAP_CORRUGATED"] = "DRAW|RESOURCE";
    allDictionary["SCRAP_PLANK"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_1"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_2"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_3"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_DOOR"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_DOOR_STATIC"] = "";
    allDictionary["SHIPPING_CONTAINER_FLOOR"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["SHIPPING_CONTAINER_PANEL"] = "DRAW|RESOURCE";
    allDictionary["SHIPPING_CONTAINER_PANEL_STATIC"] = "";
    allDictionary["SHIPPING_CONTAINER_STEPS"] = "";
    allDictionary["SHIPPING_CONTAINER_WEDGE_FLOOR"] = "";
    allDictionary["SHIPPING_CONTAINER_WEDGE_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["STEEL_DOOR"] = "";
    allDictionary["STEEL_STEPS"] = "";
    allDictionary["STRUCTURE"] = "";
    allDictionary["STRUCTURE_RAFT"] = "DRAW|RAFT";
    allDictionary["STRUCTURE_SMALL"] = "";
    allDictionary["TARP_PANEL"] = "DRAW|RESOURCE";
    allDictionary["TARP_PANEL_STATIC"] = "";
    allDictionary["TYRE"] = "DRAW|TIRE";
    allDictionary["TYRE_PILE"] = "DRAW|TIRE";
    allDictionary["VEHICLE_HELICOPTER"] = "DRAW|HELI";
    allDictionary["VEHICLE_LIFERAFT"] = "DRAW|RAFT";
    allDictionary["VEHICLE_MOTOR"] = "";
    allDictionary["VEHICLE_SAIL"] = "";
    allDictionary["WOOD_ARCH"] = "";
    allDictionary["WOOD_DOOR"] = "";
    allDictionary["WOOD_FLOOR"] = "";
    allDictionary["WOOD_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["WOOD_PANEL_STATIC"] = "";
    allDictionary["WOOD_ROOF_CAP"] = "";
    allDictionary["WOOD_ROOF_CORNER"] = "";
    allDictionary["WOOD_ROOF_MIDDLE"] = "";
    allDictionary["WOOD_ROOF_WEDGE"] = "";
    allDictionary["WOOD_STEPS"] = "";
    allDictionary["WOOD_WALL_HALF"] = "";
    allDictionary["WOOD_WALL_WINDOW"] = "";
    allDictionary["WOOD_WEDGE_FLOOR"] = "";
    allDictionary["WOOD_WEDGE_FOUNDATION"] = "DRAW|BUILDING";
    allDictionary["CONTAINER_CONSOLE"] = "";
    allDictionary["CONTAINER_CRATE"] = "";
    allDictionary["CONTAINER_LOCKER_LARGE"] = "";
    allDictionary["CONTAINER_LOCKER_SMALL"] = "";
    allDictionary["BED"] = "DRAW|SAVE";
    allDictionary["BED_SHELTER"] = "DRAW|SAVE";
    allDictionary["BRICK_STATION"] = "";
    allDictionary["CLAY"] = "DRAW|ITEM";
    allDictionary["CLOTH"] = "DRAW|ITEM";
    allDictionary["DRIFTWOOD_STICK"] = "DRAW|ITEM";
    allDictionary["ENGINE"] = "DRAW|ITEM";
    allDictionary["ENGINE_FUEL_TANK"] = "DRAW|ITEM";
    allDictionary["ENGINE_PROPELLER"] = "DRAW|ITEM";
    allDictionary["ENGINE_PUMP"] = "DRAW|ITEM";
    allDictionary["FIRE_TORCH"] = "DRAW|ITEM";
    allDictionary["FURNACE"] = "";
    allDictionary["HOBO_STOVE"] = "";
    allDictionary["KINDLING"] = "DRAW|ITEM";
    allDictionary["LEATHER"] = "DRAW|ITEM";
    allDictionary["LEAVES_FIBROUS"] = "DRAW|ITEM";
    allDictionary["LOOM"] = "";
    allDictionary["NEW_CAMPFIRE"] = "";
    allDictionary["NEW_CAMPFIRE_PIT"] = "";
    allDictionary["NEW_CAMPFIRE_SPIT"] = "";
    allDictionary["PALM_FROND"] = "DRAW|ITEM";
    allDictionary["PLANK_STATION"] = "";
    allDictionary["RAIN_CATCHER"] = "";
    allDictionary["RAWHIDE"] = "DRAW|ITEM";
    allDictionary["ROCK"] = "DRAW|ITEM";
    allDictionary["ROPE_COIL"] = "DRAW|ITEM";
    allDictionary["SMOKER"] = "";
    allDictionary["STICK"] = "DRAW|ITEM";
    allDictionary["STONE_TOOL"] = "DRAW|TOOL";
    allDictionary["TANNING_RACK"] = "";
    allDictionary["WATER_STILL"] = "";
    allDictionary["EGG_DEADEX"] = "DRAW|ITEM";
    allDictionary["EGG_WOLLIE"] = "DRAW|ITEM";	
    allDictionary["FARMING_HOE"] = "DRAW|TOOL";
    allDictionary["FARMING_PLOT_CORRUGATED"] = "";
    allDictionary["FARMING_PLOT_PLANK"] = "";
    allDictionary["FARMING_PLOT_WOOD"] = "";
    allDictionary["KURA_FRUIT"] = "DRAW|FRUIT";
    allDictionary["KURA_TREE"] = "DRAW|FRUIT";
    allDictionary["QUWAWA_FRUIT"] = "DRAW|FRUIT";
    allDictionary["QUWAWA_TREE"] = "DRAW|FRUIT";
    allDictionary["CAN_BEANS"] = "DRAW|ITEM";
    allDictionary["CAN_BEANS_OPEN"] = "DRAW|ITEM";
    allDictionary["COCONUT_DRINKABLE"] = "DRAW|ITEM";
    allDictionary["COCONUT_HALF"] = "DRAW|ITEM";
    allDictionary["COCONUT_ORANGE"] = "DRAW|COCONUT";
    allDictionary["MEAT_LARGE"] = "DRAW|ITEM";
    allDictionary["MEAT_MEDIUM"] = "DRAW|ITEM";
    allDictionary["MEAT_SMALL"] = "DRAW|ITEM";
    allDictionary["POTATO"] = "DRAW|POTATO";
    allDictionary["WATER_BOTTLE"] = "DRAW|ITEM";
    allDictionary["WATER_SKIN"] = "DRAW|ITEM";
    allDictionary["CORRUGATED_SHELF"] = "";
    allDictionary["CORRUGATED_TABLE"] = "";
    allDictionary["PLANK_CHAIR"] = "";
    allDictionary["PLANK_SHELF"] = "";
    allDictionary["PLANK_TABLE"] = "";
    allDictionary["WOOD_CHAIR"] = "";
    allDictionary["WOOD_HOOK"] = "";
    allDictionary["WOOD_SHELF"] = "";
    allDictionary["WOOD_TABLE"] = "";
    allDictionary["ANTIBIOTICS"] = "DRAW|ITEM";
    allDictionary["BANDAGE"] = "DRAW|ITEM";
    allDictionary["MEDICAL_ALOE_SALVE"] = "DRAW|ITEM";
    allDictionary["MORPHINE"] = "DRAW|ITEM";
    allDictionary["NEW_COCONUT_FLASK"] = "DRAW|ITEM";
    allDictionary["NEW_COCONUT_MEDICAL"] = "DRAW|ITEM";
    allDictionary["VITAMINS"] = "DRAW|ITEM";
    allDictionary["MISSION_EEL"] = "";
    allDictionary["MISSION_MARKER"] = "";
    allDictionary["MISSION_SHARK"] = "";
    allDictionary["MISSION_SQUID"] = "";
    allDictionary["MISSION_TROPHY_EEL"] = "";
    allDictionary["MISSION_TROPHY_SHARK"] = "";
    allDictionary["MISSION_TROPHY_SQUID"] = "";
    allDictionary["MINING_ROCK"] = "DRAW|RESOURCE";
    allDictionary["MINING_ROCK_CLAY"] = "DRAW|RESOURCE";
    allDictionary["ALOCASIA_1"] = "DRAW|TREE";
    allDictionary["ALOCASIA_2"] = "DRAW|TREE";
    allDictionary["BANANA_PLANT"] = "DRAW|TREE";	
    allDictionary["BIGROCK_1"] = "DRAW|ROCK";
    allDictionary["BIGROCK_2"] = "DRAW|ROCK";
    allDictionary["BIGROCK_3"] = "DRAW|ROCK";
    allDictionary["BIGROCK_4"] = "DRAW|ROCK";
    allDictionary["BIGROCK_5"] = "DRAW|ROCK";
    allDictionary["CERIMAN_1"] = "DRAW|TREE";
    allDictionary["CERIMAN_2"] = "DRAW|TREE";
    allDictionary["CERIMAN_3"] = "DRAW|TREE";
    allDictionary["CLIFF_001"] = "DRAW|ROCK";
    allDictionary["CLIFF_002"] = "DRAW|ROCK";
    allDictionary["CLIFF_003"] = "DRAW|ROCK";
    allDictionary["CLIFF_004"] = "DRAW|ROCK";
    allDictionary["CLIFF_005"] = "DRAW|ROCK";
    allDictionary["CLIFF_006"] = "DRAW|ROCK";	
    allDictionary["DRIFTWOOD_DECAL"] = "";
    allDictionary["OCEAN_BUOY"] = "";
    allDictionary["PHILODENDRON_1"] = "DRAW|TREE";
    allDictionary["PHILODENDRON_2"] = "DRAW|TREE";
    allDictionary["SHORELINE_ROCK_1"] = "DRAW|ROCK";
    allDictionary["SHORELINE_ROCK_2"] = "DRAW|ROCK";
    allDictionary["SMALLROCK_1"] = "DRAW|ROCK";
    allDictionary["SMALLROCK_2"] = "DRAW|ROCK";
    allDictionary["SMALLROCK_3"] = "DRAW|ROCK";
    allDictionary["SeaFort_1"] = "";
    allDictionary["SeaFort_2"] = "";
    allDictionary["SeaFort_Brige"] = "";
    allDictionary["SeaFort_Brige_Broken"] = "";
    allDictionary["DOOR"] = "";
    allDictionary["ROWBOAT_3"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_2A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_3A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_4A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_5A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_6A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_7A"] = "DRAW|WRECK";
    allDictionary["COMPASS"] = "DRAW|ITEM";
    allDictionary["DUCTTAPE"] = "DRAW|ITEM";
    allDictionary["FLARE_GUN"] = "DRAW|ITEM";
    allDictionary["FUELCAN"] = "DRAW|ITEM";
    allDictionary["LABEL_MAKER"] = "DRAW|ITEM";
    allDictionary["LANTERN"] = "DRAW|ITEM";
    allDictionary["MACHETTE"] = "DRAW|ITEM";
    allDictionary["NEW_AIRTANK"] = "DRAW|ITEM";
    allDictionary["NEW_ARROW"] = "DRAW|TOOL";
    allDictionary["NEW_CRUDE_AXE"] = "DRAW|TOOL";
    allDictionary["NEW_CRUDE_BOW"] = "DRAW|TOOL";
    allDictionary["NEW_CRUDE_HAMMER"] = "DRAW|TOOL";
    allDictionary["NEW_CRUDE_SPEAR"] = "DRAW|TOOL";
    allDictionary["NEW_FISHING_SPEAR"] = "DRAW|TOOL";
    allDictionary["NEW_REFINED_AXE"] = "DRAW|TOOL";
    allDictionary["NEW_REFINED_HAMMER"] = "DRAW|TOOL";
    allDictionary["NEW_REFINED_KNIFE"] = "DRAW|TOOL";
    allDictionary["NEW_REFINED_PICK"] = "DRAW|TOOL";
    allDictionary["NEW_REFINED_SPEAR"] = "DRAW|TOOL";
    allDictionary["NEW_SPEARGUN"] = "DRAW|TOOL";
    allDictionary["NEW_SPEARGUN_ARROW"] = "DRAW|TOOL";
    allDictionary["TORCH"] = "DRAW|TOOL";
    allDictionary["COCA_BUSH"] = "DRAW|TREE";
    allDictionary["DRIFTWOOD_PILE"] = "";
    allDictionary["FICUS_1"] = "DRAW|TREE";
    allDictionary["FICUS_2"] = "DRAW|TREE";
    allDictionary["FICUS_3"] = "DRAW|TREE";
    allDictionary["FICUS_TREE"] = "DRAW|TREE";
    allDictionary["FICUS_TREE_2"] = "DRAW|TREE";
    allDictionary["LOG_0"] = "DRAW|ITEM";
    allDictionary["LOG_1"] = "DRAW|ITEM";
    allDictionary["LOG_2"] = "DRAW|ITEM";
    allDictionary["PALM_1"] = "DRAW|PALM";
    allDictionary["PALM_2"] = "DRAW|PALM";
    allDictionary["PALM_3"] = "DRAW|PALM";
    allDictionary["PALM_4"] = "DRAW|PALM";
    allDictionary["PALM_LOG_1"] = "DRAW|ITEM";
    allDictionary["PALM_LOG_2"] = "DRAW|ITEM";
    allDictionary["PALM_LOG_3"] = "DRAW|ITEM";
    allDictionary["PALM_TOP"] = "DRAW|ITEM";
    allDictionary["PINE_1"] = "DRAW|TREE";
    allDictionary["PINE_2"] = "DRAW|TREE";
    allDictionary["PINE_3"] = "DRAW|TREE";
    allDictionary["PINE_SMALL_1"] = "DRAW|TREE";
    allDictionary["PINE_SMALL_2"] = "DRAW|TREE";
    allDictionary["PINE_SMALL_3"] = "DRAW|TREE";
    allDictionary["POTATO_PLANT"] = "DRAW|POTATO";
    allDictionary["YOUNG_PALM_1"] = "DRAW|RESOURCE";
    allDictionary["YOUNG_PALM_2"] = "DRAW|RESOURCE";
    allDictionary["YUCCA"] = "DRAW|YUCCA";
	allDictionary["ALOE_VERA_FRUIT"] = "DRAW|FLOWER";
    allDictionary["ALOE_VERA_PLANT"] = "DRAW|TREE";
    allDictionary["AJUGA_PLANT"] = "DRAW|TREE";
    allDictionary["AJUGA"] = "DRAW|FLOWER";
    allDictionary["WAVULAVULA_PLANT"] = "DRAW|TREE";
    allDictionary["WAVULAVULA"] = "DRAW|FLOWER";
    allDictionary["PIPI_PLANT"] = "DRAW|TREE";
    allDictionary["PIPI"] = "DRAW|FLOWER";
    allDictionary["COCONUT_FLASK"] = "DRAW|ITEM";
	
	allDictionary["PADDLE"] = "DRAW|TOOL";
	
	allDictionary["PART_ELECTRICAL"] = "DRAW|ITEM";
	allDictionary["PART_FILTER"] = "DRAW|ITEM";
	allDictionary["PART_GYRO"] = "DRAW|ITEM";
	allDictionary["PART_FUEL"] = "DRAW|ITEM";
	allDictionary["PART_ENGINE"] = "DRAW|ITEM";
	
	allDictionary["STRUCTURE"] = "DRAW|STRUCTURE";
	
	// probable bug, there are items without keys
	allDictionary[""] = "";
}