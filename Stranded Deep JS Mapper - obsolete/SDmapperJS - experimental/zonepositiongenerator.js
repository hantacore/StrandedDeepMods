// right now i'm very close to giving up
class ZonePositionGenerator {

	isDebug = false;
	isDevDebug = false;

	shittyValues = new Array();
	
	constructor(debug, devdebug) {
		this.isDebug = debug;
		this.isDevDebug = devdebug
		
		// add seeds then shitty values
		this.addShittyValueSeed("13285285");
		this.rejectShittyValue("13285285", "250000.00000000003", true);
		
		this.addShittyValueSeed("28898405");
		this.rejectShittyValue("28898405", "249999.99999999983", false);
	}
	
	addShittyValueSeed(seed)
	{
		this.shittyValues[seed] = new Array();
	}
	
	rejectShittyValue(seed, shittyvalue, reject)
	{
		this.shittyValues[seed][shittyvalue] = reject;
	}

	round(value, decimals) {
	  return Number(Math.round(value+'e'+decimals)+'e-'+decimals);
	}

    //customPI = 3.1415926535897931;
	customPI = 3.14159274; // .net method converts PI to float
	
	GeneratePoints(WORLD_SEED, radius, sampleRegionSize, numSamplesBeforeRejection = 30) {
		let num = radius / Math.sqrt(2);
		let myarray = new Array();//new int[Math.ceil(sampleRegionSize.x / num), Math.ceil(sampleRegionSize.y / num)];
		// JS specific init loop
		var x, y;
		for(x = 0; x < Math.ceil(sampleRegionSize.x / num); x++)
		{
			myarray.push(new Array());
			for(y = 0; y < Math.ceil(sampleRegionSize.y / num); y++)
			{
				myarray[x].push(0);
			}
		}
		
		let mylist = new Array();//new List<Vector2>();
        let mylist2 = new Array();//new List<Vector2>();
		mylist2.push(sampleRegionSize.divide(2));
		let fastRandom = new FastRandom(WORLD_SEED, this.isDevDebug);
		while (mylist2.length > 0)
        {
			let index = fastRandom.Next(0, mylist2.length);
			let vector = mylist2[index];
			let flag = false;
			var i;
			for (i = 0; i < numSamplesBeforeRejection; i++)
            {
				// 0.20 algo
				// let randomvalue = fastRandom.NextDouble();
				// let f = (randomvalue.toFixed(8) * Math.PI.toFixed(8) * 2).toFixed(8);
				// let a = new Vector2(Math.sin(f), Math.cos(f));
				// let nextRandom = fastRandom.Next(Math.trunc(radius * 100), Math.trunc(1.25 * radius * 100));
				// let vectortemp = a.multiply(nextRandom / 100);
                // let vector2 = vector.addXY(vectortemp.x, vectortemp.y);
				
				// TESTS
				
				let randomvalue = fastRandom.NextDouble();
				
				//let f = (new Number(randomvalue).toFixed(8) * this.customPI * 2).toPrecision(7); // seems ok
				
				let f = (new Number(randomvalue).toFixed(8) * this.customPI * 2);
				if (f > 0.001)
				{
					f = this.round((new Number(randomvalue).toFixed(8) * this.customPI * 2), 9).toPrecision(7)
				}
				else if (f > 0.01)
				{
					f = this.round((new Number(randomvalue).toFixed(8) * this.customPI * 2), 8).toPrecision(7)
				}
				else if (f > 0.1)
				{
					f = this.round((new Number(randomvalue).toFixed(8) * this.customPI * 2), 7).toPrecision(7)
				}
				else if (f > 1)
				{
					f = this.round((new Number(randomvalue).toFixed(8) * this.customPI * 2), 6).toPrecision(7)
				}
				if (f > 10)
				{
					f = this.round((new Number(randomvalue).toFixed(8) * this.customPI * 2), 5).toPrecision(7)
				}
				
				// compute a direction
				let sinf = Math.sin(f);
				let cosf = Math.cos(f);
				// TEST
				let a = new Vector2(sinf, cosf);
				//let a = new Vector2(sinf.toPrecision(7), cosf.toPrecision(7));
				// compute a velocity
				let nextRandom = fastRandom.Next(Math.trunc(radius * 100), Math.trunc(1.25 * radius * 100));
				let vectortemp = a.multiply(Math.trunc(nextRandom / 100));
				// add the vector to the previous one
                let vector2 = vector.addXY(vectortemp.x, vectortemp.y);
				vector2.fValue = f;
				
				if (this.isDevDebug) {
					writeDeveloperLog("sin/cos$" + sinf + "$" + cosf);
					writeDeveloperLog(randomvalue + "$" + f + "$" + a.x + "$" + a.y + "$" + nextRandom + "$" + vector2.x + "$" + vector2.y);
				}
				
				// END TESTS
				
				if (i == 0)
                {
					vector2 = vector;
                }
				if (this.IsValid(WORLD_SEED, vector2, sampleRegionSize, num, radius, mylist, myarray))
                {
                    mylist.push(vector2);
                    mylist2.push(vector2);
                    myarray[Math.trunc(vector2.x / num)][Math.trunc(vector2.y / num)] = mylist.length;
                    flag = true;
                    break;
                }
			}
			if (!flag)
            {
				mylist2.splice(index, 1);
            }
			if (this.isDevDebug) {				
				//console.log("list2 length = " + mylist2.length);
				writeDeveloperLog("list2 length = " + mylist2.length);
			}
		}
		let array2 = Array();//new Vector2[list.Count];
		// JS specific init loop
		var i;
		for (i = 0; i < mylist.length; i++)
		{
			array2.push(new Vector2(0, 0));
		}
		
		var j;
        for (j = 0; j < mylist.length; j++)
        {
            let num2 = sampleRegionSize.x * 0.5;
            // ref Vector2 reference = ref array2[j];
            let vector3 = mylist[j];
            let x = vector3.x - num2;
            let vector4 = mylist[j];
            // reference = new Vector2(x, vector4.y - num2);
			array2[j] = new Vector2(x, vector4.y - num2);
        }
        return array2;
	}
	

