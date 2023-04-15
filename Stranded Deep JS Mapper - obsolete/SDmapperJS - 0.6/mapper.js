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

function updateFlags() {
	let chk1 = document.getElementById("checkAnimals");
	drawAnimals = chk1.checked;
	
	let chk2 = document.getElementById("checkWreckages");
	drawWreckages = chk2.checked;
	
	let chk3 = document.getElementById("checkMineables");
	drawMineables = chk3.checked;
	
	let chk4 = document.getElementById("checkSavePoints");
	drawSavepoints = chk4.checked;
	
	drawMap();
}

function resetAll(){
	let c = document.getElementById("mapCanvas");
	
	let ctx = c.getContext("2d");
	ctx.restore();	
	
	marginLeft = 0;
	marginTop = 0;
	c.style.marginLeft = "0px";
	c.style.marginTop = "0px";
		
	drawMap();
}

function initViewPort() {
	let c = document.getElementById("mapCanvas");
	
	let ctx = c.getContext("2d");
	ctx.save();	
	
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
		
		drawMap();
		
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
		drawMap();
    }
}

function clearAll() {
	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");
	
	ctx.restore();
	
	// ghost silhouette bug
	ctx.beginPath();
	
	ctx.fillStyle = "#000088";
	ctx.fillRect(0, 0, c.width, c.height);
	
	ctx.fillStyle = "#ccffff";
	ctx.fillRect(0, 0, width, height);
}

