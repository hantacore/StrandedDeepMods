class Vector2 {
	
	fValue = 0;
	imprecisionX = 0;
	imprecisionY = 0;
	
	constructor(x, y) {
		this.x = x;
		this.y = y;
	}
	
	sqrMagnitude() 
	{
		return this.x * this.x + this.y * this.y;
	}
	
	addXY(x, y)
	{
		return new Vector2(this.x + x, this.y + y);
	}
	
	substractXY(x, y)
	{
		return new Vector2(this.x - x, this.y - y);
	}
	
	multiply(m)
	{
		return new Vector2(this.x * m, this.y * m);
	}
	
	divide(d)
	{
		return new Vector2(this.x / d, this.y / d);
	}
}