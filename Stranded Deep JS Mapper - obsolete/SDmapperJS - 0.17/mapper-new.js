// Stranded deep Mapper by Hantacore - v0.17 / 0.72 big world compatibility
// trees icon bug, shelter label, missing items, mineable trees, better medicine and fruits lisibility
var debug = false;
var dragging = false;
var lastX;
var lastY;
var marginLeft = 0;
var marginTop = 0;

var zonesY = 5;
var zonesX = 5;

var internalworldsize = 3000;
var width = 10000;
var height = 10000;
var offsetX = 1000;
var offsetY = 1000;
var widthratio = (width - offsetX)/3000;
var heightratio = (height - offsetY)/3000;
var distancescalefactor = 2;

var zoneXwidth = 300;
var zoneYwidth = 300;

var input, file, fr;
var newArr;
var worldSeed = 0;

var startScale = 1;
var scale = startScale;
var zoomLevel = 1;

var drawUndiscoveredIslands = false;
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
var drawMissions = false;

var withTextures = false;

var sandpattern = null;
var shorepattern = null;
var lagoonpattern = null;

var debugConsole = null;

//############################################################################################
// performance test : preload images
let loading = new Array();

function initImage(path, zindex) {
	loading.push(path);
	let myImage = new Image();
	myImage.style.zindex = zindex;
	myImage.src = path;
	myImage.onload = function () {
		let remove = loading.indexOf(path);
		loading.splice(remove, 1);
	};
	return myImage;
}

// unknown
let imageUnknown = initImage('./icons/unknown.png', 1000);

// unknown debug
let imageUnknownDebug = initImage('./icons/unknown_debug.png', 1000);

// SAVE
let imageSavepoint = initImage('./icons/save.png', 100000);

// RAFT
let imageRaft = initImage('./icons/raft.png', 500000);

// ROCKS
let imageRock = initImage('./icons/rock.png', 10000);
let imageRockCliff = initImage('./icons/cliff.png', 10000);
let imageRockDraw = initImage('./icons/rock_draw.png', 10000);

// TREES
let imagePalm = initImage('./icons/palmtree.png', 20000);
let imagePalmDraw = initImage('./icons/palmtree_draw.png', 20000);
let imagePlant = initImage('./icons/plant.png', 20000);
let imagePlantMineable = initImage('./icons/plant_mineable.png', 20000);
let imagePlantDraw = initImage('./icons/plant_draw.png', 20000);

// WRECKS
let imagePlanewreck = initImage('./icons/planewreck.png', 30000);
let imageShipwreck = initImage('./icons/shipwreck.png', 30000);

// MINABLE RESOURCES
let imageMinable = initImage('./icons/resource.png', 50000);

// MISSIONS
let imageInterest = initImage('./icons/point-of-interest.png', 40000);
let imageMissionEel = initImage('./icons/mission_eel.png', 40000);
let imageMissionSquid = initImage('./icons/mission_squid.png', 40000);
let imageMissionShark = initImage('./icons/mission_shark.png', 40000);
let imageMissionCarrier = initImage('./icons/mission_carrier.png', 40000);

// PLAYER
let imagePlayer = initImage('./icons/player.png', 100000);

// ANIMALS
let imageShark = initImage('./icons/shark.png', 40000);
let imageSharkRagdoll = initImage('./icons/shark_ragdoll.png', 40000);

let imageCrabRagdoll = initImage('./icons/crab.png', 40000);
let imageCrabSpawner = initImage('./icons/crab_spawner.png', 40000);
let imageGiantCrabRagdoll = initImage('./icons/big_crab_ragdoll.png', 40000);
let imageGiantCrabSpawner = initImage('./icons/big_crab_spawner.png', 40000);

let imageHogRagdoll = initImage('./icons/hog_ragdoll.png', 40000);
let imageHogSpawner = initImage('./icons/hog_spawner.png', 40000);
let imageBoarRagdoll = initImage('./icons/boar.png', 40000);
let imageBoarSpawner = initImage('./icons/boar_spawner.png', 40000);

let imageSnakeRagdoll = initImage('./icons/snake_ragdoll.png', 40000);
let imageSnakeSpawner = initImage('./icons/snake_spawner.png', 40000);
let imageSnakeHidespot = initImage('./icons/snake_hide.png', 40000);

let imageStingray = initImage('./icons/stingray.png', 40000);
let imageWhale = initImage('./icons/whale.png', 40000);
let imageMarlin = initImage('./icons/marlin.png', 40000);

// FRUITS
let imageFruit = initImage('./icons/fruit.png', 60000);
let imageCoconut = initImage('./icons/coconut.png', 60000);
let imagePotato = initImage('./icons/potato.png', 60000);
let imageYucca = initImage('./icons/yucca.png', 60000);

// MEDICINE
let imageFlower = initImage('./icons/flower.png', 60000);

// BUILDINGS
let imageHut = initImage('./icons/hut.png', 60000);
let imageWater = initImage('./icons/water.png', 40000);
let imageFire = initImage('./icons/fire.png', 40000);
let imageIndustry = initImage('./icons/industry.png', 40000);
let imageFoundation = initImage('./icons/foundation.png', 60000);
let imageContainer = initImage('./icons/container.png', 60000);

// SEAFORTS
let imageSeafort = initImage('./icons/seafort.png', 60000);

// ITEMS
let imageTool = initImage('./icons/tool.png', 50000);
let imageItem = initImage('./icons/item.png', 50000);
let imageCrate = initImage('./icons/crate.png', 50000);

// RAFT MATERIALS
let imageBarrel = initImage('./icons/barrel.png', 60000);
let imageBuoy = initImage('./icons/buoy.png', 60000);
let imageTire = initImage('./icons/tire.png', 60000);


//###############################################################################################