function drawGrid() {
	clearAll();
	
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

function drawMap(focusOnPlayer) {

	if (typeof(newArr) == "undefined"
		|| !newArr.hasOwnProperty("Version")) {
		alert("No save file loaded !");
		return;
	}
	
	if (focusOnPlayer === undefined) {
		focusOnPlayer = false;
    } 

	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");
	
	drawGrid();
	
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
			ctx.fillStyle = 'black';
			ctx.font = "10px Arial";
			ctx.fillText(currentZone.Name, x * zoneXwidth + 10, y * zoneYwidth + 20);
			ctx.fillStyle = 'black';
			
			if (!currentZone.hasOwnProperty("Discovered") || (currentZone.hasOwnProperty("Discovered") && currentZone["Discovered"] == false)){
				let calcx = x * zoneXwidth + 0.5 * zoneXwidth;
				let calcy = y * zoneYwidth + 0.5 * zoneYwidth;
				
				let imageObj = new Image();
				imageObj.src = './icons/unknown.png';
				imageObj.onload = function () {
					ctx.drawImage(imageObj, calcx - 25, calcy - 25, 50, 50);
				};
				
				//ctx.fillStyle = "#000088";
				//ctx.fillRect(x * zoneXwidth + 1, y * zoneYwidth + 1, (x + 1) * zoneXwidth - 1, (y + 1) * zoneYwidth - 1);
			} 
			else {
				// draw water
				//ctx.fillStyle = '#ccffff';
				//ctx.fillRect(x * zoneXwidth + 1, y * zoneYwidth + 1, (x + 1) * zoneXwidth - 2, (y + 1) * zoneYwidth - 2);
				
				// draw island silhouette
				ctx.fillStyle = '#c2b280';
				ctx.beginPath();
				let start = true;
				$.each(currentZone.Objects, function(name, currentvalue) {
					
					let currentItemY = currentvalue["Transform"]["localPosition"]["y"];
					let key = currentvalue["name"].substring(currentvalue["name"].indexOf("]"),currentvalue["name"].indexOf("(Clone)"))
					
					// key not contained in any dictionary, debug to help add it
					if (!treesDictionary.hasOwnProperty(key)
						&& !rocksDictionary.hasOwnProperty(key)
						&& !animalsDictionary.hasOwnProperty(key)
						&& !itemsDictionary.hasOwnProperty(key)
						&& !wreckagesDictionary.hasOwnProperty(key)
						&& !raftsDictionary.hasOwnProperty(key)
						&& !ressourceDictionary.hasOwnProperty(key)
						&& !minableressourceDictionary.hasOwnProperty(key)
						&& !toolsDictionary.hasOwnProperty(key)
						&& !structuresDictionary.hasOwnProperty(key)
						&& !structureSavableDictionary.hasOwnProperty(key)
						&& !wreckageElementsDictionary.hasOwnProperty(key)
						&& !fortsDictionary.hasOwnProperty(key)
						&& !enginePartsDictionary.hasOwnProperty(key)
						&& !otherDictionary.hasOwnProperty(key)
						&& !allDictionary.hasOwnProperty(key)) {
							alert("Key " + key + " not found in any dictionary, add it for mapping completion");
							console.log(currentvalue["name"]);
					}
					
					// island silhouette
					if ((rocksDictionary.hasOwnProperty(key) 
						|| treesDictionary.hasOwnProperty(key)
						|| wreckagesDictionary.hasOwnProperty(key)
						) && currentItemY >= -0.1)
					{						
						let currentItemX = currentvalue["Transform"]["localPosition"]["x"];
						let currentItemZ = currentvalue["Transform"]["localPosition"]["z"];
					
						// calculated position
						let calcx = x * zoneXwidth + 0.5 * zoneXwidth + Number(currentItemX) * (width / 1000);
						let calcy =  y * zoneYwidth + 0.5 * zoneYwidth - Number(currentItemZ) * (height / 1000);
					
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
				ctx.strokeStyle = '#c2b280';
				ctx.stroke();
				ctx.fillStyle = '#c2b280';
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
					if (rocksDictionary.hasOwnProperty(key)) {				
						let imageObj = new Image();
						imageObj.style.zindex = 1000;
						imageObj.src = './icons/rock.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
						};
					}
					
					// draw trees
					if (treesDictionary.hasOwnProperty(key))
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
						if (wreckagesDictionary.hasOwnProperty(key))
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
						if (animalsDictionary.hasOwnProperty(key))
						{
							let imageObj = new Image();
							imageObj.style.zindex = 4000;
							if (key.indexOf("SHARK") !== -1) {
								imageObj.src = './icons/shark.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
							else if (key.indexOf("RAY") !== -1) {
								imageObj.src = './icons/stingray.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 8, calcy - 8, 16, 16);
								};
							}
							else if (key.indexOf("BOAR") !== -1) {
								imageObj.src = './icons/boar.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
							else if (key.indexOf("SNAKE") !== -1) {
								imageObj.src = './icons/snake.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
						}
					}
					
					// draw minable resources
					if (drawMineables) {
						if (minableressourceDictionary.hasOwnProperty(key))
						{
							if (key.indexOf("MINING") !== -1 || key.indexOf("PILE") !== -1) {
								let imageObj = new Image();
								imageObj.style.zindex = 5000;
								imageObj.src = './icons/resource.png';
								imageObj.onload = function () {
									ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
								};
							}
						}
					}
					
					if (toolsDictionary.hasOwnProperty(key))
					{
						let imageObj = new Image();
						imageObj.style.zindex = 5000;
						imageObj.src = './icons/tool.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 10, calcy - 10, 20, 20);
						};
					}
					
					if (itemsDictionary.hasOwnProperty(key))
					{
						let imageObj = new Image();
						imageObj.style.zindex = 5000;
						imageObj.src = './icons/item.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 6, calcy - 6, 12, 12);
						};
					}
					
					if (raftsDictionary.hasOwnProperty(key))
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
						if (structureSavableDictionary.hasOwnProperty(key))
						{
							let imageObj = new Image();
							imageObj.style.zindex = 6000;
							imageObj.src = './icons/save.png';
							imageObj.onload = function () {
								ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
							};
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


var treesDictionary = {};
var rocksDictionary = {};
var animalsDictionary = {};
var itemsDictionary = {};
var wreckagesDictionary = {};
var wreckageElementsDictionary = {};
var raftsDictionary = {};
var ressourceDictionary = {};
var minableressourceDictionary = {};
var toolsDictionary = {};
var structuresDictionary = {};
var structureSavableDictionary = {};
var fortsDictionary = {};
var enginePartsDictionary = {};
var otherDictionary = {};

var allDictionary = {};

function initDictionaries() {
	treesDictionary["PALM_TREE"] = true;
	treesDictionary["PALM_TREE_1"] = true;
	treesDictionary["PALM_TREE_2"] = true;
	treesDictionary["PALM_TREE_3"] = true;
	treesDictionary["PALM_TREE_4"] = true;
	treesDictionary["PINE_2"] = true;
	treesDictionary["PINE_1"] = true;
	treesDictionary["PINE_3"] = true;
	treesDictionary["PINE_2"] = true;	
	treesDictionary["PINE_SMALL_3"] = true;
	treesDictionary["PINE_SMALL_2"] = true;
	treesDictionary["ALOCASIA_2"] = true;
	treesDictionary["YUCCA"] = true;
	treesDictionary["POTATO_PLANT"] = true;
	treesDictionary["FICUS_1"] = true;
	treesDictionary["FICUS_2"] = true;
	treesDictionary["CERIMAN_2"] = true;
	treesDictionary["BANANA_PLANT"] = true;
	treesDictionary["QUWAWA_TREE"] = true;
	treesDictionary["COCA_BUSH"] = true;
	treesDictionary["YOUNG_PALM_1"] = true;
	treesDictionary["YOUNG_PALM_2"] = true;
	treesDictionary["FICUS_TREE_2"] = true;
	treesDictionary["FICUS_3"] = true;
	treesDictionary["KURA_TREE"] = true;
	treesDictionary["PALM_1"] = true;
	treesDictionary["PALM_4"] = true;
	treesDictionary["PALM_3"] = true;
	treesDictionary["PALM_2"] = true;
	treesDictionary["CERIMAN_3"] = true;
	treesDictionary["ALOCASIA_1"] = true;
	treesDictionary["CERIMAN_1"] = true;
	treesDictionary["FICUS_TREE"] = true;
	
	rocksDictionary["ROCK"] = true;
	rocksDictionary["CLIFF_001"] = true;
	rocksDictionary["CLIFF_002"] = true;
	rocksDictionary["CLIFF_003"] = true;
	rocksDictionary["CLIFF_004"] = true;
	rocksDictionary["CLIFF_005"] = true;
	rocksDictionary["CLIFF_006"] = true;
	rocksDictionary["BIGROCK_2"] = true;
	rocksDictionary["BIGROCK_1"] = true;
	rocksDictionary["SMALLROCK_1"] = true;
	rocksDictionary["SMALLROCK_3"] = true;
	rocksDictionary["SMALLROCK_2"] = true;
	rocksDictionary["SHORELINE_ROCK_2"] = true;
	rocksDictionary["BIGROCK_5"] = true;
	rocksDictionary["BIGROCK_4"] = true;
	rocksDictionary["BIGROCK_3"] = true;
	rocksDictionary["SHORELINE_ROCK_1"] = true;

	animalsDictionary["BAT"] = true;
	animalsDictionary["SEAGULL"] = true;
	animalsDictionary["BOAR"] = true;
	animalsDictionary["BOAR_RAGDOLL"] = true;
	animalsDictionary["CRAB"] = true;
	animalsDictionary["ARCHER"] = true;
	animalsDictionary["CLOWN_TRIGGERFISH"] = true;
	animalsDictionary["COD"] = true;
	animalsDictionary["DISCUS"] = true;
	animalsDictionary["LIONFISH"] = true;
	animalsDictionary["PILCHARD"] = true;
	animalsDictionary["SARDINE"] = true;
	animalsDictionary["STING_RAY"] = true;
	animalsDictionary["STING_RAY_RAGDOLL"] = true;
	animalsDictionary["MARLIN"] = true;
	animalsDictionary["SHARK_WHITE"] = true;
	animalsDictionary["SHARK_REEF"] = true;
	animalsDictionary["SHARK_TIGER"] = true;
	animalsDictionary["WHALE"] = true;
	animalsDictionary["MARLIN_RAGDOLL"] = true;
	animalsDictionary["SHARK_GREAT WHITE_RAGDOLL"] = true;
	animalsDictionary["SHARK_REEF_RAGDOLL"] = true;
	animalsDictionary["SHARK_TIGER_SHARK_RAGDOLL"] = true;
	animalsDictionary["PATROL_GREATWHITE"] = true;
	animalsDictionary["PATROL_MARLIN"] = true;
	animalsDictionary["PATROL_REEFSHARK"] = true;
	animalsDictionary["PATROL_TIGERSHARK"] = true;
	animalsDictionary["HIDESPOT_SNAKE"] = true;
	animalsDictionary["SNAKE"] = true;
	animalsDictionary["SNAKE_RAGDOLL"] = true;


	
	itemsDictionary["STICK"] = true;
	itemsDictionary["DRIFTWOOD_STICK"] = true;
	itemsDictionary["BARREL"] = true;
	itemsDictionary["BUOYBALL"] = true;
	itemsDictionary["FLARE_GUN"] = true;
	itemsDictionary["LANTERN"] = true;
	
	wreckagesDictionary["RowBoat_3"] = true;
	wreckagesDictionary["SHIPWRECK_1A"] = true;
	wreckagesDictionary["SHIPWRECK_2A"] = true;
	wreckagesDictionary["SHIPWRECK_3A"] = true;
	wreckagesDictionary["SHIPWRECK_4A"] = true;
	wreckagesDictionary["SHIPWRECK_5A"] = true;
	wreckagesDictionary["SHIPWRECK_6A"] = true;
	wreckagesDictionary["SHIPWRECK_7A"] = true;
	wreckagesDictionary["PLANEWRECK"] = true;

	wreckageElementsDictionary["DOOR"] = true;
	wreckageElementsDictionary["FLOOR_HATCH"] = true;
	
	raftsDictionary["PADDLE"] = true;
	raftsDictionary["RAFT_V1"] = true;
	raftsDictionary["VEHICLE_LIFERAFT"] = true;
	raftsDictionary["STRUCTURE_RAFT"] = true;
	//raftsDictionary["RAFT_BASE_BALLS"] = true;
	//raftsDictionary["RAFT_FLOOR_PLANK"] = true;

	ressourceDictionary["COCONUT_ORANGE"] = true;
	ressourceDictionary["COCONUT_GREEN"] = true;
	ressourceDictionary["COCONUT_DRINKABLE"] = true;
	ressourceDictionary["QUWAWA_FRUIT"] = true;
	ressourceDictionary["LEAVES_FIBROUS"] = true;
	ressourceDictionary["LOG_1"] = true;
	ressourceDictionary["POTATO"] = true;
	ressourceDictionary["PALM_FROND"] = true;
	ressourceDictionary["MEAT_SMALL"] = true;
	ressourceDictionary["SCRAP_CORRUGATED"] = true;
	ressourceDictionary["SCRAP_PLANK"] = true;
	ressourceDictionary["KURA_FRUIT"] = true;
	ressourceDictionary["PALM_TOP"] = true;
	
	minableressourceDictionary["HARDCASE_1"] = true;
	minableressourceDictionary["TOOLBOX_1"] = true;
	minableressourceDictionary["LOCKER_1"] = true;
	minableressourceDictionary["LOCKER_2"] = true;
	minableressourceDictionary["LOCKER_3"] = true;
	minableressourceDictionary["LOCKER_4"] = true;
	minableressourceDictionary["CONSOLE_1"] = true;
	minableressourceDictionary["CONSOLE_2"] = true;
	minableressourceDictionary["CONSOLE_3"] = true;
	minableressourceDictionary["WALL_CABINET_1"] = true;
	minableressourceDictionary["CONTAINER_CONSOLE"] = true;
	minableressourceDictionary["CONTAINER_CRATE"] = true;
	minableressourceDictionary["CONTAINER_LOCKER_LARGE"] = true;
	minableressourceDictionary["CONTAINER_LOCKER_SMALL"] = true;
	minableressourceDictionary["BARREL_PILE"] = true;
	minableressourceDictionary["MINING_ROCK"] = true;
	minableressourceDictionary["TYRE_PILE"] = true;
	minableressourceDictionary["MINING_ROCK_CLAY"] = true;
	minableressourceDictionary["BUOYBALL_PILE"] = true;
	minableressourceDictionary["DRIFTWOOD_PILE"] = true;
	minableressourceDictionary["PALM_LOG_1"] = true;
	minableressourceDictionary["PALM_LOG_2"] = true;
	minableressourceDictionary["PALM_LOG_3"] = true;
	
	
	toolsDictionary["NEW_REFINED_PICK"] = true;
	toolsDictionary["NEW_FISHING_SPEAR"] = true;
	toolsDictionary["FARMING_HOE"] = true;
	toolsDictionary["KINDLING"] = true;
	toolsDictionary["NEW_CRUDE_SPEAR"] = true;
	toolsDictionary["NEW_CRUDE_AXE"] = true;
	toolsDictionary["NEW_CRUDE_HAMMER"] = true;
	toolsDictionary["FIRE_TORCH"] = true;

	
	structuresDictionary["STRUCTURE_SMALL"] = true;
	structuresDictionary["NEW_CAMPFIRE"] = true;
	structuresDictionary["NEW_CAMPFIRE_PIT"] = true;
	structuresDictionary["SMOKER"] = true;
	structuresDictionary["WATER_STILL"] = true;
	structuresDictionary["TANNING_RACK"] = true;
	structuresDictionary["STRUCTURE"] = true;
	structuresDictionary["WOOD_FOUNDATION"] = true;
	structuresDictionary["WOOD_STEPS"] = true;
	structuresDictionary["PLANK_PANEL_STATIC"] = true;
	structuresDictionary["PLANK_WALL_WINDOW"] = true;
	structuresDictionary["WOOD_PANEL_STATIC"] = true;
	structuresDictionary["WOOD_WALL_HALF"] = true;
	structuresDictionary["WOOD_ARCH"] = true;
	structuresDictionary["WOOD_FLOOR"] = true;
	structuresDictionary["WOOD_WALL_WINDOW"] = true;
	structuresDictionary["TARP_PANEL"] = true;
	structuresDictionary["FARMING_PLOT_CORRUGATED"] = true;
	structuresDictionary["HOBO_STOVE"] = true;
	structuresDictionary["DRIFTWOOD_PANEL_STATIC"] = true;

	
	structureSavableDictionary["BED_SHELTER"] = true;

	fortsDictionary["Sea_Fort_1"] = true;
	fortsDictionary["Sea_Fort_2"] = true;
	fortsDictionary["Sea_Fort_3"] = true;
	fortsDictionary["Sea_Fort_Bridge"] = true;
	
	enginePartsDictionary["ENGINE_FUEL_TANK"] = true;
	enginePartsDictionary["ENGINE_PUMP"] = true;
	enginePartsDictionary["ENGINE_PROPELLER"] = true;
	
	otherDictionary["DRIFTWOOD_DECAL"] = true;
	
	
	// All for 0.39
	allDictionary["BAT"] = true;
    allDictionary["SEAGULL"] = true;
    allDictionary["BOAR"] = true;
    allDictionary["BOAR_RAGDOLL"] = true;
    allDictionary["CRAB"] = true;
    allDictionary["ARCHER"] = true;
    allDictionary["CLOWN_TRIGGERFISH"] = true;
    allDictionary["COD"] = true;
    allDictionary["DISCUS"] = true;
    allDictionary["LIONFISH"] = true;
    allDictionary["PILCHARD"] = true;
    allDictionary["SARDINE"] = true;
    allDictionary["STING_RAY"] = true;
    allDictionary["STING_RAY_RAGDOLL"] = true;
    allDictionary["MARLIN"] = true;
    allDictionary["SHARK_WHITE"] = true;
    allDictionary["SHARK_REEF"] = true;
    allDictionary["SHARK_TIGER"] = true;
    allDictionary["WHALE"] = true;
    allDictionary["MARLIN_RAGDOLL"] = true;
    allDictionary["SHARK_GREAT WHITE_RAGDOLL"] = true;
    allDictionary["SHARK_REEF_RAGDOLL"] = true;
    allDictionary["SHARK_TIGER_SHARK_RAGDOLL"] = true;
    allDictionary["PATROL_GREATWHITE"] = true;
    allDictionary["PATROL_MARLIN"] = true;
    allDictionary["PATROL_REEFSHARK"] = true;
    allDictionary["PATROL_TIGERSHARK"] = true;
    allDictionary["HIDESPOT_SNAKE"] = true;
    allDictionary["SNAKE"] = true;
    allDictionary["SNAKE_RAGDOLL"] = true;
    allDictionary["BARREL"] = true;
    allDictionary["BARREL_PILE"] = true;
    allDictionary["BRICK_ARCH"] = true;
    allDictionary["BRICK_FLOOR"] = true;
    allDictionary["BRICK_FOUNDATION"] = true;
    allDictionary["BRICK_PANEL_STATIC"] = true;
    allDictionary["BRICK_ROOF_CAP"] = true;
    allDictionary["BRICK_ROOF_CORNER"] = true;
    allDictionary["BRICK_ROOF_MIDDLE"] = true;
    allDictionary["BRICK_ROOF_WEDGE"] = true;
    allDictionary["BRICK_WALL_HALF"] = true;
    allDictionary["BRICK_WALL_WINDOW"] = true;
    allDictionary["BRICK_WEDGE_FLOOR"] = true;
    allDictionary["BRICK_WEDGE_FOUNDATION"] = true;
    allDictionary["BRICKS"] = true;
    allDictionary["BUOYBALL"] = true;
    allDictionary["BUOYBALL_PILE"] = true;
    allDictionary["CORRUGATED_ARCH"] = true;
    allDictionary["CORRUGATED_DOOR"] = true;
    allDictionary["CORRUGATED_FLOOR"] = true;
    allDictionary["CORRUGATED_FOUNDATION"] = true;
    allDictionary["CORRUGATED_PANEL_STATIC"] = true;
    allDictionary["CORRUGATED_STEPS"] = true;
    allDictionary["CORRUGATED_WALL_HALF"] = true;
    allDictionary["CORRUGATED_WALL_WINDOW"] = true;
    allDictionary["CORRUGATED_WEDGE_FLOOR"] = true;
    allDictionary["CORRUGATED_WEDGE_FOUNDATION"] = true;
    allDictionary["DRIFTWOOD_ARCH"] = true;
    allDictionary["DRIFTWOOD_DOOR"] = true;
    allDictionary["DRIFTWOOD_FLOOR"] = true;
    allDictionary["DRIFTWOOD_FOUNDATION"] = true;
    allDictionary["DRIFTWOOD_PANEL_STATIC"] = true;
    allDictionary["DRIFTWOOD_STEPS"] = true;
    allDictionary["DRIFTWOOD_WALL_HALF"] = true;
    allDictionary["DRIFTWOOD_WALL_WINDOW"] = true;
    allDictionary["DRIFTWOOD_WEDGE_FLOOR"] = true;
    allDictionary["DRIFTWOOD_WEDGE_FOUNDATION"] = true;
    allDictionary["GYRO_BASE_1"] = true;
    allDictionary["GYRO_COCKPIT_4"] = true;
    allDictionary["GYRO_MOTOR_3"] = true;
    allDictionary["GYRO_ROTORS_5"] = true;
    allDictionary["GYRO_SEAT_2"] = true;
    allDictionary["GYRO_STRUCTURE"] = true;
    allDictionary["RAFT_OUTRIGGER"] = true;
    allDictionary["WOOD_CANOE"] = true;
    allDictionary["WOOD_RAFT"] = true;
    allDictionary["PLANK_ARCH"] = true;
    allDictionary["PLANK_DOOR"] = true;
    allDictionary["PLANK_FLOOR"] = true;
    allDictionary["PLANK_FOUNDATION"] = true;
    allDictionary["PLANK_PANEL_STATIC"] = true;
    allDictionary["PLANK_STEPS"] = true;
    allDictionary["PLANK_WALL_HALF"] = true;
    allDictionary["PLANK_WALL_WINDOW"] = true;
    allDictionary["PLANK_WEDGE_FLOOR"] = true;
    allDictionary["PLANK_WEDGE_FOUNDATION"] = true;
    allDictionary["RAFT_BASE_BALLS"] = true;
    allDictionary["RAFT_BASE_BARREL"] = true;
    allDictionary["RAFT_BASE_TYRE"] = true;
    allDictionary["RAFT_BASE_WOOD_BUNDLE"] = true;
    allDictionary["RAFT_FLOOR_CORRUGATED"] = true;
    allDictionary["RAFT_FLOOR_DRIFTWOOD"] = true;
    allDictionary["RAFT_FLOOR_PLANK"] = true;
    allDictionary["RAFT_FLOOR_STEEL"] = true;
    allDictionary["RAFT_FLOOR_WOOD"] = true;
    allDictionary["SCRAP_CORRUGATED"] = true;
    allDictionary["SCRAP_PLANK"] = true;
    allDictionary["SHIPPING_CONTAINER_1"] = true;
    allDictionary["SHIPPING_CONTAINER_2"] = true;
    allDictionary["SHIPPING_CONTAINER_3"] = true;
    allDictionary["SHIPPING_CONTAINER_DOOR"] = true;
    allDictionary["SHIPPING_CONTAINER_DOOR_STATIC"] = true;
    allDictionary["SHIPPING_CONTAINER_FLOOR"] = true;
    allDictionary["SHIPPING_CONTAINER_FOUNDATION"] = true;
    allDictionary["SHIPPING_CONTAINER_PANEL"] = true;
    allDictionary["SHIPPING_CONTAINER_PANEL_STATIC"] = true;
    allDictionary["SHIPPING_CONTAINER_STEPS"] = true;
    allDictionary["SHIPPING_CONTAINER_WEDGE_FLOOR"] = true;
    allDictionary["SHIPPING_CONTAINER_WEDGE_FOUNDATION"] = true;
    allDictionary["STEEL_DOOR"] = true;
    allDictionary["STEEL_STEPS"] = true;
    allDictionary["STRUCTURE"] = true;
    allDictionary["STRUCTURE_RAFT"] = true;
    allDictionary["STRUCTURE_SMALL"] = true;
    allDictionary["TARP_PANEL"] = true;
    allDictionary["TARP_PANEL_STATIC"] = true;
    allDictionary["TYRE"] = true;
    allDictionary["TYRE_PILE"] = true;
    allDictionary["VEHICLE_HELICOPTER"] = true;
    allDictionary["VEHICLE_LIFERAFT"] = true;
    allDictionary["VEHICLE_MOTOR"] = true;
    allDictionary["VEHICLE_SAIL"] = true;
    allDictionary["WOOD_ARCH"] = true;
    allDictionary["WOOD_DOOR"] = true;
    allDictionary["WOOD_FLOOR"] = true;
    allDictionary["WOOD_FOUNDATION"] = true;
    allDictionary["WOOD_PANEL_STATIC"] = true;
    allDictionary["WOOD_ROOF_CAP"] = true;
    allDictionary["WOOD_ROOF_CORNER"] = true;
    allDictionary["WOOD_ROOF_MIDDLE"] = true;
    allDictionary["WOOD_ROOF_WEDGE"] = true;
    allDictionary["WOOD_STEPS"] = true;
    allDictionary["WOOD_WALL_HALF"] = true;
    allDictionary["WOOD_WALL_WINDOW"] = true;
    allDictionary["WOOD_WEDGE_FLOOR"] = true;
    allDictionary["WOOD_WEDGE_FOUNDATION"] = true;
    allDictionary["CONTAINER_CONSOLE"] = true;
    allDictionary["CONTAINER_CRATE"] = true;
    allDictionary["CONTAINER_LOCKER_LARGE"] = true;
    allDictionary["CONTAINER_LOCKER_SMALL"] = true;
    allDictionary["BED"] = true;
    allDictionary["BED_SHELTER"] = true;
    allDictionary["BRICK_STATION"] = true;
    allDictionary["CLAY"] = true;
    allDictionary["CLOTH"] = true;
    allDictionary["DRIFTWOOD_STICK"] = true;
    allDictionary["ENGINE"] = true;
    allDictionary["ENGINE_FUEL_TANK"] = true;
    allDictionary["ENGINE_PROPELLER"] = true;
    allDictionary["ENGINE_PUMP"] = true;
    allDictionary["FIRE_TORCH"] = true;
    allDictionary["FURNACE"] = true;
    allDictionary["HOBO_STOVE"] = true;
    allDictionary["KINDLING"] = true;
    allDictionary["LEATHER"] = true;
    allDictionary["LEAVES_FIBROUS"] = true;
    allDictionary["LOOM"] = true;
    allDictionary["NEW_CAMPFIRE"] = true;
    allDictionary["NEW_CAMPFIRE_PIT"] = true;
    allDictionary["NEW_CAMPFIRE_SPIT"] = true;
    allDictionary["PALM_FROND"] = true;
    allDictionary["PLANK_STATION"] = true;
    allDictionary["RAIN_CATCHER"] = true;
    allDictionary["RAWHIDE"] = true;
    allDictionary["ROCK"] = true;
    allDictionary["ROPE_COIL"] = true;
    allDictionary["SMOKER"] = true;
    allDictionary["STICK"] = true;
    allDictionary["STONE_TOOL"] = true;
    allDictionary["TANNING_RACK"] = true;
    allDictionary["WATER_STILL"] = true;
    allDictionary["EGG_DEADEX"] = true;
    allDictionary["EGG_WOLLIE"] = true;	
    allDictionary["ALOE_VERA_FRUIT"] = true;
    allDictionary["ALOE_VERA_PLANT"] = true;
    allDictionary["FARMING_HOE"] = true;
    allDictionary["FARMING_PLOT_CORRUGATED"] = true;
    allDictionary["FARMING_PLOT_PLANK"] = true;
    allDictionary["FARMING_PLOT_WOOD"] = true;
    allDictionary["KURA_FRUIT"] = true;
    allDictionary["KURA_TREE"] = true;
    allDictionary["QUWAWA_FRUIT"] = true;
    allDictionary["QUWAWA_TREE"] = true;
    allDictionary["CAN_BEANS"] = true;
    allDictionary["CAN_BEANS_OPEN"] = true;
    allDictionary["COCONUT_DRINKABLE"] = true;
    allDictionary["COCONUT_HALF"] = true;
    allDictionary["COCONUT_ORANGE"] = true;
    allDictionary["MEAT_LARGE"] = true;
    allDictionary["MEAT_MEDIUM"] = true;
    allDictionary["MEAT_SMALL"] = true;
    allDictionary["POTATO"] = true;
    allDictionary["WATER_BOTTLE"] = true;
    allDictionary["WATER_SKIN"] = true;
    allDictionary["CORRUGATED_SHELF"] = true;
    allDictionary["CORRUGATED_TABLE"] = true;
    allDictionary["PLANK_CHAIR"] = true;
    allDictionary["PLANK_SHELF"] = true;
    allDictionary["PLANK_TABLE"] = true;
    allDictionary["WOOD_CHAIR"] = true;
    allDictionary["WOOD_HOOK"] = true;
    allDictionary["WOOD_SHELF"] = true;
    allDictionary["WOOD_TABLE"] = true;
    allDictionary["ANTIBIOTICS"] = true;
    allDictionary["BANDAGE"] = true;
    allDictionary["MEDICAL_ALOE_SALVE"] = true;
    allDictionary["MORPHINE"] = true;
    allDictionary["NEW_COCONUT_FLASK"] = true;
    allDictionary["NEW_COCONUT_MEDICAL"] = true;
    allDictionary["VITAMINS"] = true;
    allDictionary["MISSION_EEL"] = true;
    allDictionary["MISSION_MARKER"] = true;
    allDictionary["MISSION_SHARK"] = true;
    allDictionary["MISSION_SQUID"] = true;
    allDictionary["MISSION_TROPHY_EEL"] = true;
    allDictionary["MISSION_TROPHY_SHARK"] = true;
    allDictionary["MISSION_TROPHY_SQUID"] = true;
    allDictionary["MINING_ROCK"] = true;
    allDictionary["MINING_ROCK_CLAY"] = true;
    allDictionary["ALOCASIA_1"] = true;
    allDictionary["ALOCASIA_2"] = true;
    allDictionary["BANANA_PLANT"] = true;	
    allDictionary["BIGROCK_1"] = true;
    allDictionary["BIGROCK_2"] = true;
    allDictionary["BIGROCK_3"] = true;
    allDictionary["BIGROCK_4"] = true;
    allDictionary["BIGROCK_5"] = true;
    allDictionary["CERIMAN_1"] = true;
    allDictionary["CERIMAN_2"] = true;
    allDictionary["CERIMAN_3"] = true;
    allDictionary["CLIFF_001"] = true;
    allDictionary["CLIFF_002"] = true;
    allDictionary["CLIFF_003"] = true;
    allDictionary["CLIFF_004"] = true;
    allDictionary["CLIFF_005"] = true;
    allDictionary["CLIFF_006"] = true;	
    allDictionary["DRIFTWOOD_DECAL"] = true;
    allDictionary["OCEAN_BUOY"] = true;
    allDictionary["PHILODENDRON_1"] = true;
    allDictionary["PHILODENDRON_2"] = true;
    allDictionary["SHORELINE_ROCK_1"] = true;
    allDictionary["SHORELINE_ROCK_2"] = true;
    allDictionary["SMALLROCK_1"] = true;
    allDictionary["SMALLROCK_2"] = true;
    allDictionary["SMALLROCK_3"] = true;
    allDictionary["SeaFort_1"] = true;
    allDictionary["SeaFort_2"] = true;
    allDictionary["SeaFort_Brige"] = true;
    allDictionary["SeaFort_Brige_Broken"] = true;
    allDictionary["DOOR"] = true;
    allDictionary["ROWBOAT_3"] = true;
    allDictionary["SHIPWRECK_2A"] = true;
    allDictionary["SHIPWRECK_3A"] = true;
    allDictionary["SHIPWRECK_4A"] = true;
    allDictionary["SHIPWRECK_5A"] = true;
    allDictionary["SHIPWRECK_6A"] = true;
    allDictionary["SHIPWRECK_7A"] = true;
    allDictionary["COMPASS"] = true;
    allDictionary["DUCTTAPE"] = true;
    allDictionary["FLARE_GUN"] = true;
    allDictionary["FUELCAN"] = true;
    allDictionary["LABEL_MAKER"] = true;
    allDictionary["LANTERN"] = true;
    allDictionary["MACHETTE"] = true;
    allDictionary["NEW_AIRTANK"] = true;
    allDictionary["NEW_ARROW"] = true;
    allDictionary["NEW_CRUDE_AXE"] = true;
    allDictionary["NEW_CRUDE_BOW"] = true;
    allDictionary["NEW_CRUDE_HAMMER"] = true;
    allDictionary["NEW_CRUDE_SPEAR"] = true;
    allDictionary["NEW_FISHING_SPEAR"] = true;
    allDictionary["NEW_REFINED_AXE"] = true;
    allDictionary["NEW_REFINED_HAMMER"] = true;
    allDictionary["NEW_REFINED_KNIFE"] = true;
    allDictionary["NEW_REFINED_PICK"] = true;
    allDictionary["NEW_REFINED_SPEAR"] = true;
    allDictionary["NEW_SPEARGUN"] = true;
    allDictionary["NEW_SPEARGUN_ARROW"] = true;
    allDictionary["TORCH"] = true;
    allDictionary["COCA_BUSH"] = true;
    allDictionary["DRIFTWOOD_PILE"] = true;
    allDictionary["FICUS_1"] = true;
    allDictionary["FICUS_2"] = true;
    allDictionary["FICUS_3"] = true;
    allDictionary["FICUS_TREE"] = true;
    allDictionary["FICUS_TREE_2"] = true;
    allDictionary["LOG_0"] = true;
    allDictionary["LOG_1"] = true;
    allDictionary["LOG_2"] = true;
    allDictionary["PALM_1"] = true;
    allDictionary["PALM_2"] = true;
    allDictionary["PALM_3"] = true;
    allDictionary["PALM_4"] = true;
    allDictionary["PALM_LOG_1"] = true;
    allDictionary["PALM_LOG_2"] = true;
    allDictionary["PALM_LOG_3"] = true;
    allDictionary["PALM_TOP"] = true;
    allDictionary["PINE_1"] = true;
    allDictionary["PINE_2"] = true;
    allDictionary["PINE_3"] = true;
    allDictionary["PINE_SMALL_1"] = true;
    allDictionary["PINE_SMALL_2"] = true;
    allDictionary["PINE_SMALL_3"] = true;
    allDictionary["POTATO_PLANT"] = true;
    allDictionary["YOUNG_PALM_1"] = true;
    allDictionary["YOUNG_PALM_2"] = true;
    allDictionary["YUCCA"] = true;
    allDictionary["AJUGA_PLANT"] = true;
    allDictionary["AJUGA"] = true;
    allDictionary["WAVULAVULA_PLANT"] = true;
    allDictionary["WAVULAVULA"] = true;
    allDictionary["PIPI_PLANT"] = true;
    allDictionary["PIPI"] = true;
    allDictionary["COCONUT_FLASK"] = true;
}