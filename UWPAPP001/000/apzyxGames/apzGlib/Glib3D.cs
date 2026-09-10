using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Foundation;
using Windows.Graphics;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace apzGlib
{
    public class Cámara
    {
        public Átomo Centro { get; set; }

        public Átomo Sentido { get; set; }

        public Int32 Ancho { get; set; }

        public Int32 Alto { get; set; }

        public Cámara(Int32 ancho, Int32 alto, Átomo centro, Átomo sentido)
        {
            Ancho = ancho;
            Alto = alto;
            Centro = centro;
            Sentido = sentido;
        }

        public bool IsPointOnLine(double[] A, double[] B, double[] C)
        {
            // Calculamos los vectores AB y AC
            double[] AB = { B[0] - A[0], B[1] - A[1], B[2] - A[2] };
            double[] AC = { C[0] - A[0], C[1] - A[1], C[2] - A[2] };

            // Verificamos si los vectores son proporcionales
            double t1 = AB[0] != 0 ? AC[0] / AB[0] : 0;
            double t2 = AB[1] != 0 ? AC[1] / AB[1] : 0;
            double t3 = AB[2] != 0 ? AC[2] / AB[2] : 0;

            return (t1 == t2) && (t2 == t3);
        }

        public IEnumerable<double[]> GenerateParallelLines(double[] A, double[] B, double height, double width)
        {
            // Vector director de la recta original
            double[] d = { B[0] - A[0], B[1] - A[1], B[2] - A[2] };

            // Generamos puntos en el plano y rectas paralelas
            for (double i = -height / 2; i <= height / 2; i += 1)
            {
                for (double j = -width / 2; j <= width / 2; j += 1)
                {
                    double[] P = { B[0] + i, B[1] + j, B[2] };
                    yield return P;
                }
            }
        }
    }
    /// <summary>Objetorrrrr</summary>
    public class Obtjeto
    {
        /// <summary>Mallas que componen el objeto</summary>
        public List<Entidad> Entidades { get; set; }
    }



    /// <summary>Malla que define un objeto o parte de él</summary>
    public class Entidad
    {
        /// <summary>El punto central del objeto en coordenadas esféricas</summary>
        public Átomo Centro { get; set; }

        /// <summary>Gradiente de distribución de malla</summary>
        public Double Gradiente { get; set; }

        /// <summary>Una lista de polígonos que definen la entidad</summary>
        public ConcurrentBag<Átomo> Malla { get; set; }

        /// <summary>Otras mallas correlacionadas</summary>
        public ConcurrentBag<Correlación> Correlaciones { get; set; }


        public Entidad()
        {
            // Inicializamos
            Malla = new ConcurrentBag<Átomo>();
            Correlaciones = new ConcurrentBag<Correlación>();
        }

        public static Entidad CrearEsfera(Átomo centro, Double radio, Double grad, Material materia = null, ConcurrentBag<Correlación> corrs = null)
        {
            // Inicializamos y asociamos
            Entidad entidad = new Entidad();
            centro.Padre = entidad;
            entidad.Centro = centro;
            entidad.Correlaciones = corrs;

            // Calculamos la malla

            // En el caso de la esfera
            // proyectamos en todas las direcciones hasta la distancia radio máxima
            // con el gradiente como unidad minima de distancia entre puntos de malla

            for (double j = 0; j < 360; j += grad)
            {
                for (double k = 0; k <= 360; k += grad)
                {
                    for (double i = 0 + grad; i <= radio; i += grad)
                    {
                        Átomo a = new Átomo(i, j, k);
                        a.Padre = entidad;
                        a.Material = materia;
                        entidad.Malla.Add(a);
                    }
                }
            }

            return entidad;
        }

        public static async Task<Entidad> CrearCubo(Átomo centro, Double lado, Double grad, ConcurrentBag<Correlación> corrs = null, Material mat = null)
        {

            // Inicializamos y asociamos
            Entidad entidad = new Entidad();
            centro.Padre= entidad;
            entidad.Centro = centro;
            entidad.Correlaciones = corrs;

            // Calculamos la malla

            // En el caso del cubo

            ConcurrentBag<Átomo> cuboVérticesRelativos = new ConcurrentBag<Átomo>();

            List<Task> tasks = new List<Task>();
            int maxDegreeOfParallelism = 100; // Número máximo de tareas en paralelo
            SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            for (double y = centro.Y - (lado / 2); y <= centro.Y + (lado / 2); y += grad)
            {
                for (double x = centro.X - (lado / 2); x <= centro.X + (lado / 2); x += grad)
                {
                    for (double z = centro.Z - (lado / 2); z <= centro.Z + (lado / 2); z += grad)
                    {
                        double yL = y;
                        double xL = x;
                        double zL = z;

                        await semaphore.WaitAsync();

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                try
                                {
                                    cuboVérticesRelativos.Add(new Átomo(xL, yL, zL, mat) { Padre = entidad });
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);

                }
            }


            entidad.Malla = cuboVérticesRelativos;

            // Con la hipotenusa podremos obtener el punto medio en la bisectriz del cubo
            // deberemos sumar y restar lado medios en las diferentes direcciones
            // y lo haremos como un proceso de incremento el gradiente para obtener la malla






            return entidad;
        }


        public static Entidad CrearCuboCoorEsf(Átomo centro, Double lado, Double grad, ConcurrentBag<Correlación> corrs = null, Material mat = null)
        {
            // Inicializamos y asociamos
            Entidad entidad = new Entidad();
            centro.Padre = entidad;
            entidad.Centro = centro;
            entidad.Correlaciones = corrs;

            // Calculamos la malla

            // En el caso del cubo

            // Si tenemos las corrdenadas centrales y tenemos el lado, calculamos la hipotenusa
            // se puede observar un triángulo rectangulo con ambos catetos con valor lado medios

            ConcurrentBag<Átomo> cuboVérticesRelativos = new ConcurrentBag<Átomo>();

            foreach (Int16 rhoIter in new List<Int16>() { 45, 135 })
            {
                foreach (Int16 thetaIter in new List<Int16>() { 45, 135, 225, 315 })
                {
                    double phiVR = lado * Math.Sqrt(3) / 2;
                    double rhoVR = rhoIter;
                    double thetaVR = thetaIter;

                    cuboVérticesRelativos.Add(new Átomo(rhoVR, thetaVR, phiVR, mat) { Padre = entidad });
                }
            }


            entidad.Malla = cuboVérticesRelativos;

            // Con la hipotenusa podremos obtener el punto medio en la bisectriz del cubo
            // deberemos sumar y restar lado medios en las diferentes direcciones
            // y lo haremos como un proceso de incremento el gradiente para obtener la malla






            return entidad;
        }
    }

    public class Correlación
    {
        /// <summary>Malla correlacinada con la malla padre</summary>
        public Entidad MallaO { get; set; }

        /// <summary>Distancia de la separación entre la correlación de centros</summary>
        public Double Distancia { get; set; }

        /// <summary>Obtiene o establece la relación de tipo de la correlación</summary>
        public TiposDeCorrelación TipoDeCorrelación { get; set; }

    }

    public enum TiposDeCorrelación
    {
        Estática,
        Dinámica1
    }




    public class Átomo
    {
        public Entidad Padre { get; set; }

        public double X { get; set; } // La distancia desde el origen al vértice
        public double Y { get; set; } // El ángulo azimutal del vértice
        public double Z { get; set; } // El ángulo polar del vértice

        public Material Material { get; set; }

        public Átomo(double x, double y, double z, Material mat = null)
        {
            // Asignar los valores de los parámetros a las propiedades del vértice
            X = x;
            Y = y;
            Z = z;
            Material = mat;
        }
    }


    public class Material
    {
        public double Resistencia { get; set; }

        public Color Color { get; set; }


    }

}
