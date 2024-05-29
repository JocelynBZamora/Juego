using Microsoft.AspNetCore.SignalR;
using SignalRHubs.Models;

namespace SignalRHubs.Hubs
{
    public class GatoHub : Hub
    {

        public static Dictionary<string, string> usuarios = new Dictionary<string, string>();// existe para todas las instancias
        
        
        public static Dictionary<string,Partida> Partidas=new Dictionary<string,Partida>();
        
        
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
                await Clients.Users(Context.ConnectionId).SendAsync("Play","         ");

                var datosPartida = new Partida()
                {
                    NombrePartida = partida,
                    NombreUsuario1 = nombreUsuario,
                    NombreUsuario2 = contrincante,
                    ConnectionId1 = Context.ConnectionId,
                    ConnectionId2 = usuarios[contrincante],
                    Turno = 'X'
                };
                Partidas[partida] = datosPartida;
            }
        }



        public async Task Jugar(string partida, string nombreUsuario,string tablero)
        {
            var datosPartida = Partidas[partida];
            if(GanoXO(datosPartida.Turno,tablero))
            {
                await Clients.Group(partida).SendAsync("GameOver",nombreUsuario);
            }
            else
            {
                datosPartida.Turno = datosPartida.Turno == 'X' ? '0' : 'X';
                var siguiente=datosPartida.Turno=='X'?datosPartida.ConnectionId1 : datosPartida.ConnectionId2;
                await Clients.Users(siguiente).SendAsync("Play",tablero);
            }
        }

        private bool GanoXO(char turno, string tablero)
        {
            
        }

        int[,] lineas = new int[,]
        {
            {0,1,2 },   //...........
            {3,4,5 },
            {6,7,8 },
            {0,3,6 },
            {1,4,7 },
            {2,5,8 }
        };


    }
}
