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
		}
		else if (scale > 0.2) {
			scale = 0.9;
			ctx.scale(scale, scale);
		}
		
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
		
		// focus on player?
		
		// marginLeft = scale * marginLeft;
		// marginTop = scale * marginTop;
		// c.style.marginLeft = marginLeft + "px";
		// c.style.marginTop = marginTop + "px";
    }
}

function clearAll() {
	let c = document.getElementById("mapCanvas");
	let ctx = c.getContext("2d");
	
	ctx.restore();
	
	ctx.fillStyle = "#000088";
	ctx.fillRect(0, 0, c.width, c.height);
	
	ctx.fillStyle = "#ccffff";
	ctx.fillRect(0, 0, width, height);
}

function drawGrid() {
	clearAll();
	
	let c = document.getElementById("mapCanvas");

	//width = c.width;
	//height = c.height;
	//offsetX = width/2;
	//offsetY = height/2;
	
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

function drawMap() {

	if (typeof(newArr) == "undefined"
		|| !newArr.hasOwnProperty("Version")) {
		alert("No save file loaded !");
		return;
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
						&& !otherDictionary.hasOwnProperty(key)) {
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
						//ctx.beginPath();
						//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
						//ctx.fillStyle = '#5A4D41';
						//ctx.fill();
						//ctx.closePath();
						
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
						//ctx.beginPath();
						//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
						//ctx.fillStyle = '#234311';
						//ctx.fill();
						//ctx.closePath();
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
							//ctx.beginPath();
							//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
							//ctx.fillStyle = 'purple';
							//ctx.fill();
							//ctx.closePath();
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
							//ctx.beginPath();
							//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
							//ctx.fillStyle = 'red';
							//ctx.fill();
							//ctx.closePath();
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
							//ctx.beginPath();
							//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
							//ctx.fillStyle = 'orange';
							//ctx.fill();
							//ctx.closePath();
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
						//ctx.beginPath();
						//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
						//ctx.fillStyle = 'purple';
						//ctx.fill();
						//ctx.closePath();
						let imageObj = new Image();
						imageObj.style.zindex = 5000;
						imageObj.src = './icons/tool.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 10, calcy - 10, 20, 20);
						};
					}
					
					if (itemsDictionary.hasOwnProperty(key))
					{
						//ctx.beginPath();
						//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
						//ctx.fillStyle = 'purple';
						//ctx.fill();
						//ctx.closePath();
						let imageObj = new Image();
						imageObj.style.zindex = 5000;
						imageObj.src = './icons/item.png';
						imageObj.onload = function () {
							ctx.drawImage(imageObj, calcx - 6, calcy - 6, 12, 12);
						};
					}
					
					if (raftsDictionary.hasOwnProperty(key))
					{
						//ctx.beginPath();
						//ctx.arc(calcx, calcy, 2, 0, 2 * Math.PI, false);
						//ctx.fillStyle = 'purple';
						//ctx.fill();
						//ctx.closePath();
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
							//ctx.beginPath();
							//ctx.arc(calcx, calcy, 5, 0, 2 * Math.PI, false);
							//ctx.fillStyle = 'yellow';
							//ctx.fill();
							//ctx.closePath()
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
				let calcx = x * zoneXwidth + 0.5 * zoneXwidth + Number(newArr.Persistent.StrandedWorld.PlayerPosition["x"]) * (width / 1000);
				let calcy = y * zoneYwidth + 0.5 * zoneYwidth - Number(newArr.Persistent.StrandedWorld.PlayerPosition["y"]) * (height / 1000);
				
				//var calcx = x * zoneXwidth + 0.5 * zoneXwidth + Number(newArr.Persistent.StrandedWorld.PlayerPosition["x"]);
				//var calcy = y * zoneYwidth + 0.5 * zoneYwidth + Number(newArr.Persistent.StrandedWorld.PlayerPosition["z"]);
			
				let imageObj = new Image();
				imageObj.style.zindex = 7000;
				imageObj.src = './icons/player.png';
				imageObj.onload = function () {
					ctx.drawImage(imageObj, calcx - 12, calcy - 12, 24, 24);
				};
			
				//ctx.beginPath();
				//ctx.arc(calcx, calcy, 10, 0, 2 * Math.PI, false);
				//ctx.closePath();
				//ctx.fillStyle = 'red';
				//ctx.fill();
				//ctx.lineWidth = 1;
				//ctx.strokeStyle = '#003300';
				//ctx.stroke();
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
var otherDictionary = {};

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
	treesDictionary["FICUS_3"] = true;
	treesDictionary["KURA_TREE"] = true;
	treesDictionary["PALM_1"] = true;
	treesDictionary["PALM_4"] = true;
	treesDictionary["PALM_3"] = true;
	treesDictionary["PALM_2"] = true;
	treesDictionary["CERIMAN_3"] = true;
	treesDictionary["ALOCASIA_1"] = true;
	treesDictionary["CERIMAN_1"] = true;
	
	rocksDictionary["ROCK"] = true;
	rocksDictionary["CLIFF_005"] = true;
	rocksDictionary["CLIFF_003"] = true;
	rocksDictionary["CLIFF_001"] = true;
	rocksDictionary["CLIFF_006"] = true;
	rocksDictionary["CLIFF_002"] = true;
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

	animalsDictionary["SHARK_TIGER"] = true;
	animalsDictionary["SHARK_REEF"] = true;
	animalsDictionary["SHARK_WHITE"] = true;
	animalsDictionary["MARLIN"] = true;
	animalsDictionary["GREAT_SEA_TURTLE"] = true;
	animalsDictionary["WHALE"] = true;
	animalsDictionary["CRAB_HOME"] = true;
	animalsDictionary["BOAR"] = true;
	animalsDictionary["CRAB"] = true;
	animalsDictionary["SNAKE"] = true;
	animalsDictionary["STING_RAY"] = true;
	animalsDictionary["PATROL_REEFSHARK"] = true;
	animalsDictionary["PATROL_TIGERSHARK"] = true;
	animalsDictionary["HIDESPOT_SNAKE"] = true;
	
	
	itemsDictionary["STICK"] = true;
	itemsDictionary["DRIFTWOOD_STICK"] = true;
	itemsDictionary["BARREL"] = true;
	itemsDictionary["BUOYBALL"] = true;
	
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
	
	
	toolsDictionary["NEW_REFINED_PICK"] = true;
	toolsDictionary["NEW_FISHING_SPEAR"] = true;
	toolsDictionary["FARMING_HOE"] = true;
	toolsDictionary["KINDLING"] = true;

	
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

	
	structureSavableDictionary["BED_SHELTER"] = true;

	fortsDictionary["Sea_Fort_1"] = true;
	fortsDictionary["Sea_Fort_2"] = true;
	fortsDictionary["Sea_Fort_3"] = true;
	fortsDictionary["Sea_Fort_Bridge"] = true;
	
	otherDictionary["DRIFTWOOD_DECAL"] = true;
}