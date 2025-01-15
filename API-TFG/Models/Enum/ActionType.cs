namespace API_TFG.Models.Enum
{
    public enum ActionType
    {
        Upload = 1,          // Subir archivo
        Download = 2,        // Descargar archivo
        SoftDelete = 3,      // Eliminación lógica (IsDeleted = true)
        HardDelete = 4,      // Eliminación definitiva del archivo de la base de datos
        Share = 5,           // Compartir archivo con otro usuario
        UpdatePermissions = 6, // Actualizar los permisos de acceso
        Restore = 7,         // Restaurar archivo eliminado
        RevokeAccess = 8,    // Revocar el acceso compartido de un usuario
        Update = 9           // Actualizar file
    }
}