function writeLog(logText)
{
	if (debugConsole == null)
		return;
	
	debugConsole.value += (logText + "\n");
}

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

	let chk11 = document.getElementById("checkDebug");
	debug = chk11.checked;
	if (debugConsole == null) 
	{
		debugConsole = document.getElementById("txtLog");
	}
	debugConsole.value = '';
	if (debug)
		debugConsole.style.visibility = "visible" ;
	else
		debugConsole.style.visibility = "hidden" ;
  
  	let chk12 = document.getElementById("checkMissions");
	drawMissions = chk12.checked;

  	let chk13 = document.getElementById("checkWorld");
	drawUndiscoveredIslands = chk13.checked;
	
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
	newArr = null;
	worldSeed = null;
	input.Value = "";
	
	let f = document.getElementById("jsonFile");
	f.reset();
	
	let c = document.getElementById("mapCanvas");
	
	let ctx = c.getContext("2d");
	ctx.restore();	
	
	marginLeft = 0;
	marginTop = 0;
	c.style.marginLeft = "0px";
	c.style.marginTop = "0px";
	
	clearAll();
	
	//drawMap(true, false);
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

function initViewPort() {
	let c = document.getElementById("mapCanvas");
	let dv = document.getElementById("canvasdiv");
	
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
	//imageSand.src = './icons/tex/sand.jpg';
	imageSand.src = './icons/tex/sand_draw.jpg';
	
	let imageShore = new Image();
		imageShore.onload = function () {
		shorepattern = ctx.createPattern(imageShore, "repeat");
	};
	//imageShore.src = './icons/tex/sand.jpg';
	imageShore.src = './icons/tex/shore_draw.jpg';
	
	let imageLagoon = new Image();
	imageLagoon.onload = function () {
		lagoonpattern = ctx.createPattern(imageLagoon, "repeat");
	};
	//imageLagoon.src = './icons/tex/lagoon1.jpg';
	imageLagoon.src = './icons/tex/water_draw.png';
	
	c.addEventListener('mousedown', function(e) {
		let evt = e || event;
		dragging = true;
		lastX = evt.clientX;
		lastY = evt.clientY;
		e.preventDefault();
	}, false);
	
	dv.addEventListener('mousedown', function(e) {
		let evt = e || event;
		dragging = true;
		lastX = evt.clientX;
		lastY = evt.clientY;
		e.preventDefault();
	}, false);

	// Firefox zoom bug
	$(window).on('wheel', function(event){
		
		if (typeof(newArr) == "undefined"
		|| !newArr.hasOwnProperty("Version")) {
			return;
		}
		
		let delta = -event.originalEvent.deltaY;

		let ctx = c.getContext("2d");
		ctx.clearRect(0, 0, c.width, c.height);
		
		if (delta > 0 && scale < 2 && zoomLevel < 1.7)
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
	});
	
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

function loadFile(reset) {
  
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
	  if (reset){
		fr = new FileReader();
		fr.onload = receivedText;
	  }
      fr.readAsText(file);
    }

    function receivedText(e) {
		lines = e.target.result;
		newArr = JSON.parse(lines); 
		drawMap(true, true);
    }
}

function drawGrid() {
	// grid became useless with big world random positioning
	return;
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
			writeLog("No save file loaded !");
			return;
		}
		drawMapElements(focusOnPlayer);
	}
}

