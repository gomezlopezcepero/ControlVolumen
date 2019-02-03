using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsControlLibrary1
{

   

    [ToolboxBitmap(typeof(Volumen), "altavoz.png")]

    

    public partial class Volumen: UserControl
    {



        Boolean voz = true;
        Boolean arrastrando = false;
        Bitmap altavoz =null;
        int longitudBarra=220;
        int longitudPico = 35;
        int diferencia = 0;

        public Volumen()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
           
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            this.Width = (int)(this.Height) * 2;

        }


        public enum EstadoVol
        {
            Silencio,
            Bajo,
            Medio,
            Alto,
        };


        [Description("Nivel de volúmen del control"), DefaultValue(EstadoVol.Alto)]

        public String Nivel
        {
            get { return label1.Text.Substring(0, label1.Text.Length-1); }
            set {
                try
                {
                    if (int.Parse(value) >= 0 && int.Parse(value) <= 100)
                    {
                        longitudBarra =  60+(160 * int.Parse(value) / 100);
                        int valor = longitudBarra;

                        calculoColores();

                        longitudBarra = valor;


                        calculoPico(longitudBarra);
                        calculoPorcentaje();
                        Invalidate();
                    }
                }
                catch(Exception e)
                {

                }
            }
        }


        public class EstadoArgs : EventArgs
        {
            private EstadoVol est;
            public EstadoArgs(EstadoVol nuevoEstado)
            {
                est = nuevoEstado;
            }
            public EstadoVol Volumen
            {
                get { return est; }
            }
        }


       



        public delegate void CamobioValor(object sender, EstadoArgs e);
        public event CamobioValor CambioEstado;


        private EstadoVol estado = EstadoVol.Alto;

        [Description("Estado del volumen del control"), DefaultValue(EstadoVol.Alto)]



        public EstadoVol Estado
        {
            get { return estado; }
            set
            {
                estado = value;
                label1.Text=estado.ToString();
                if (estado.ToString().Equals("Alto"))
                {
                    longitudBarra = 220;
                    calculoPico(longitudBarra);
                    calculoPorcentaje();
                }
                else if (estado.ToString().Equals("Medio"))
                {
                    longitudBarra = 172;
                    calculoPico(longitudBarra);
                    calculoPorcentaje();

                }
                else if (estado.ToString().Equals("Bajo"))
                {
                    longitudBarra = 100;
                    calculoPico(longitudBarra);
                    calculoPorcentaje();
                }
                else if (estado.ToString().Equals("Silencio"))
                {
                    voz = false;
                    longitudBarra = 60;
                }

                Invalidate();
                var evento = this.CambioEstado;
                if (evento != null)
                    evento(this, new EstadoArgs(estado));
            }
        }






        //METODO ONPAINT

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            Brush[] vol = new Brush[]{
                new SolidBrush(Color.Gray),
                new SolidBrush(Color.Gray),
                new SolidBrush(Color.Gray)};

            int cont = 0;

            switch (estado)
            {
                case EstadoVol.Bajo:
                    vol[2] = new SolidBrush(Color.Lime);
                    cont = 2;
                   
                        break;
                case EstadoVol.Medio:
                    vol[1] = new SolidBrush(Color.Yellow);
               
                        cont = 1;
                    break;
                case EstadoVol.Alto:
                    vol[0] = new SolidBrush(Color.Red);
                    
                    cont = 0;
                    break;
            }

            if (voz||longitudBarra>65)
            {
                 altavoz = WindowsFormsControlLibrary1.Properties.Resources.altavozOn;
            }
            else
            {
                 altavoz = WindowsFormsControlLibrary1.Properties.Resources.altavozOff;
            }
            
            
            Point point1 = new Point(60, 86);
            Point point2 = new Point(60, 100);
            Point point3 = new Point(longitudBarra, 100);
            Point point4 = new Point(longitudBarra, longitudPico);

            Point[] curvePoints = { point1, point2, point3, point4 };

            // Draw polygon to screen.
            e.Graphics.FillPolygon(vol[cont], curvePoints);

            g.DrawImage(altavoz, 5, 67);

            

        }


        private void UserControl1_Click(object sender, EventArgs e)
        {
         

            
        }

        //ALTAVOZ

        private void UserControl1_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Location.X > 5 && e.Location.X < 35 && e.Location.Y > 67 && e.Location.Y < 97)
            {
                if (voz)
                {
                    voz = false;
                    longitudBarra = 60;
                    this.Estado = EstadoVol.Silencio;
                }
                else
                {
                    voz = true;
                    longitudBarra = 220;
                    longitudPico = 35;
                    this.Estado = EstadoVol.Alto;
                }
            }

            Invalidate();
        }

        private void UserControl1_Layout(object sender, LayoutEventArgs e)
        {
            if (e.AffectedProperty == "Bounds")
            {
                this.Width = (int)(this.Height) *2;
                Invalidate();
            }
        }



        //ARRASTRAR BARRA

        private void UserControl1_MouseDown(object sender, MouseEventArgs e)
        {

            arrastrando = true;
        }

        private void UserControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastrando && (e.Location.X > 60 && e.Location.X < 220) && (e.Location.Y < 100 && e.Location.Y > 35))
            {
                int valor =  e.Location.X;

                calculoColores();

                longitudBarra = valor;

                calculoPico(longitudBarra);
                calculoPorcentaje();

            }
        }

        private void UserControl1_MouseUp(object sender, MouseEventArgs e)
        {
            arrastrando = false;
        }
        






        private void calculoPico(int longitudBarra)
        {

            int cont = 0;
            int longitud = 220 - longitudBarra;
            while (longitud > 3)
            {
                longitud = longitud - 3;
                cont++;
            }

            longitudPico = cont + 35;

        }

        private Double calculoPorcentaje()
        {
            Double percent = longitudBarra - 60;

            percent = Math.Truncate(((percent) / 159) * 100);
            label1.Text = percent + "%";

            return percent;
        }


        private void calculoColores()
        {
            if ((calculoPorcentaje() >= 0 && calculoPorcentaje() < 3))
            {
                this.Estado = EstadoVol.Silencio;
                voz = false;
            }
            else
            {
                voz = true;
            }

            if ((calculoPorcentaje() > 0 && calculoPorcentaje() <= 50))
                this.Estado = EstadoVol.Bajo;

            if ((calculoPorcentaje() > 50 && calculoPorcentaje() < 90))
                this.Estado = EstadoVol.Medio;


            if ((calculoPorcentaje() >= 90 && calculoPorcentaje() <= 100))
                this.Estado = EstadoVol.Alto;
        }
        
        //CONTROLES DE TECLADO

        private void UserControl1_KeyUp(object sender, KeyEventArgs e)
        {
            int valor = 0;
            if (e.KeyCode == Keys.Left)
            {
                if (calculoPorcentaje() >= 10)
                {
                    longitudBarra -= 16;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (calculoPorcentaje() <= 90)
                {
                    longitudBarra += 16;
                }
            }

            valor = longitudBarra;

            calculoColores();

            longitudBarra = valor;

            calculoPico(longitudBarra);
            calculoPorcentaje();
            Invalidate();
        }
    }
}
