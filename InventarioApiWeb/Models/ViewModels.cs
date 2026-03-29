using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario no puede estar vacio")]
    [Display(Name = "Usuario")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña no puede estar vacia")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;
}

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public bool RequiresRestocking { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductViewModel
{
    [Required(ErrorMessage = "El nombre no puede ser vacio")]
    [StringLength(150, ErrorMessage = "Maximo 150 caracteres")]
    [Display(Name = "Nombre del Producto")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio no puede estar vacio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    [Display(Name = "Precio")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El stock actual no puede estar vacio")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    [Display(Name = "Stock Actual")]
    public int CurrentStock { get; set; }

    [Required(ErrorMessage = "El stock minimo no puede estar vacio")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock minimo no puede ser negativo")]
    [Display(Name = "Stock Minimo")]
    public int MinimumStock { get; set; }
}

public class SaleItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class SaleViewModel
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string SoldBy { get; set; } = string.Empty;
    public List<SaleItemViewModel> Items { get; set; } = new();
}

public class RegisterSaleViewModel
{
    public List<SaleLineViewModel> Lines { get; set; } = new() { new SaleLineViewModel() };
}

public class SaleLineViewModel
{
    [Required(ErrorMessage = "Seleccione un producto.")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad no puede estar vacia")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; } = 1;
}

public class LowStockViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public int StockDeficit { get; set; }
}