function drawMapElements(focusOnPlayer) {
	var t0 = performance.now();
	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");
	
	// read world seed
	worldSeed = newArr.Persistent.StrandedWorld.WorldSeed;
	if (debug)
	{
		console.log("Seed : " + worldSeed);
	}

	// compute random islands positions
	// The new big world randomizer
	let myFastRandom = new FastRandom(worldSeed);
	let myZonePositionGenerator = new ZonePositionGenerator();
	let maxIslands = 49;
	let radius = 500;
	let num3 = radius * 1.25 * 7;
	let sampleRegionSize = new Vector2(num3, num3);
	let numSamplesBeforeRejection = 30;
	// get a position list
	let generatedZonePoints = myZonePositionGenerator.GeneratePoints(worldSeed, radius, sampleRegionSize, numSamplesBeforeRejection);
	if (debug)
	{
		var zoneindexdebug;
		for(zoneindexdebug = 0; zoneindexdebug < generatedZonePoints.length; zoneindexdebug++)
		{
			console.log(generatedZonePoints[zoneindexdebug].x + "$" + generatedZonePoints[zoneindexdebug].y);
		}
	}
		
	// map the islands to the positions
	let generationZonePositions = Array();
	// JS specific init loop
	var zoneindex;
	for(zoneindex = 0; zoneindex < maxIslands; zoneindex++)
	{
		generationZonePositions.push(new Vector2(0,0));
	}
	let randomlist = Array();
	let upperBound = generatedZonePoints.length;
	for (zoneindex = 0; zoneindex < maxIslands; zoneindex++)
	{
		let num4 = myFastRandom.Next(0, upperBound);
		while (randomlist.includes(num4))
		{
			num4 = myFastRandom.Next(0, upperBound);
		}
		if (zoneindex == 0)
		{
			num4 = 0;
		}
		randomlist.push(num4);
		generationZonePositions[zoneindex] = generatedZonePoints[num4];
		
		if (debug)
		{
			console.log(generationZonePositions[zoneindex].x + "$" + generationZonePositions[zoneindex].y);
		}
	}

	// end of big world randomizer
	
	// loop on zones
	let zoneIndex;
	for(zoneIndex = 0; zoneIndex < generationZonePositions.length; zoneIndex++)
	{
			// get computed island coordinates
			let x, y;
			x = (width/2) + generationZonePositions[zoneIndex].x * distancescalefactor;
			y = (height/2) - generationZonePositions[zoneIndex].y * distancescalefactor;
			
			let currentZone = newArr.Persistent.StrandedWorld.Zones[zoneIndex];
							
			// debug : zone label
			if (drawZoneNames) {
				
				let fontsize = 10 / zoomLevel;
				let blockheight = 20 / zoomLevel;
				
				if (withTextures) {
					ctx.fillStyle = '#d8bc9d';
				}
				else {
					ctx.fillStyle = '#ccffff';
				}
				ctx.fillRect(x - zoneXwidth + 5, y - zoneYwidth + 16 - 9/zoomLevel, 500 + 50/zoomLevel, blockheight);
				ctx.fillStyle = 'black';
				if (withTextures) {
					ctx.font = "bold "+fontsize+"px Lucida Handwriting";
				}
				else {
					ctx.font = "bold "+fontsize+"px Arial";	
				}
				
				let fullname = currentZone.Id;
				let parts = currentZone.Name.split(' ');
				if (parts.length == 3) {
					fullname = namesDictionary[parts[0]] + " " + namesDictionary[parts[1]]+ " " + namesDictionary[parts[2]];
				}
				
				//ctx.fillText(currentZone.Id + " (" + currentZone.Name + ")", x - zoneXwidth + 10, y - zoneYwidth + 20);	
				ctx.fillText(fullname, x - zoneXwidth + 10, y - zoneYwidth + 20);	
				//ctx.fillText(currentZone.Id, x - zoneXwidth + 10, y - zoneYwidth + 20);	
				ctx.fillStyle = 'black';
			}
			
			// ? icon on island position
			if (!currentZone.hasOwnProperty("Discovered") || (currentZone.hasOwnProperty("Discovered") && currentZone["Discovered"] == false)) {
				if(drawUndiscoveredIslands) {
					let calcx = x;
					let calcy = y;
					
					if (debug && currentZone.Id.includes("MISSION"))
					{				
						let size = 50 / zoomLevel;
						ctx.drawImage(imageUnknownDebug, calcx - size/2, calcy - size/2, size, size);
					}
					else
					{
						let size = 50 / zoomLevel;
						ctx.drawImage(imageUnknown, calcx - size/2, calcy - size/2, size, size);
					}
					
					if (drawZoneNames)
					{
						// tooltip
						addTooltip(calcx, calcy, currentZone.Id + " (" + currentZone.Name + ")");
					}
				}
			} 
			else {	
				// draw wet sand test
				ctx.fillStyle = '#ff0000';
				ctx.beginPath();
				let start = true;
				$.each(currentZone.Objects, function(name, currentvalue) {
					
					let currentItemY = Number(String(currentvalue["Transform"]["localPosition"]["y"]).replace(",","."));
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// key not contained in the dictionary, debug to help add it and exit
					if (!allDictionary.hasOwnProperty(key)) {
						if (debug) {
							//alert("Key " + key + " not found in any dictionary, add it for mapping completion");
							writeLog("Key " + key + " not found in any dictionary");
						}
						console.log(currentvalue["name"]);
						console.log("Key " + key + " not found in any dictionary, add it for mapping completion");
						return;
					}
					
					// island silhouette
					if ((allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("ROCK") || String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("PLANT") || String(allDictionary[key]).includes("WRECK"))
						//) && currentItemY <= -0.1 && currentItemY >= -0.8)
						) && currentItemY >= -0.8)
					{						
						let currentItemX = Number(String(currentvalue["Transform"]["localPosition"]["x"]).replace(",","."));
						let currentItemZ = Number(String(currentvalue["Transform"]["localPosition"]["z"]).replace(",","."));
					
						// calculated position
						let calcx = x + Number(currentItemX) * (width / 1000 / widthratio);
						let calcy = y - Number(currentItemZ) * (height / 1000 / heightratio)
					
						if (start) {
							ctx.moveTo(calcx, calcy);
							start = false;
						}
						else {
							ctx.lineTo(calcx, calcy);
						}
						
						ctx.arc(calcx, calcy, 0, 0, 2 * Math.PI, false);
					}
					else if (debug)
					{
						//console.log("Key " + key + " will not be used for island silhouette (wet sand)");
						//if (String(allDictionary[key]).includes("ROCK") || String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("WRECK"))
						//{
						//	console.log("Y : " + currentItemY + " / dictionary key = " + allDictionary[key]);
						//}
					}
				});
				
				ctx.closePath();
				
				if (withTextures) {
					ctx.lineWidth = 28;
					ctx.lineCap = "round";
					ctx.lineJoin = "round";
					ctx.strokeStyle = '#000000';
					ctx.stroke();
				}
				
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
					ctx.fillStyle = shorepattern;
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
					
					let currentItemY = Number(String(currentvalue["Transform"]["localPosition"]["y"]).replace(",","."));
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// key not contained in the dictionary, debug to help add it and exit
					if (!allDictionary.hasOwnProperty(key)) {
						if (debug) {
							//alert("Key " + key + " not found in any dictionary, add it for mapping completion");
							writeLog("Key " + key + " not found in any dictionary");
							
						}
						console.log(currentvalue["name"]);
						return;
					}
					
					// island silhouette
					if ((allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("ROCK") || String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("PLANT") || String(allDictionary[key]).includes("WRECK"))
						) && currentItemY >= -0.1)
					{						
						let currentItemX = Number(String(currentvalue["Transform"]["localPosition"]["x"]).replace(",","."));
						let currentItemZ = Number(String(currentvalue["Transform"]["localPosition"]["z"]).replace(",","."));
					
						// calculated position
						let calcx = x + Number(currentItemX) * (width / 1000 / widthratio);
						let calcy = y - Number(currentItemZ) * (height / 1000 / heightratio)
					
						if (start) {
							ctx.moveTo(calcx, calcy);
							start = false;
						}
						else {
							ctx.lineTo(calcx, calcy);
						}
						
						ctx.arc(calcx, calcy, 5, 0, 2 * Math.PI, false);
					}
					else if (debug)
					{
						//console.log("Key " + key + " will not be used for island silhouette (dry sand)");
						//if (String(allDictionary[key]).includes("ROCK") || String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("WRECK"))
						//{
						//	console.log("Y : " + currentItemY + " / dictionary key = " + allDictionary[key]);
						//}
						
					}
				});
				
				ctx.closePath();

				ctx.lineWidth = 25;
				ctx.lineCap = "round";
				ctx.lineJoin = "round";
				if (withTextures) {
					ctx.strokeStyle = sandpattern;
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
					
					let currentItemX = Number(String(currentvalue["Transform"]["localPosition"]["x"]).replace(",","."));
					let currentItemY = Number(String(currentvalue["Transform"]["localPosition"]["y"]).replace(",","."));
					let currentItemZ = Number(String(currentvalue["Transform"]["localPosition"]["z"]).replace(",","."));
					
					// calculated position
					let calcx = x + Number(currentItemX) * (width / 1000 / widthratio);
					let calcy = y - Number(currentItemZ) * (height / 1000 / heightratio);
					
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// draw rocks
					if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("ROCK"))
						) {				
						if (String(allDictionary[key]).includes("CLIFF")) {
							ctx.drawImage(imageRockCliff, calcx - 12, calcy - 12, 24, 24);
						}
						else {
							if (withTextures) {
								ctx.drawImage(imageRockDraw, calcx - 12, calcy - 12, 24, 24);
							}
							else {
								ctx.drawImage(imageRock, calcx - 12, calcy - 12, 24, 24);
							}
						}						
					}
					
					// draw trees
					if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("TREE") || String(allDictionary[key]).includes("PALM"))
						)
					{
						if (key.indexOf("PALM") !== -1) {
							if (withTextures) {
								ctx.drawImage(imagePalmDraw, calcx - 12, calcy - 12, 24, 24);
							}
							else {
								ctx.drawImage(imagePalm, calcx - 12, calcy - 12, 24, 24);
							}
						}
						else if ((key.indexOf("FICUS")  !== -1 || key.indexOf("PINE")  !== -1) && String(allDictionary[key]).includes("MINEABLE")) {
							if (withTextures) {
								ctx.drawImage(imagePlantDraw, calcx - 8, calcy - 8, 16, 16);
							} 
							else {
								ctx.drawImage(imagePlantMineable, calcx - 8, calcy - 8, 16, 16);
							}
						}
						else {
							if (withTextures) {
								ctx.drawImage(imagePlantDraw, calcx - 8, calcy - 8, 16, 16);
							} 
							else {
								ctx.drawImage(imagePlant, calcx - 8, calcy - 8, 16, 16);
							}
						}
					}
					
					// draw wreckages
					if (drawWreckages) {
						if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("WRECK"))
						)
						{
							if (key.indexOf("PLANEWRECK") !== -1) {
								ctx.drawImage(imagePlanewreck, calcx - 18, calcy - 18, 36, 36);
							}
							else
							{
								ctx.drawImage(imageShipwreck, calcx - 18, calcy - 18, 36, 36);
							}
						}
					}
					
					// draw missions
					if (allDictionary.hasOwnProperty(key) 
						&& String(allDictionary[key]).includes("DRAW")
						&& (String(allDictionary[key]).includes("MISSION"))
						)
					{					
						let size = 50 / zoomLevel;
						if (allDictionary[key].indexOf("EEL") !== -1) {
							if (drawMissions) {
								ctx.drawImage(imageMissionEel, calcx - size/2, calcy - size/2, size, size);
							}
							else {
								ctx.drawImage(imageInterest, calcx - size/2, calcy - size/2, size, size);					
							}
						}
						if (allDictionary[key].indexOf("SQUID") !== -1) {
							if (drawMissions) {
								ctx.drawImage(imageMissionSquid, calcx - size/2, calcy - size/2, size, size);
							}
							else {
								ctx.drawImage(imageInterest, calcx - size/2, calcy - size/2, size, size);					
							}
						}
						if (allDictionary[key].indexOf("MEG") !== -1) {
							if (drawMissions) {
								ctx.drawImage(imageMissionShark, calcx - size/2, calcy - size/2, size, size);
							}
							else {
								ctx.drawImage(imageInterest, calcx - size/2, calcy - size/2, size, size);					
							}
						}
						if (allDictionary[key].indexOf("CARRIER") !== -1) {
							if (drawMissions) {
								ctx.drawImage(imageMissionCarrier, calcx - size/2, calcy - size/2, size, size);
							}
							else {
								ctx.drawImage(imageInterest, calcx - size/2, calcy - size/2, size, size);					
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
							if (key.indexOf("RAGDOLL") !== -1) 
							{
								ctx.drawImage(imageSharkRagdoll, calcx - 12, calcy - 12, 24, 24);
							}
							else if (key.indexOf("SHARK") !== -1) {
								ctx.drawImage(imageShark, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key + " counter : " + currentvalue["respawnCounter"] + " / distance : " + currentvalue["spawnDistance"]);
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("STINGRAY"))
							)
						{
							if (key.indexOf("RAY") !== -1) {
								ctx.drawImage(imageStingray, calcx - 8, calcy - 8, 16, 16);
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("WHALE"))
							)
						{
							if (key.indexOf("WHALE") !== -1) {
								ctx.drawImage(imageWhale, calcx - 8, calcy - 8, 16, 16);
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("MARLIN"))
							)
						{
							if (key.indexOf("MARLIN") !== -1) {
								ctx.drawImage(imageMarlin, calcx - 8, calcy - 8, 16, 16);
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("CRAB"))
							)
						{
							// big crabs
							if (key.indexOf("GIANT_CRAB_RAGDOLL_RAGDOLL") !== -1) {
								ctx.drawImage(imageGiantCrabRagdoll, calcx - 12, calcy - 12, 24, 24);
							}
							else if (key.indexOf("GIANT_CRAB_SPAWNER_SPAWNER") !== -1) 
							{
								ctx.drawImage(imageGiantCrabSpawner, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
							
							// crabs					
							else if (key.indexOf("CRAB_RAGDOLL") !== -1) {
								ctx.drawImage(imageCrabRagdoll, calcx - 12, calcy - 12, 24, 24);
							}
							else if (key.indexOf("CRAB_SPAWNER") !== -1) 
							{
								ctx.drawImage(imageCrabSpawner, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("BOAR"))
							)
						{
							// hogs
							if (key.indexOf("HOG_RAGDOLL") !== -1) {
								ctx.drawImage(imageHogRagdoll, calcx - 12, calcy - 12, 24, 24);
							}
							else if (key.indexOf("HOG_SPAWNER") !== -1) 
							{
								ctx.drawImage(imageHogSpawner, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
							
							// boars					
							else if (key.indexOf("BOAR_RAGDOLL") !== -1) {
								ctx.drawImage(imageBoarRagdoll, calcx - 12, calcy - 12, 24, 24);
							}
							else if (key.indexOf("BOAR_SPAWNER") !== -1) 
							{
								ctx.drawImage(imageBoarSpawner, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SNAKE"))
							)
						{
							if (key.indexOf("SNAKE_RAGDOLL") !== -1) {
								ctx.drawImage(imageSnakeRagdoll, calcx - 12, calcy - 12, 24, 24);
							}
							else if (key.indexOf("SNAKE_SPAWNER") !== -1) {
								ctx.drawImage(imageSnakeSpawner, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
							else
							{
								ctx.drawImage(imageSnakeHidespot, calcx - 12, calcy - 12, 24, 24);
								
								// tooltip
								addTooltip(calcx, calcy, key);
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
								ctx.drawImage(imageMinable, calcx - 12, calcy - 12, 24, 24);
								
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
							ctx.drawImage(imageTool, calcx - 10, calcy - 10, 20, 20);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					
						if (allDictionary.hasOwnProperty(key) 
								&& String(allDictionary[key]).includes("DRAW")
								&& (String(allDictionary[key]).includes("ITEM"))
							)
						{
							ctx.drawImage(imageItem, calcx - 6, calcy - 6, 12, 12);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
								&& String(allDictionary[key]).includes("DRAW")
								&& (String(allDictionary[key]).includes("CRATE"))
							)
						{
							
							if (currentItemY >= 0
								|| drawWreckages)
							{							
								ctx.drawImage(imageCrate, calcx - 6, calcy - 6, 12, 12);
								
								// tooltip
								addTooltip(calcx, calcy, key);
							}
						}
					}
					
					if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("RAFT"))
						)
					{
						if (key.includes("STRUCTURE_RAFT") && currentvalue.hasOwnProperty("RigidBody")) {
							currentItemX = Number(String(currentvalue["RigidBody"]["position"]["x"]).replace(",","."));
							currentItemY = Number(String(currentvalue["RigidBody"]["position"]["y"]).replace(",","."));
							currentItemZ = Number(String(currentvalue["RigidBody"]["position"]["z"]).replace(",","."));
								
							// calculated position
							calcx = x + Number(currentItemX) * (width / 1000 / widthratio);
							calcy = y - Number(currentItemZ) * (height / 1000 / heightratio);
							
							let size = 24 / zoomLevel;
							ctx.drawImage(imageRaft, calcx - 10, calcy - 10, size, size);
						}
						else {
							let size = 24 / zoomLevel;
							ctx.drawImage(imageRaft, calcx - 10, calcy - 10, size, size);
						}
					}
					
					// draw saving points
					if (drawSavepoints) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SAVE"))
							)
						{
							let size = 24 / zoomLevel;
							ctx.drawImage(imageSavepoint, calcx - size/2, calcy - size/2, size, size);
							//ctx.drawImage(imageSavepoint, calcx - 12, calcy - 12, 24, 24);
							
							// if shelter has label, show island name
							if (currentvalue.hasOwnProperty("dName"))
							{
								let fontsize = 10 / zoomLevel;
								let blockheight = 20 / zoomLevel;
								
								if (withTextures) {
									ctx.fillStyle = '#d8bc9d';
								}
								else {
									ctx.fillStyle = '#ccffff';
								}
								ctx.fillRect(x - zoneXwidth + 5, y - zoneYwidth + 16 - 9/zoomLevel, 500 + 50/zoomLevel, blockheight);
								ctx.fillStyle = 'black';
								if (withTextures) {
									ctx.font = "bold "+fontsize+"px Lucida Handwriting";
								}
								else {
									ctx.font = "bold "+fontsize+"px Arial";	
								}
								
								//ctx.fillText(currentZone.Id + " (" + currentZone.Name + ")", x - zoneXwidth + 10, y - zoneYwidth + 20);	
								ctx.fillText(currentvalue["dName"], x - zoneXwidth + 10, y - zoneYwidth + 20);	
								//ctx.fillText(currentZone.Id, x - zoneXwidth + 10, y - zoneYwidth + 20);	
								ctx.fillStyle = 'black';
							}
						}
					}
					
					if (drawRaftMaterials)
					{
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("BARREL"))
							)
						{
							ctx.drawImage(imageBarrel, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("BUOY"))
							)
						{
							ctx.drawImage(imageBuoy, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("TIRE"))
							)
						{
							ctx.drawImage(imageTire, calcx - 12, calcy - 12, 24, 24);
							
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
							ctx.drawImage(imageFruit, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("COCONUT"))
							)
						{
							ctx.drawImage(imageCoconut, calcx - 6, calcy - 6, 12, 12);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("POTATO"))
							)
						{
							ctx.drawImage(imagePotato, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("YUCCA")))
						{
							ctx.drawImage(imageYucca, calcx - 12, calcy - 12, 24, 24);
							
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
							ctx.drawImage(imageFlower, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					}
					
					// draw buildings
					if (drawBuildings) {
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SHELTER"))
							)
						{
							ctx.drawImage(imageHut, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("INDUSTRY"))
							)
						{
							if (currentvalue.hasOwnProperty("Parent")) {
								currentItemX = Number(String(currentvalue["Parent"]["Transform"]["localPosition"]["x"]).replace(",","."));
								currentItemY = Number(String(currentvalue["Parent"]["Transform"]["localPosition"]["y"]).replace(",","."));
								currentItemZ = Number(String(currentvalue["Parent"]["Transform"]["localPosition"]["z"]).replace(",","."));
								
								// calculated position
								calcx = x + Number(currentItemX) * (width / 1000 / widthratio);
								calcy = y - Number(currentItemZ) * (height / 1000 / heightratio);
							}
							

							if (String(allDictionary[key]).includes("WATER")) {
								ctx.drawImage(imageWater, calcx - 12, calcy - 12, 24, 24);
							}
							else if (String(allDictionary[key]).includes("FIRE")) {
								ctx.drawImage(imageFire, calcx - 12, calcy - 12, 24, 24);
							}
							else {
								ctx.drawImage(imageIndustry, calcx - 12, calcy - 12, 24, 24);
							}

							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("SEAFORT"))
							)
						{
							ctx.drawImage(imageSeafort, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
						
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("STRUCTURE"))
							)
						{
							ctx.drawImage(imageFoundation, calcx - 12, calcy - 12, 24, 24);
							
							// contructions list
							$.each(currentvalue["Constructions"], function(name, currentConstruction) {
								// loop on references
								$.each(currentZone.Objects, function(name, currentconstructiontolookfor) {
									// shitty inner loop to get the sub elements
									if (currentConstruction == currentconstructiontolookfor["reference"])
									{
										let currentSubItemX = Number(String(currentconstructiontolookfor["Transform"]["localPosition"]["x"]).replace(",","."));
										let currentSubItemZ = Number(String(currentconstructiontolookfor["Transform"]["localPosition"]["z"]).replace(",","."));
										
										let subcalcx = calcx + Number(currentSubItemX);
										let subcalcy = calcy + Number(currentSubItemZ);
										
										ctx.drawImage(imageFoundation, subcalcx - 12, subcalcy - 12, 24, 24);
									}
								});
							});
						}
						
						// draw containers
						if (allDictionary.hasOwnProperty(key) 
							&& String(allDictionary[key]).includes("DRAW")
							&& (String(allDictionary[key]).includes("CONTAINER"))
						)
						{
							ctx.drawImage(imageContainer, calcx - 12, calcy - 12, 24, 24);
							
							// tooltip
							addTooltip(calcx, calcy, key);
						}
					}
					
					if (debug
						&& allDictionary.hasOwnProperty(key)
						&& key != ""
						&& String(allDictionary[key]) == "")
					{
						writeLog(key + " has been found, but not drawn");
					}
				});
			}
			
			// draw player 1 position
			if (newArr.Persistent.StrandedWorld.PlayersData.length == 1
				&& newArr.Persistent.StrandedWorld.PlayersData[0].ZoneId.localeCompare(currentZone.Id) == 0)
			{
				// calculated position
				let calcx = (width/2) + (Number(String(newArr.Persistent.StrandedWorld.PlayersData[0].Position["x"]).replace(",","."))) * distancescalefactor;// local position conversion from center of island
				let calcy = (height/2) - (Number(String(newArr.Persistent.StrandedWorld.PlayersData[0].Position["z"]).replace(",","."))) * distancescalefactor;// local position conversion from center of island

				let size = 24 / zoomLevel;
				ctx.drawImage(imagePlayer, calcx - size/2, calcy - size/2, size, size);
				
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
			
			// draw player 2 position
			if (newArr.Persistent.StrandedWorld.PlayersData.length == 2
				&& newArr.Persistent.StrandedWorld.PlayersData[1].ZoneId.localeCompare(currentZone.Id) == 0)
			{
				// calculated position
				let calcx = (width/2) + (Number(String(newArr.Persistent.StrandedWorld.PlayersData[1].Position["x"]).replace(",","."))) * distancescalefactor;// local position conversion from center of island
				let calcy = (height/2) - (Number(String(newArr.Persistent.StrandedWorld.PlayersData[1].Position["z"]).replace(",","."))) * distancescalefactor;// local position conversion from center of island
								
				let size = 24 / zoomLevel;
				ctx.drawImage(imagePlayer, calcx - size/2, calcy - size/2, size, size);
				
				// tooltip
				addTooltip(calcx, calcy, "Yeah, that\'s your friend");
			}
		//}
	}
	var t1 = performance.now();
	console.log("Map render time = " + (t1 - t0) + " milliseconds.")
}

var namesDictionary = {};

namesDictionary["MAP_NAME_PREFIX_1"] = "LITTLE";
namesDictionary["MAP_NAME_PREFIX_2"] = "GREAT";
namesDictionary["MAP_NAME_PREFIX_3"] = "UPPER";
namesDictionary["MAP_NAME_PREFIX_4"] = "LOWER";
namesDictionary["MAP_NAME_PREFIX_5"] = "ENDLESS";
namesDictionary["MAP_NAME_PREFIX_6"] = "SOUTHERN";
namesDictionary["MAP_NAME_PREFIX_7"] = "WESTERN";
namesDictionary["MAP_NAME_PREFIX_8"] = "TAINTED";

namesDictionary["MAP_NAME_STEM_1"] = "BOTTOM";
namesDictionary["MAP_NAME_STEM_2"] = "RAGING";
namesDictionary["MAP_NAME_STEM_3"] = "END";
namesDictionary["MAP_NAME_STEM_4"] = "DARK";
namesDictionary["MAP_NAME_STEM_5"] = "SECRET";
namesDictionary["MAP_NAME_STEM_6"] = "BOTTOMLESS";
namesDictionary["MAP_NAME_STEM_7"] = "ENDLESS";
namesDictionary["MAP_NAME_STEM_8"] = "FORGOTTEN";
namesDictionary["MAP_NAME_STEM_9"] = "ANCIENT";
namesDictionary["MAP_NAME_STEM_10"] = "TROPICAL";
	
namesDictionary["MAP_NAME_SUFFIX_1"] = "ISLAND";
namesDictionary["MAP_NAME_SUFFIX_2"] = "ATOLL";
namesDictionary["MAP_NAME_SUFFIX_3"] = "CREST";
namesDictionary["MAP_NAME_SUFFIX_4"] = "CORNER";
namesDictionary["MAP_NAME_SUFFIX_5"] = "SANCTUARY";
namesDictionary["MAP_NAME_SUFFIX_6"] = "ESCAPE";

var allDictionary = {};

function initDictionaries() {
	// keys = DRAW|ROCK|PALM|TREE|SHARK|MARLIN|WHALE|STINGRAY|BOARD|SNAKE|WRECK|TOOL|MINABLE|FORT|SAVE|RESOURCE|RAFT|RAFTMAT|TIRE|BUOY|BARREL|HELI|TOOL
	allDictionary["BAT"] = "BAT";
    allDictionary["SEAGULL"] = "SEAGULL";
    allDictionary["BOAR"] = "DRAW|BOAR";
    allDictionary["BOAR_RAGDOLL"] = "DRAW|BOAR";
	allDictionary["BOAR_SPAWNER"] = "DRAW|BOAR";
	allDictionary["HOG"] = "DRAW|BOAR";
	allDictionary["HOG_RAGDOLL"] = "DRAW|BOAR";
	allDictionary["HOG_SPAWNER"] = "DRAW|BOAR";
    allDictionary["CRAB"] = "DRAW|CRAB";
	allDictionary["CRAB_SPAWNER"] = "DRAW|CRAB"; // need to add
	allDictionary["GIANT_CRAB_SPAWNER"] = "DRAW|CRAB"; // need to add
	allDictionary["GIANT_CRAB_RAGDOLL"] = "DRAW|CRAB"; // need to add
	allDictionary["GIANT_CRAB"] = "CRAB";
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
    allDictionary["MARLIN_RAGDOLL"] = "DRAW|MARLIN";
	
	allDictionary["WHALE"] = "DRAW|WHALE";
	
    allDictionary["SHARK_WHITE"] = "DRAW|SHARK";
    allDictionary["SHARK_REEF"] = "DRAW|SHARK";
    allDictionary["SHARK_TIGER"] = "DRAW|SHARK";
    	
    allDictionary["SHARK_GREAT WHITE_RAGDOLL"] = "DRAW|SHARK";
    allDictionary["SHARK_REEF_RAGDOLL"] = "DRAW|SHARK";
    allDictionary["SHARK_TIGER_SHARK_RAGDOLL"] = "DRAW|SHARK";
	allDictionary["SHARK_HAMMERHEAD_RAGDOLL"] = "DRAW|SHARK";
    allDictionary["PATROL_GREATWHITE"] = "DRAW|SHARK";
    allDictionary["PATROL_MARLIN"] = "DRAW|MARLIN";
    allDictionary["PATROL_REEFSHARK"] = "DRAW|SHARK";
    allDictionary["PATROL_TIGERSHARK"] = "DRAW|SHARK";
	allDictionary["PATROL_HAMMERHEAD"] = "DRAW|SHARK";
	
    allDictionary["HIDESPOT_SNAKE"] = "DRAW|SNAKE";
    allDictionary["SNAKE"] = "DRAW|SNAKE";
    allDictionary["SNAKE_RAGDOLL"] = "DRAW|SNAKE";
	allDictionary["SNAKE_SPAWNER"] = "DRAW|SNAKE";
	
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
	allDictionary["BRICK_STEPS"] = "";
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
	allDictionary["RAFT_CANOPY"] = "";
    allDictionary["SCRAP_CORRUGATED"] = "DRAW|RESOURCE";
    allDictionary["SCRAP_PLANK"] = "DRAW|RESOURCE";
	allDictionary["SHIPPING_CONTAINER"] = "DRAW|CONTAINER";
    allDictionary["SHIPPING_CONTAINER_1"] = "DRAW|CONTAINER";
    allDictionary["SHIPPING_CONTAINER_2"] = "DRAW|CONTAINER";
    allDictionary["SHIPPING_CONTAINER_3"] = "DRAW|CONTAINER";
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
    allDictionary["CONTAINER_CRATE"] = "DRAW|CRATE";
    allDictionary["CONTAINER_LOCKER_LARGE"] = "";
    allDictionary["CONTAINER_LOCKER_SMALL"] = "";
	allDictionary["CONTAINER_SHELF"] = "";
    allDictionary["BED"] = "DRAW|SAVE";
    allDictionary["BED_SHELTER"] = "DRAW|SAVE";
    allDictionary["CLAY"] = "DRAW|ITEM";
    allDictionary["CLOTH"] = "DRAW|ITEM";
    allDictionary["DRIFTWOOD_STICK"] = "DRAW|ITEM";
    allDictionary["ENGINE"] = "DRAW|ITEM";
    allDictionary["ENGINE_FUEL_TANK"] = "DRAW|ITEM";
    allDictionary["ENGINE_PROPELLER"] = "DRAW|ITEM";
    allDictionary["ENGINE_PUMP"] = "DRAW|ITEM";
    allDictionary["FIRE_TORCH"] = "DRAW|ITEM";
    allDictionary["KINDLING"] = "DRAW|ITEM";
    allDictionary["LEATHER"] = "DRAW|ITEM";
    allDictionary["LEAVES_FIBROUS"] = "DRAW|ITEM";
    allDictionary["PALM_FROND"] = "DRAW|ITEM";
    allDictionary["RAWHIDE"] = "DRAW|ITEM";
    allDictionary["ROCK"] = "DRAW|ITEM";
    allDictionary["ROPE_COIL"] = "DRAW|ITEM";
	allDictionary["SPYGLASS"] = "DRAW|ITEM";
    allDictionary["STICK"] = "DRAW|ITEM";
    allDictionary["STONE_TOOL"] = "DRAW|TOOL";
    allDictionary["EGG_DEADEX"] = "DRAW|ITEM";
    allDictionary["EGG_WOLLIE"] = "DRAW|ITEM";	
    allDictionary["FARMING_HOE"] = "DRAW|TOOL";

    allDictionary["LOOM"] = "DRAW|INDUSTRY";
    allDictionary["TANNING_RACK"] = "DRAW|INDUSTRY";
    allDictionary["WATER_STILL"] = "DRAW|INDUSTRY|WATER";
    allDictionary["NEW_CAMPFIRE"] = "DRAW|INDUSTRY|FIRE";
    allDictionary["NEW_CAMPFIRE_PIT"] = "DRAW|INDUSTRY|FIRE";
    allDictionary["NEW_CAMPFIRE_SPIT"] = "DRAW|INDUSTRY|FIRE";	
	allDictionary["FUEL_STILL"] = "DRAW|INDUSTRY";
    allDictionary["FARMING_PLOT_CORRUGATED"] = "DRAW|INDUSTRY";
    allDictionary["FARMING_PLOT_PLANK"] = "DRAW|INDUSTRY";
    allDictionary["FARMING_PLOT_WOOD"] = "DRAW|INDUSTRY";
	allDictionary["FURNACE"] = "DRAW|INDUSTRY";
    allDictionary["HOBO_STOVE"] = "DRAW|INDUSTRY|FIRE";
	allDictionary["SMOKER"] = "DRAW|INDUSTRY|FIRE";
	allDictionary["BRICK_STATION"] = "DRAW|INDUSTRY";
	allDictionary["PLANK_STATION"] = "DRAW|INDUSTRY";
	allDictionary["TRAP_FISH"] = "DRAW|INDUSTRY";
    allDictionary["RAIN_CATCHER"] = "DRAW|INDUSTRY|WATER";
	
    allDictionary["KURA_FRUIT"] = "DRAW|FRUIT";
    allDictionary["KURA_TREE"] = "DRAW|TREE";
    allDictionary["QUWAWA_FRUIT"] = "DRAW|FRUIT";
    allDictionary["QUWAWA_TREE"] = "DRAW|TREE";
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
	allDictionary["LIGHT_HOOK"] = "";
	
    allDictionary["ANTIBIOTICS"] = "DRAW|ITEM";
    allDictionary["BANDAGE"] = "DRAW|ITEM";
    allDictionary["MEDICAL_ALOE_SALVE"] = "DRAW|ITEM";
    allDictionary["MORPHINE"] = "DRAW|ITEM";
    allDictionary["NEW_COCONUT_FLASK"] = "DRAW|ITEM";
    allDictionary["NEW_COCONUT_MEDICAL"] = "DRAW|ITEM";
    allDictionary["VITAMINS"] = "DRAW|ITEM";
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
    allDictionary["CLIFF_001"] = "DRAW|ROCK|CLIFF";
    allDictionary["CLIFF_002"] = "DRAW|ROCK|CLIFF";
    allDictionary["CLIFF_003"] = "DRAW|ROCK|CLIFF";
    allDictionary["CLIFF_004"] = "DRAW|ROCK|CLIFF";
    allDictionary["CLIFF_005"] = "DRAW|ROCK|CLIFF";
    allDictionary["CLIFF_006"] = "DRAW|ROCK|CLIFF";	
    allDictionary["DRIFTWOOD_DECAL"] = "";
    allDictionary["OCEAN_BUOY"] = "";
    allDictionary["PHILODENDRON_1"] = "DRAW|TREE";
    allDictionary["PHILODENDRON_2"] = "DRAW|TREE";
    allDictionary["SHORELINE_ROCK_1"] = "DRAW|ROCK";
    allDictionary["SHORELINE_ROCK_2"] = "DRAW|ROCK";
    allDictionary["SMALLROCK_1"] = "DRAW|ROCK";
    allDictionary["SMALLROCK_2"] = "DRAW|ROCK";
    allDictionary["SMALLROCK_3"] = "DRAW|ROCK";
    allDictionary["SeaFort_1"] = "DRAW|SEAFORT";
    allDictionary["SeaFort_2"] = "DRAW|SEAFORT";
	allDictionary["SeaFort_3"] = "DRAW|SEAFORT";
    allDictionary["SeaFort_Brige"] = "";
    allDictionary["SeaFort_Brige_Broken"] = "";
    allDictionary["DOOR"] = "";
	allDictionary["DOOR_13M_165d"] = "";
	allDictionary["DOOR_13M_85d"] = "";
	allDictionary["DOOR_13_85d"] = "";
	allDictionary["DOOR_13_165d"] = "";
	allDictionary["DOOR_13D1"] = "";
	allDictionary["DOOR_13D2"] = "";
	allDictionary["DOOR_13D3"] = "";
    allDictionary["ROWBOAT_3"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_2A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_3A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_4A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_5A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_6A"] = "DRAW|WRECK";
    allDictionary["SHIPWRECK_7A"] = "DRAW|WRECK";
	allDictionary["SHIPWRECK_8A"] = "DRAW|WRECK";
	allDictionary["SHIPWRECK_9A"] = "DRAW|WRECK";
	allDictionary["SHIPWRECK_10A"] = "DRAW|WRECK";
	allDictionary["SHIPWRECK_11A"] = "DRAW|WRECK";
	allDictionary["SHIPWRECK_12A"] = "DRAW|WRECK";
	allDictionary["SHIPWRECK_13A"] = "DRAW|WRECK";
	allDictionary["PLANEWRECK_1A"] = "DRAW|WRECK";
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
    allDictionary["FICUS_1"] = "DRAW|TREE|MINEABLE";
    allDictionary["FICUS_2"] = "DRAW|TREE|MINEABLE";
    allDictionary["FICUS_3"] = "DRAW|TREE|MINEABLE";
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
    allDictionary["PINE_SMALL_1"] = "DRAW|TREE|MINEABLE";
    allDictionary["PINE_SMALL_2"] = "DRAW|TREE|MINEABLE";
    allDictionary["PINE_SMALL_3"] = "DRAW|TREE|MINEABLE";
    allDictionary["POTATO_PLANT"] = "POTATO";
    allDictionary["YOUNG_PALM_1"] = "DRAW|RESOURCE";
    allDictionary["YOUNG_PALM_2"] = "DRAW|RESOURCE";
    allDictionary["YUCCA"] = "DRAW|YUCCA";
	allDictionary["YUCCA_CUTTING"] = "DRAW|YUCCA";
	allDictionary["YUCCA_HARVEST"] = "DRAW|YUCCA";
	allDictionary["ALOE_VERA_FRUIT"] = "DRAW|FLOWER";
    allDictionary["ALOE_VERA_PLANT"] = "DRAW|PLANT";
    allDictionary["AJUGA_PLANT"] = "TREE";
    allDictionary["AJUGA"] = "DRAW|FLOWER";
    allDictionary["WAVULAVULA_PLANT"] = "DRAW|PLANT";
    allDictionary["WAVULAVULA"] = "DRAW|FLOWER";
    allDictionary["PIPI_PLANT"] = "DRAW|PLANT";
    allDictionary["PIPI"] = "DRAW|FLOWER";
    allDictionary["COCONUT_FLASK"] = "DRAW|ITEM";
	
	allDictionary["PADDLE"] = "DRAW|TOOL";
	
	allDictionary["PART_ELECTRICAL"] = "DRAW|ITEM";
	allDictionary["PART_FILTER"] = "DRAW|ITEM";
	allDictionary["PART_GYRO"] = "DRAW|ITEM";
	allDictionary["PART_FUEL"] = "DRAW|ITEM";
	allDictionary["PART_ENGINE"] = "DRAW|ITEM";
	allDictionary["VEHICLE_RUDDER"] = "";
	allDictionary["RAFT_ANCHOR"] = "";
	
	allDictionary["STRUCTURE"] = "DRAW|STRUCTURE";
	
	allDictionary["MISSION_EEL"] = "DRAW|MISSION_EEL";
    allDictionary["MISSION_MARKER"] = "";
    allDictionary["MISSION_SHARK"] = "DRAW|MISSION_MEG";
    allDictionary["MISSION_SQUID"] = "DRAW|MISSION_SQUID";
    allDictionary["MISSION_TROPHY_EEL"] = "";
    allDictionary["MISSION_TROPHY_SHARK"] = "";
    allDictionary["MISSION_TROPHY_SQUID"] = "";
	allDictionary["AIRCRAFT"] = "DRAW|MISSION_CARRIER";
	
	allDictionary["Shelter01"] = "DRAW|SHELTER";
	allDictionary["Shelter02"] = "DRAW|SHELTER";
	allDictionary["Shelter03"] = "DRAW|SHELTER";
	allDictionary["Shelter04"] = "DRAW|SHELTER";
	allDictionary["Shelter05"] = "DRAW|SHELTER";
	allDictionary["Shelter06"] = "DRAW|SHELTER";
	
	// probable bug, there are items without keys
	allDictionary[""] = "";
}