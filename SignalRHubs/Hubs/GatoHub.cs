using Microsoft.AspNetCore.SignalR;

namespace SignalRHubs.Hubs
{
    public class GatoHub : Hub
    {

        public static Dictionary<string, string> usuarios = new Dictionary<string, string>();// existe para todas las instancias
        public async Task IniciarSesion(string nombreUsuario)
        {
            //verifica si el usuario esta en uso
            if (usuarios.Keys.Any(x =>
            x.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase)
            ))
            {
                //enviar mensaje de error 
                await Clients.Caller.SendAsync("ReviciveMensaje", "error", "El nombre de usuario ya está en uso");
            }
            else
            {
                usuarios[nombreUsuario] =Context.ConnectionId;
                await Clients.Caller.SendAsync("ReciveMensaje", "ok", "Sesión iniciada");

            }
        }


        public static Queue<string> colaUsuarios = new Queue<string>();

        public static int NumPartida = 0;
        

        public async Task BuscarPartida(string nombreUsuario)
        {
            if (colaUsuarios.Count == 0)
            {
                colaUsuarios.Enqueue(nombreUsuario);
            }
            else
            {
                var contrincante=colaUsuarios.Dequeue();
                string partida=$"partida {NumPartida} ";
                await Groups.AddToGroupAsync(Context.ConnectionId, partida);
                await Groups.AddToGroupAsync(usuarios[contrincante], partida);
                NumPartida++;
                await Clients.Groups(partida).SendAsync("Game Started",partida);
                await Clients.Users(Context.ConnectionId).SendAsync("Play");
            }
        }



        public async Task Jugar(string partida, string nombreUsuario,string tablero)
        {

        }

        


    }
}
