namespace Task2.API.Models
{
	public class CreateProductForm
	{
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public string? Category { get; set; }
		public string? Brand { get; set; }
		public IFormFile? File { get; set; }
	}

	public class UpdateProductForm
	{
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
		public string? Category { get; set; }
		public string? Brand { get; set; }
		public bool IsActive { get; set; } = true;
		public IFormFile? File { get; set; }
	}
}
