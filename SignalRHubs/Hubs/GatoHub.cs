using Microsoft.AspNetCore.SignalR;

namespace SignalRHubs.Hubs
{
    public class GatoHub : Hub
    {

        public static Dictionary<string, string> usuarios = new Dictionary<string, string>();// existe para todas las instancias
        public async Task IniciarSesion(string nombreUsuario)
        {
            //verifica si el usuario esta en uso
            if (usuarios.Values.Any(x =>
            x.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase)
            ))
            {
                //enviar mensaje de error 
                await Clients.Caller.SendAsync("ReviciveMensaje", "error", "El nombre de usuario ya está en uso");
            }
            else
            {
                usuarios[Context.ConnectionId] = nombreUsuario;
                await Clients.Caller.SendAsync("ReciveMensaje", "ok", "Sesión iniciada");

            }
        }


        public static Queue<string> colaUsuarios = new Queue<string>();
        public async Task BuscarPartida(string nombreUsuario)
        {

        }


    }
}
