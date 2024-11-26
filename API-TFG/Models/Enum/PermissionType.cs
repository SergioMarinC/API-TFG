namespace API_TFG.Models.Enum
{
    public enum PermissionType
    {
        Read = 1,        // Solo lectura
        Write = 2,       // Lectura y edición
        Delete = 3,      // Eliminar el archivo
        Share = 4,       // Compartir el archivo
        FullAccess = 5   // Todos los permisos
    }
}
