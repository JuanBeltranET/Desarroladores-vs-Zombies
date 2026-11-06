using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D debugTexture;

        private Texture2D zombie;//f898f8
        private Texture2D superviviente;
        private Texture2D background;
        private Texture2D Zombiesas;//00a0f8
        private Texture2D ZombieTank;
        private Texture2D barraVIDA;//ffffff
        private Texture2D barraST;
        private Texture2D pistola;//0000f8
        private Texture2D gameover;
        private Texture2D Items;
        // private Texture2D inventario;
        // Imagenes del menu
        private Texture2D fondoMenu;
        private Texture2D botonJugar;
        //musica
        private Song backgroundMusic;
        private Song MusicaMenu;

        // Estados del juego
        enum EstadoJuego
        {
            Menu,
            Jugando
        }

        EstadoJuego estadoActual = EstadoJuego.Menu;

        // Boton Jugar
        Rectangle rectBotonJugar = new Rectangle(224, 190, 192, 96);

        // Control del mouse
        MouseState mouseAnterior;

        // Control del teclado
        KeyboardState tecladoAnterior;

        // HITBOXES
        Rectangle hitboxZombie = new Rectangle(600, 100, 35, 50);
        Rectangle hitboxJugador;
        Rectangle hitboxZombieSal = new Rectangle(300, 100, 45, 57);
        Rectangle hitboxZombieTank = new Rectangle(500, 100, 35, 57);
        Rectangle hitboxItem = new Rectangle(550, 75 ,64, 64);

        // Limites del pasto
        Rectangle limitesPasto = new Rectangle(160, 30, 625, 420);

        // Movimiento de Fiolina
        Vector2 posicionFiolina = new Vector2(220, 100);
        float velocidadFiolina = 150f;

        // POSICIONES DE LOS ZOMBIES
        Vector2 posicionZombie = new Vector2(600, 100);
        Vector2 posicionZombieSal = new Vector2(300, 100);
        Vector2 posicionZombieTank = new Vector2(500, 100);

        //posicion Item
        Vector2 posicionItem = new Vector2(550, 75);


        // VELOCIDAD DE LOS ZOMBIES
        float velocidadZombie = 65f;
        float velocidadZombieSal = 90f;
        float velocidadZombieTank = 35f;

        // Estado de movimiento
        bool moviendo = false;

        // Sprite de Fiolina quieta
        Rectangle spriteQuieto = new Rectangle(71, 3, 29, 50);

        // Animacion de Fiolina
        float timer;
        int threshold = 100;

        Rectangle[] sourceRectangles;

        int currentAnimationIndex = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            debugTexture = new Texture2D(GraphicsDevice, 1, 1);

            debugTexture.SetData(new[] { Color.Red });

            // CARGAR IMAGENES DEL JUEGO
            zombie = Content.Load<Texture2D>("imagenes/Zombie");

            superviviente = Content.Load<Texture2D>("imagenes/FiolinaSprite");

            background = Content.Load<Texture2D>("imagenes/Background");

            Zombiesas = Content.Load<Texture2D>("imagenes/ZombieSalvaje");

            ZombieTank = Content.Load<Texture2D>("imagenes/ZombieTanque");

            barraVIDA = Content.Load<Texture2D>("imagenes/HealthBar");

            barraST = Content.Load<Texture2D>("imagenes/StaminaMeter");

            Items = Content.Load<Texture2D>("imagenes/Items");
            // inventario = Content.Load<Texture2D>("imagenes/Inventario");

            pistola = Content.Load<Texture2D>("imagenes/Arma");

            gameover = Content.Load<Texture2D>("imagenes/GameOver");

            // CARGAR IMAGENES DEL MENU
            fondoMenu = Content.Load<Texture2D>("imagenes/FondoMenu");

            botonJugar = Content.Load<Texture2D>("imagenes/Botones");

            // MUSICA
            backgroundMusic = Content.Load<Song>("musica/04. Grasswalk");

            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(backgroundMusic);

            // CONFIGURACION DE ANIMACION
            timer = 0;

            sourceRectangles = new Rectangle[8];

            sourceRectangles[0] = new Rectangle(8, 56, 33, 41);
            sourceRectangles[1] = new Rectangle(39, 56, 33, 41);
            sourceRectangles[2] = new Rectangle(70, 56, 33, 41);
            sourceRectangles[3] = new Rectangle(103, 56, 34, 41);
            sourceRectangles[4] = new Rectangle(137, 56, 33, 40);
            sourceRectangles[5] = new Rectangle(170, 56, 34, 41);
            sourceRectangles[6] = new Rectangle(205, 56, 34, 41);
            sourceRectangles[7] = new Rectangle(244, 56, 34, 41);

            currentAnimationIndex = 0;
        }

        // REINICIAR POSICIONES
        private void ReiniciarPosiciones()
        {
            posicionFiolina = new Vector2(220, 100);

            posicionZombie = new Vector2(600, 100);
            posicionZombieSal = new Vector2(600, 150);
            posicionZombieTank = new Vector2(600, 200);

            hitboxZombie = new Rectangle(600, 100, 35, 50);
            hitboxZombieSal = new Rectangle(300, 100, 45, 57);
            hitboxZombieTank = new Rectangle(500, 100, 35, 57);

            hitboxJugador = new Rectangle(220, 100, 30, 45);

            timer = 0;
            currentAnimationIndex = 0;

            moviendo = false;
        }

        // METODO PARA PERSEGUIR A FIOLINA
        private void PerseguirJugador(
            ref Vector2 posicion,
            ref Rectangle hitbox,
            float velocidad,
            float tiempo)
        {
            // Centro de Fiolina
            Vector2 centroJugador = posicionFiolina + new Vector2(15, 22);

            // Centro del zombie
            Vector2 centroZombie = new Vector2(
                hitbox.X + hitbox.Width / 2,
                hitbox.Y + hitbox.Height / 2
            );

            // Calcular direccion hacia Fiolina
            Vector2 direccion = centroJugador - centroZombie;

            // Comprobar que no esten en la misma posicion
            if (direccion.Length() > 0)
            {
                direccion.Normalize();

                // Mover zombie hacia Fiolina
                posicion += direccion * velocidad * tiempo;
            }

            // Actualizar hitbox
            hitbox = new Rectangle(
                (int)posicion.X,
                (int)posicion.Y,
                hitbox.Width,
                hitbox.Height
            );
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState teclado = Keyboard.GetState();

            MouseState mouseActual = Mouse.GetState();

            // SALIR DEL JUEGO
            if (teclado.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // MENU PRINCIPAL
            if (estadoActual == EstadoJuego.Menu)
            {
                // Detectar clic del mouse
                if (mouseActual.LeftButton == ButtonState.Pressed &&
                    mouseAnterior.LeftButton == ButtonState.Released)
                {
                    // Comprobar si se hizo clic en Jugar
                    if (rectBotonJugar.Contains(mouseActual.Position))
                    {
                        estadoActual = EstadoJuego.Jugando;

                        ReiniciarPosiciones();
                    }
                }

                mouseAnterior = mouseActual;

                tecladoAnterior = teclado;

                base.Update(gameTime);

                return;
            }

            // VOLVER AL MENU
            if (teclado.IsKeyDown(Keys.Back) &&
                tecladoAnterior.IsKeyUp(Keys.Back))
            {
                estadoActual = EstadoJuego.Menu;

                ReiniciarPosiciones();

                mouseAnterior = mouseActual;

                tecladoAnterior = teclado;

                base.Update(gameTime);

                return;
            }

            tecladoAnterior = teclado;

            mouseAnterior = mouseActual;

            // MOVIMIENTO DEL JUEGO
            float tiempo =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            moviendo = false;

            // Velocidad de movimiento
            float velocidadActual = velocidadFiolina;

            // Correr con Z
            if (teclado.IsKeyDown(Keys.Z))
            {
                velocidadActual *= 2;
            }

            // MOVIMIENTO HORIZONTAL
            float movimientoX = 0;

            if (teclado.IsKeyDown(Keys.Right))
            {
                movimientoX = velocidadActual * tiempo;

                moviendo = true;
            }

            if (teclado.IsKeyDown(Keys.Left))
            {
                movimientoX = -velocidadActual * tiempo;

                moviendo = true;
            }



            // Aplicar movimiento horizontal
            posicionFiolina.X += movimientoX;

            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            // Colision con los limites del pasto
            if (!limitesPasto.Contains(hitboxJugador))
            {
                posicionFiolina.X -= movimientoX;
            }

            // MOVIMIENTO VERTICAL
            float movimientoY = 0;

            if (teclado.IsKeyDown(Keys.Up))
            {
                movimientoY = -velocidadActual * tiempo;

                moviendo = true;
            }

            if (teclado.IsKeyDown(Keys.Down))
            {
                movimientoY = velocidadActual * tiempo;

                moviendo = true;
            }

            // Aplicar movimiento vertical
            posicionFiolina.Y += movimientoY;

            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            // Colision con los limites del pasto
            if (!limitesPasto.Contains(hitboxJugador))
            {
                posicionFiolina.Y -= movimientoY;
            }

            // ANIMACION DE FIOLINA
            if (moviendo)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (timer >= threshold)
                {
                    currentAnimationIndex++;

                    if (currentAnimationIndex >= sourceRectangles.Length)
                    {
                        currentAnimationIndex = 0;
                    }

                    timer = 0;
                }
            }
            else
            {
                currentAnimationIndex = 0;

                timer = 0;
            }

            // HITBOX DE FIOLINA
            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            // PERSECUCION DE LOS ZOMBIES

            // Zombie normal
            PerseguirJugador(
                ref posicionZombie,
                ref hitboxZombie,
                velocidadZombie,
                tiempo
            );

            // Zombie salvaje
            PerseguirJugador(
                ref posicionZombieSal,
                ref hitboxZombieSal,
                velocidadZombieSal,
                tiempo
            );

            // Zombie tanque
            PerseguirJugador(
                ref posicionZombieTank,
                ref hitboxZombieTank,
                velocidadZombieTank,
                tiempo
            );

            // COLISIONES CON LOS ZOMBIES

            // Zombie normal
            if (hitboxJugador.Intersects(hitboxZombie))
            {
                Debug.WriteLine("colision");

                posicionFiolina = new Vector2(220, 100);

                posicionZombie = new Vector2(600, 100);
            }

            // Zombie salvaje
            if (hitboxJugador.Intersects(hitboxZombieSal))
            {
                Debug.WriteLine("colision");

                posicionFiolina = new Vector2(220, 100);

                posicionZombieSal = new Vector2(600, 150);

            }

            // Zombie tanque
            if (hitboxJugador.Intersects(hitboxZombieTank))
            {
                Debug.WriteLine("colision");

                posicionFiolina = new Vector2(220, 100);

                posicionZombieTank = new Vector2(600, 200);
            }

            // Actualizar hitbox despues de una colision
            hitboxJugador = new Rectangle(
                (int)posicionFiolina.X,
                (int)posicionFiolina.Y,
                30,
                45
            );

            //colicion Item
            if (hitboxJugador.Intersects(hitboxItem))
            {
                Debug.WriteLine("colicion");

                velocidadActual *= 2000000;
            }

            base.Update(gameTime);
        }

        // DIBUJAR MENU
        private void DibujarMenu()
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Fondo del menu
            _spriteBatch.Draw(
                fondoMenu,
                new Rectangle(
                    0,
                    0,
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height
                ),
                Color.White
            );

            // Boton Jugar
            _spriteBatch.Draw(
                botonJugar,
                rectBotonJugar,
                Color.White
            );

            _spriteBatch.End();
        }

        // DIBUJAR JUEGO
        protected override void Draw(GameTime gameTime)
        {
            // Si estamos en el menu
            if (estadoActual == EstadoJuego.Menu)
            {
                DibujarMenu();

                base.Draw(gameTime);

                return;
            }

            GraphicsDevice.Clear(Color.Orange);

            _spriteBatch.Begin();

            Rectangle Zquieto = new Rectangle(0, 0, 35, 50);

            Rectangle ZSquieto = new Rectangle(4, 329, 45, 57);

            Rectangle ZTquieto = new Rectangle(4, 10, 35, 57);

            Rectangle barraVIDA1 = new Rectangle(0, 0, 48, 45);

            Rectangle barraVIDA2 = new Rectangle(200, 2, 48, 45);

            Rectangle barraVIDA3 = new Rectangle(400, 2, 48, 45);

            Rectangle barraSTAM = new Rectangle(3, 4, 164, 35);

            Rectangle Pistola1 = new Rectangle(260, 41, 294, 62);

            Rectangle ItemV = new Rectangle(0, 129, 64, 64);


            // FONDO DEL NIVEL
            _spriteBatch.Draw(
                background,
                new Vector2(0, -10),
                null,
                Color.White,
                0f,
                new Vector2(10, 25),
                0.8f,
                SpriteEffects.None,
                0f
            );

            // FIOLINA
            if (moviendo)
            {
                // Animacion de movimiento
                _spriteBatch.Draw(
                    superviviente,
                    posicionFiolina,
                    sourceRectangles[currentAnimationIndex],
                    Color.White
                );
            }
            else
            {
                // Sprite de Fiolina quieta
                _spriteBatch.Draw(
                    superviviente,
                    posicionFiolina,
                    spriteQuieto,
                    Color.White
                );
            }

            // ZOMBIE NORMAL
            _spriteBatch.Draw(
                zombie,
                posicionZombie,
                Zquieto,
                Color.White
            );

            // ZOMBIE SALVAJE
            _spriteBatch.Draw(
                Zombiesas,
                posicionZombieSal,
                ZSquieto,
                Color.White
            );

            // ZOMBIE TANQUE
            _spriteBatch.Draw(
                ZombieTank,
                posicionZombieTank,
                ZTquieto,
                Color.White
            );

            //items
            _spriteBatch.Draw(
                Items,
                posicionItem,
                ItemV,
                Color.White
                );


            // BARRA DE STAMINA
            _spriteBatch.Draw(
                barraST,
                new Vector2(20, 425),
                barraSTAM,
                Color.White
            );

            /*
            _spriteBatch.Draw(
                inventario,
                new Vector2(20, 20),
                Color.White
            );
            */



            // BARRA DE VIDA
            if (hitboxJugador.Intersects(hitboxZombieSal))
            {
                _spriteBatch.Draw(
                    barraVIDA,
                    new Vector2(100, 50),
                    barraVIDA2,
                    Color.White
                );

                if (hitboxJugador.Intersects(hitboxZombieSal))
                {
                    _spriteBatch.Draw(
                        barraVIDA,
                        new Vector2(100, 50),
                        barraVIDA3,
                        Color.White
                    );

                    /*if (hitboxJugador.Intersects(hitboxZombieSal))
                    {
                        _spriteBatch.Draw(
                            gameover,
                            new Vector2(100, 50),
                            Color.White
                        );

                    }
                    */
                }

            }
            else
            {
                _spriteBatch.Draw(
                    barraVIDA,
                    new Vector2(100, 50),
                    barraVIDA1,
                    Color.White
                );
            }

            if (hitboxJugador.Intersects(hitboxZombie))
            {
                _spriteBatch.Draw(
                    barraVIDA,
                    new Vector2(100, 50),
                    barraVIDA2,
                    Color.White
                );

                if (hitboxJugador.Intersects(hitboxZombie))
                {
                    _spriteBatch.Draw(
                        barraVIDA,
                        new Vector2(100, 50),
                        barraVIDA3,
                        Color.White
                    );

                    /* if (hitboxJugador.Intersects(hitboxZombie))
                     {
                         _spriteBatch.Draw(
                             gameover,
                             new Vector2(100, 50),
                             Color.White
                         );

                     }
                    */
                }

            }
            else
            {
                _spriteBatch.Draw(
                    barraVIDA,
                    new Vector2(100, 50),
                    barraVIDA1,
                    Color.White
                );
            }

            if (hitboxJugador.Intersects(hitboxZombieTank))
            {
                _spriteBatch.Draw(
                    barraVIDA,
                    new Vector2(100, 50),
                    barraVIDA2,
                    Color.White
                );

                if (hitboxJugador.Intersects(hitboxZombieTank))
                {
                    _spriteBatch.Draw(
                        barraVIDA,
                        new Vector2(100, 50),
                        barraVIDA3,
                        Color.White
                    );

                    /* if (hitboxJugador.Intersects(hitboxZombieTank))
                     {
                         _spriteBatch.Draw(
                             gameover,
                             new Vector2(100, 50),
                             Color.White
                         );

                     }
                     */
                }

            }
            else
            {
                _spriteBatch.Draw(
                    barraVIDA,
                    new Vector2(100, 50),
                    barraVIDA1,
                    Color.White
                );
            }



            // HITBOXES
            _spriteBatch.Draw(
                debugTexture,
                hitboxJugador,
                Color.White * 0.4f
            );

            _spriteBatch.Draw(
                debugTexture,
                hitboxZombie,
                Color.White * 0.4f
            );

            _spriteBatch.Draw(
                debugTexture,
                hitboxZombieSal,
                Color.White * 0.4f
            );

            _spriteBatch.Draw(
                debugTexture,
                hitboxZombieTank,
                Color.White * 0.4f
            );

            _spriteBatch.Draw(
                debugTexture,
                hitboxItem,
                Color.White
                );


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}