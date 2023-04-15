// right now i'm very close to giving up
class ZonePositionGenerator {

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
		let fastRandom = new FastRandom(WORLD_SEED);
		while (mylist2.length > 0)
        {
			let index = fastRandom.Next(0, mylist2.length);
			let vector = mylist2[index];
			let flag = false;
			var i;
			for (i = 0; i < numSamplesBeforeRejection; i++)
            {
				let randomvalue = fastRandom.NextDouble();
				let f = (randomvalue.toFixed(8) * Math.PI.toFixed(8) * 2).toFixed(8);
				let a = new Vector2(Math.sin(f), Math.cos(f));
				let nextRandom = fastRandom.Next(Math.trunc(radius * 100), Math.trunc(1.25 * radius * 100));
				let vectortemp = a.multiply(nextRandom / 100);
                let vector2 = vector.addXY(vectortemp.x, vectortemp.y);
				if (i == 0)
                {
					vector2 = vector;
                }
				if (this.IsValid(vector2, sampleRegionSize, num, radius, mylist, myarray))
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
			//console.log("list2 length = " + mylist2.length);
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
	

	IsValid(candidate, sampleRegionSize, cellSize, radius, points, mygrid)
	{
		if (candidate.x >= 0 && candidate.x < (sampleRegionSize.x + 0.999999) && candidate.y >= 0 && candidate.y < (sampleRegionSize.y + 0.999999)) // error margin due to JS + 0.999999
		{
			let num = Math.trunc(candidate.x / cellSize);
			let num2 = Math.trunc(candidate.y / cellSize);
			let num3 = Math.max(0, num - 2);
			let num4 = Math.min(num + 2, mygrid.length - 1);
			let num5 = Math.max(0, num2 - 2);
			let num6 = Math.min(num2 + 2, mygrid[0].length - 1);
			var i, j;
			for (i = num3; i <= num4; i++)
			{
				for (j = num5; j <= num6; j++)
				{
					let num7 = mygrid[i][j] - 1;
					if (num7 != -1)
					{
						let sqrMagnitude = candidate.substractXY(points[num7].x, points[num7].y).sqrMagnitude();
						if (sqrMagnitude < (radius * radius)) // error margin due to JS  * 0.9995
						{
							//console.log("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / false (sqmagnitude="+sqrMagnitude+" / radius2="+radius * radius+")");
							return false;
						}
					}
				}
			}
			//console.log("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / true");
			return true;
		}
		//console.log("IsValid : x=" + candidate.x + " / y=" + candidate.y + " / false (sampleRegionSize.x="+sampleRegionSize.x+" / sampleRegionSize.y="+sampleRegionSize.y+")");
		return false;
	}
}