namespace API_TFG.Models.Enum
{
    public enum ActionType
    {
        Upload = 1,          // Subir archivo
        Download = 2,        // Descargar archivo
        Delete = 3,          // Eliminar archivo
        Share = 4,           // Compartir archivo
        Edit = 5,            // Editar archivo o metadatos
        View = 6,            // Ver archivo
        Rename = 7,          // Renombrar archivo
        Move = 8,            // Mover archivo
        Restore = 9,         // Restaurar archivo eliminado
        ChangePermission = 10, // Cambiar permisos de acceso
        Other = 99           // Acción no especificada
    }
}
