// i fucking hate you stupid javascript
class FastRandom {
	
	REAL_UNIT_INT = 4.6566128730773926E-10;
	REAL_UNIT_UINT = 2.3283064365386963E-10;
	Y = 842502087;
	Z = 3579807591;
	W = 273326509;
	x = 0;
	y = 0;
	z = 0;
	w = 0;
	bitBuffer = 0;
	bitMask = 1; // ?
	
	constructor(seed){
		this.Reinitialise(seed);
	}
	
	Reinitialise(seed)
	{
		this.x = seed;
		this.y = this.Y;
		this.z = this.Z;
		this.w = this.W;
	}
	
	Next(lowerBound, upperBound)
	{
		if (lowerBound > upperBound)
		{
			return -1;
		}
		//uint num = x ^ (x << 11);
		let num = (this.x ^ (this.x << 11 >>> 0)) >>> 0;
		this.x = this.y;
		this.y = this.z;
		this.z = this.w;
		let num2 = upperBound - lowerBound;
		if (num2 < 0)
		{
			//return lowerBound + (int)(2.3283064365386963E-10 * (double)(w = (w ^ (w >> 19) ^ (num ^ (num >> 8)))) * (double)((long)upperBound - (long)lowerBound));
			//return lowerBound + Math.trunc(2.3283064365386963E-10 * (this.w = (this.w ^ (this.w >> 19) ^ ((num ^ (num >>> 8)) >>> 0) )) * (upperBound - lowerBound));
			
			this.w = ((this.w ^ (this.w >>> 19) >>> 0) ^ ((num ^ (num >>> 8)) >>> 0) ) >>> 0;
			return lowerBound + Math.trunc(2.3283064365386963E-10 * this.w * (upperBound - lowerBound));
		}
		//return lowerBound + (int)(4.6566128730773926E-10 * (double)(int)(int.MaxValue & (w = (w ^ (w >> 19) ^ (num ^ (num >> 8))))) * (double)num2);
		//return lowerBound + Math.trunc(4.6566128730773926E-10 * Math.trunc(2147483647 & (this.w = (this.w ^ (this.w >> 19) ^ ((num ^ (num >>> 8)) >>> 0) ))) * num2);
		
		this.w = ((this.w ^ (this.w >>> 19) >>> 0) ^ ((num ^ (num >>> 8)) >>> 0) ) >>> 0;
		
		let result = lowerBound + Math.trunc(4.6566128730773926E-10 * Math.trunc(2147483647 & this.w) * num2);
		//console.log(this.x+"$"+this.y+"$"+this.z+"$"+this.w+"$"+num+"$"+num2+"$"+result);
		return result;
	}
	
	NextDouble()
	{
		let num = (this.x ^ (this.x << 11 >>> 0)) >>> 0;
		this.x = this.y;
		this.y = this.z;
		this.z = this.w;
		//return 4.6566128730773926E-10 * (double)(int)(int.MaxValue & (w = (w ^ (w >> 19) ^ (num ^ (num >> 8)))));
		//return 4.6566128730773926E-10 * Math.trunc(2147483647 & (this.w = (this.w ^ (this.w >> 19) ^ ((num ^ (num >>> 8)) >>> 0)) ));
		this.w = ((this.w ^ (this.w >>> 19) >>> 0) ^ ((num ^ (num >>> 8)) >>> 0)) >>> 0;
		let result = 4.6566128730773926E-10 * Math.trunc(2147483647 & this.w);
		//console.log(this.x+"$"+this.y+"$"+this.z+"$"+this.w+"$"+num+"$0$"+result);
		return result;
	}
}