using Microsoft.SqlServer.Server;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;
using System;

namespace InventarioApi.DTOs;

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default);