	IsValid(seed, candidate, sampleRegionSize, cellSize, radius, points, mygrid)
	{	
		if (candidate.x >= -0.0001 && candidate.x < (sampleRegionSize.x + 0.0001) && candidate.y >= -0.0001 && candidate.y < (sampleRegionSize.y + 0.0001))
		{
			let num = Math.trunc(candidate.x / cellSize);
			let num2 = Math.trunc(candidate.y / cellSize);
			let num3 = Math.max(0, num - 2);
			let num4 = Math.min(num + 2, mygrid.length - 1);
			let num5 = Math.max(0, num2 - 2);
			let num6 = Math.min(num2 + 2, mygrid[0].length - 1);
			if (this.isDevDebug) {
				writeDeveloperLog("IsValid : num " + num + " / num2 " + num2 + " / num3 " + num3 + " / num4 " + num4 + " / num5 " + num5 + " / num6 " + num6);
			}
			var i, j;
			for (i = num3; i <= num4; i++)
			{
				for (j = num5; j <= num6; j++)
				{
					let num7 = mygrid[i][j] - 1;
					if (num7 != -1)
					{
						let sqrMagnitude = candidate.substractXY(points[num7].x, points[num7].y).sqrMagnitude();
						if (this.isDevDebug) {
							writeDeveloperLog("sqmagnitude=" + sqrMagnitude + " | radius² =" + radius * radius + "$" + points[num7].x + "$" + points[num7].y + "$" + candidate.x + "$" + candidate.y);
						}
						
						// check if distance between islands is large enough (radius²), some value are calculation errors
						if (this.shittyValues.hasOwnProperty(seed)
							&& this.shittyValues[seed].hasOwnProperty(sqrMagnitude))
						{
							// check shitty values explicitely reject
							if (this.shittyValues[seed][sqrMagnitude])
							{
								if (this.isDevDebug) {
									writeDeveloperLog("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / false (sqmagnitude="+sqrMagnitude+" / radius2="+radius * radius+") user rejected shitty value");
								}
								console.log("Shitty value rejected");
								return false;
							}
							// else value is accepted
							console.log("Shitty value accepted");
						}
						else
						{	
							if (Math.abs(sqrMagnitude - (radius * radius)) < 0.0000001
								|| Number(Math.abs(sqrMagnitude - (radius * radius)).toFixed(8)) == 0)
							{								
								// delta 0.0005
								let error = 0;
								
								if (candidate.fValue < 10)
								{
									error = 0.0002;
								} else {
									error = 0.002;
								}
								
								// let numxerror = new Array();
								// numxerror.push(points[num7].x);
								// numxerror.push(points[num7].x + error);
								// numxerror.push(points[num7].x - error);
								
								// let numyerror = new Array();
								// numyerror.push(points[num7].y);
								// numyerror.push(points[num7].y + error);
								// numyerror.push(points[num7].y - error);
								
								// let candidatexerror = new Array();
								// candidatexerror.push(candidate.x);
								// candidatexerror.push(candidate.x + error);
								// candidatexerror.push(candidate.x - error);
								
								// let candidateyerror = new Array();
								// candidateyerror.push(candidate.y);
								// candidateyerror.push(candidate.y + error);
								// candidateyerror.push(candidate.y - error);
								
								let numxerror = new Array();
								numxerror.push(0);
								numxerror.push(error);
								numxerror.push(-error);
								
								let numyerror = new Array();
								numyerror.push(0);
								numyerror.push(error);
								numyerror.push(-error);
								
								let candidatexerror = new Array();
								candidatexerror.push(0);
								candidatexerror.push(error);
								candidatexerror.push(-error);
								
								let candidateyerror = new Array();
								candidateyerror.push(0);
								candidateyerror.push(error);
								candidateyerror.push(-error);
								
								let reject = 0;
								let total = 0;
								for(let nx = 0 ; nx < numxerror.length ; nx++) {
									for(let ny = 0 ; ny < numyerror.length ; ny++) {
										for(let cx = 0 ; cx < candidatexerror.length ; cx++) {
											for(let cy = 0 ; cy < candidateyerror.length ; cy++) {
												// errors must be of opposite signs
												if ((candidatexerror[cx] * candidateyerror[cy] <= 0) && (numxerror[nx] * numyerror[ny] <= 0))
												{
													total += 1;
													let tempmagnitude = new Vector2(candidate.x + candidatexerror[cx], candidate.y + candidateyerror[cy]).substractXY(points[num7].x + numxerror[nx], points[num7].y + numyerror[ny]).sqrMagnitude();
													if (tempmagnitude < (radius * radius)) {
														reject += 1;
													}
												}
											}
										}
									}
								}
								let maxrejects = 2 * (total / 3);
								console.log("Rejected ? " + reject + " >? " + maxrejects + " / " + total);
								if (reject > maxrejects)
								{
									console.log("Border value rejected");
									if (this.isDevDebug) {
										writeDeveloperLog("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / false (sqmagnitude="+sqrMagnitude+" / radius2="+radius * radius+") rejected = " + reject + " > " + maxrejects + " / " + total);
									}
									return false;
								}
								console.log("Border value accepted");
							}
							// if sqrmagnitude imprecision enough
							else if (sqrMagnitude < (radius * radius))
							{
								if (this.isDevDebug) {
									writeDeveloperLog("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / false (sqmagnitude="+sqrMagnitude+" / radius2="+radius * radius+")");
								}
								return false;
							}
						}
					}
				}
			}
			if (this.isDevDebug) {
				writeDeveloperLog("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / true");
			}
			return true;
		}
		if (this.isDevDebug) {
			writeDeveloperLog("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / false (sampleRegionSize.x="+sampleRegionSize.x+" / sampleRegionSize.y="+sampleRegionSize.y+")");
		}
		return false;
	}
}